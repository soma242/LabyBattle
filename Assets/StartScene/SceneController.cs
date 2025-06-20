using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

using MessagePipe;
//using Cysharp.Threading.Tasks;


//To_InputActionController
//var scenePub = GlobalMessagePipe.GetPublisher<InputSystemSwitch>();



public class SceneController : MonoBehaviour
{

    private System.IDisposable disposable;
    private System.IDisposable disposableBattle;

    //Titel‚É–ß‚é‚ÌSub
    private System.IDisposable disposableDungeon;
    private System.IDisposable disposableMenu;


    [SerializeField]
    private InputActionController layerHolder;

    void Awake()
    {

#if !UNITY_EDITOR
        SceneManager.LoadScene("StartSelectScene", LoadSceneMode.Additive);
#endif

        SetSub();
    }

    void OnDestroy()
    {
        disposable?.Dispose();
        disposableBattle?.Dispose(); 
        disposableDungeon?.Dispose();
        disposableMenu?.Dispose();
    }

    void OnApplicationQuit()
    {
        Debug.Log("quit");
        var quitPub = GlobalMessagePipe.GetPublisher<SaveCallMessage>();
        quitPub.Publish(new SaveCallMessage());
    }

    private void SetSub()
    {
        var bag = DisposableBag.CreateBuilder();

        var disableSub = GlobalMessagePipe.GetSubscriber<StartSceneDisableMessage>();


        //Start‰æ–Ê‚©‚çDungeon‚Ö
        disableSub.Subscribe(get => {
            //disposable?.Dispose();

            //•K‚¸“Ç‚İ‚İ‘O‚ÉiInputLayer‚Ì‹L˜^‡˜‚ª‹¶‚í‚È‚¢‚æ‚¤‚Éj
            var scenePub = GlobalMessagePipe.GetPublisher<InputSystemSwitch>();
            scenePub.Publish(new InputSystemSwitch(SceneName.dungeon));

            SceneManager.UnloadSceneAsync("StartSelectScene");


            //Debug.Log("pub");
            SceneManager.LoadScene("DungeonScene", LoadSceneMode.Additive);

            //SetEnableSub();

            //InputSystem‚ª‘¶İ‚µ‚Ä‚¢‚é‚Æ‘¼‚ÌScene‚É‰e‹¿‚ğ‹y‚Ú‚·
            //InputSystem‚Ìenabled‚ğ‘€ì‚µ‚Ä‚à•Ï‚í‚ç‚È‚¢
            //InputLayer‚Ìˆµ‚¢‚ª–â‘è‚É‚È‚Á‚Ä‚¢‚é

            //test
            //SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);

            RegistSkill();


            //title‚É–ß‚é
            var titleSub = GlobalMessagePipe.GetSubscriber<ToTitleMessage>();
            disposableDungeon = titleSub.Subscribe(get =>
            {
                SceneManager.UnloadSceneAsync("DungeonScene");
            });

        }).AddTo(bag);


        //Dungeon‚©‚çBattle‚Ö
        //From_FormationCommander
        var battleSub = GlobalMessagePipe.GetSubscriber<CallBattleMessage>();
        battleSub.Subscribe(async get =>
        {

            var scenePub = GlobalMessagePipe.GetPublisher<InputSystemSwitch>();
            scenePub.Publish(new InputSystemSwitch(SceneName.battle));
            //Debug.Log("load");
            await SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);

            var startPub = GlobalMessagePipe.GetPublisher<BattleSceneMessage.BattleStartMessage>();
            startPub.Publish(new BattleSceneMessage.BattleStartMessage());

            var sceneSub = GlobalMessagePipe.GetSubscriber<InputSystemSwitch>();

            //Battle‚©‚çDungeon‚É–ß‚é
            disposableBattle = sceneSub.Subscribe(get =>
            {
                disposableBattle?.Dispose();
                var layerPub = GlobalMessagePipe.GetPublisher<InputLayer>();
                layerPub.Publish(new InputLayer(layerHolder.dungeonLayer));

            });
        }).AddTo(bag);


        //Battle‚ğUnload
        var endSub = GlobalMessagePipe.GetSubscriber<EndBattleSceneMessage>();
        endSub.Subscribe(get => 
        {
            Debug.Log("battle unload");

            SceneManager.UnloadSceneAsync("BattleScene");
            var scenePub = GlobalMessagePipe.GetPublisher<InputSystemSwitch>();
            scenePub.Publish(new InputSystemSwitch(SceneName.dungeon));
        }).AddTo(bag);

        //Menu‚ğUnload
        var closeSub = GlobalMessagePipe.GetSubscriber<MenuToDungeonMessage>();
        closeSub.Subscribe(get =>
        {
            //Debug.Log(name);
            SceneManager.UnloadSceneAsync("MenuScene");
            var scenePub = GlobalMessagePipe.GetPublisher<InputSystemSwitch>();
            scenePub.Publish(new InputSystemSwitch(SceneName.dungeon));
        }).AddTo(bag);

        //Dungeon‚©‚çMenu
        var openSub = GlobalMessagePipe.GetSubscriber<DungeonToMenuMessage>();
        openSub.Subscribe(get =>
        {
            SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Additive);
            var scenePub = GlobalMessagePipe.GetPublisher<InputSystemSwitch>();
            scenePub.Publish(new InputSystemSwitch(SceneName.menu));

            var bag = DisposableBag.CreateBuilder();

            var titleSub = GlobalMessagePipe.GetSubscriber<ToTitleMessage>();
            titleSub.Subscribe(get =>
            {
                disposableMenu?.Dispose();
                SceneManager.UnloadSceneAsync("MenuScene");
            }).AddTo(bag);

            var closeSub = GlobalMessagePipe.GetSubscriber<MenuToDungeonMessage>();
            closeSub.Subscribe(get =>
            {
                disposableMenu?.Dispose();
                var layerPub = GlobalMessagePipe.GetPublisher<InputLayer>();
                layerPub.Publish(new InputLayer(layerHolder.dungeonLayer));
                Debug.Log("close");
            }).AddTo(bag);

            disposableMenu = bag.Build();
        }).AddTo(bag);


        //Title‚Ö
        var titleSub = GlobalMessagePipe.GetSubscriber<ToTitleMessage>();
        titleSub.Subscribe(get =>
        {
            var scenePub = GlobalMessagePipe.GetPublisher<InputSystemSwitch>();
            scenePub.Publish(new InputSystemSwitch(SceneName.start));
            SceneManager.LoadScene("StartSelectScene", LoadSceneMode.Additive);
        }).AddTo(bag);


        disposable = bag.Build();
    }

    /*
    private void SetEnableSub()
    {
        var enableSub = GlobalMessagePipe.GetSubscriber<StartSceneEnableMessage>();
        disposable = enableSub.Subscribe(get =>
        {
            disposable?.Dispose();

            SceneManager.LoadScene("StartSelectScene", LoadSceneMode.Additive);

            SetSub();

        });
    }
    */

    private async UniTask RegistSkill()
    {
        var registAPub = GlobalMessagePipe.GetAsyncPublisher<sbyte, SkillStruct.RegistSkillStart>();

        for (sbyte i = FormationScope.FirstChara(); i <= FormationScope.LastChara(); i++)
        {
            registAPub.Publish(i, new SkillStruct.RegistSkillStart());

        }
    }
}

