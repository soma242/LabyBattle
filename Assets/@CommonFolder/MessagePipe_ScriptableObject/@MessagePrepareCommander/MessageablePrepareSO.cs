using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using MessagePipe;


using SkillStruct;

//using MessageableManager;

//一つだけ（あるいは使用するコンテナごとに分けて）用意する（containerの用意と依存関係の注入順序は決まってなければならない）

/////手順
//MessageablePrepareSOへのMessageableInjectListSOの追加とアタッチ
//対応するMessageableInjectListSOへ全てのMessageableScriptableObjectのアタッチ



[CreateAssetMenu(menuName = "MessageableSO/Important/＊ひとつだけ用意MessageablePrepareSO")]
public class MessageablePrepareSO : ScriptableObject
{

    //[SerializeField] private MessageableInjectListSO activeSkillSO;
    [SerializeField] private List<MessageableInjectListSO> injectList = new List<MessageableInjectListSO>();



    public void OnEnable()
    {
        //プレイ時間外に働いたか確認用(OnEnableの作動タイミング確認につかった)
        //Debug.Log("enableRun");
        //OnEnable，Awake共に，インスペクタ上でこのSOを開いているかシーン上に参照を作らなければ走らなかった。
        //@zzzにアタッチしたSetProviderReferenceから参照を作っている。
        //実際に使用するときは最初に起動するシーンに参照を作ること。
        //一度動けばいいので他のシーンからの参照は消す

#if UNITY_EDITOR
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            //Debug.Log("Started");
#endif

            SetGlobalMessageProvider();


            //MessageableScriptableObjectの種類ごとに対応したInjectListから行う。
            //activeSkillSO.MessageDependencyInjection();
            foreach (MessageableInjectListSO injecter in injectList)
            {
                injecter.MessageDependencyInjection();
            }
#if UNITY_EDITOR
    }
#endif
    }


    //BuiltinContainerによるMessagePipe用のコンテナ用意
    //Brokerをコメントアウトすることで，Pubを探せる。（Startなどだけ）
    public void SetGlobalMessageProvider()
    {
        var builder = new BuiltinContainerBuilder();

        builder.AddMessagePipe(/* configure option */);

        // AddMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.

        //Common
        //builder.AddMessageBroker<SkillEnumKey, EntrySkillMessage>();

        //To_Formation, From_CharacterData
        builder.AddMessageBroker<sbyte, FormCharacterMessage>();

        //ToCharacterCommander
        builder.AddMessageBroker<FormCharacterBootMessage>();

        //builder.AddMessageBroker<RemoveFormCharacterMessage>();

        //To_ActiveSkillCommander
        builder.AddMessageBroker<ActiveSkillCommand>();

        //To_skillSoldier
        builder.AddMessageBroker<NormalAttack>();

        //To_DamageCalc
        builder.AddMessageBroker<sbyte, NormalDamageCalcMessage>();

        //To_MoveSKillCommander
        builder.AddMessageBroker<MoveSkillCommand>();
        builder.AddMessageBroker<TargetingMoveSkillCommand>();

        //To_CharaActionSimulator
        builder.AddMessageBroker<sbyte, ChangeActionMessage>();

        //スキル登録， To_Formation
        builder.AddMessageBroker<RegistCommonSkill>();

        builder.AddMessageBroker<sbyte, RegistSkillStart>();
        builder.AddMessageBroker<RegistSkillFinish>();


        builder.AddMessageBroker<SelectSkillListChangeMessage>();

        //SkillTargetSort, From_SkillSO, To_TargetOption
        //chara
        builder.AddMessageBroker<Active_SingleCharaTarget>();
        builder.AddMessageBroker<Active_AllCharaTarget>();
        builder.AddMessageBroker<Active_FrontCharaTarget>();
        builder.AddMessageBroker<Active_BackCharaTarget>();

        //enemy
        builder.AddMessageBroker<Active_SingleEnemyTarget>();
        builder.AddMessageBroker<Active_AllEnemyTarget>();



    builder.AddMessageBroker<sbyte, RegistActiveSkill>();
        builder.AddMessageBroker<sbyte, RegistMoveSkillOnFront>();
        builder.AddMessageBroker<sbyte, RegistMoveSkillOnBack>();

        

        //BattleScene
        //To_BattleSceneCommand
        builder.AddMessageBroker<BattleSceneMessage.BattleStartMessage>();

        //From_BattleSceneCommand, To_effect,  To_CharaNameSimulator, To_EnemySimulator,
        builder.AddMessageBroker<BattleSceneMessage.BattlePrepareMessage>();
        //From_BattleSceneCommand, To_MSO_FormationCharaSO
        builder.AddMessageBroker<BattleSceneMessage.BattleFinishMessage>();

        builder.AddMessageBroker<BattleSceneMessage.TurnStartMessage>();

        builder.AddMessageBroker<sbyte, BattleSceneMessage.ActionSelectStartMessage>();
        builder.AddMessageBroker<BattleSceneMessage.ActionSelectEndMessage>();

        builder.AddMessageBroker<BattleSceneMessage.TurnEndMessage>();

        builder.AddMessageBroker<BattleSceneMessage.CharaAgilitySetMessage>();


        builder.AddMessageBroker<BattleSceneMessage.InputLayer>();

        builder.AddMessageBroker<BattleSceneMessage.InputLayerChanged>();
        builder.AddMessageBroker<BattleSceneMessage.InputLayer, BattleSceneMessage.InputLayerChanged>();

        builder.AddMessageBroker<BattleSceneMessage.SelectChange>();
        builder.AddMessageBroker<BattleSceneMessage.SelectMessage, BattleSceneMessage.SelectChange>();
        builder.AddMessageBroker<BattleSceneMessage.InputLayerChanged>();

        //zzzTester
        /*
        builder.AddMessageBroker<SortMessage>();
        builder.AddMessageBroker<SortValueMessage>();
        
        builder.AddMessageBroker<TestFA>();
        */

        // also exists AddMessageBroker<TKey, TMessage>, AddRequestHandler, AddAsyncRequestHandler

        // AddMessageHandlerFilter: Register for filter, also exists RegisterAsyncMessageHandlerFilter, Register(Async)RequestHandlerFilter
        //builder.AddMessageHandlerFilter<MyFilter<BattleStart>>();

        // create provider and set to Global(to enable diagnostics window and global fucntion)
        var provider = builder.BuildServiceProvider();
        GlobalMessagePipe.SetProvider(provider);
    }

}
