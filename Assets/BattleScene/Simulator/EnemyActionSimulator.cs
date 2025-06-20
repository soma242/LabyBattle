using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

using MessagePipe;
using SkillStruct;
using BattleSceneMessage;

using Cysharp.Threading.Tasks;

#pragma warning disable CS1998 // disable warning
#pragma warning disable CS4014 // disable warning


/// <summary>
/// 画面に表示するsimulate結果を管理するだけ    
/// </summary>


[System.Serializable]
public class EnemyActionSimulateComp
{
    public TMP_Text nameText;
    public TMP_Text skillText;
    public TMP_Text targetText;
    public TMP_Text changeTargetText;
    public Canvas changeTargetCanvas;
    public Canvas allCanvas;

    public Image hpCircle;




    public void ResetSimulater()
    {

        allCanvas.enabled = false;
    }
}

public class EnemyActionSimulator : MonoBehaviour
{

    [SerializeField]
    private List<EnemyActionSimulateComp> simulateComp = new List<EnemyActionSimulateComp>();

    [SerializeField]
    private MSO_FormationCommander formCommander;

    private string tauntUserName;
    //private sbyte tauntUserNum;

    private sbyte simulatedNum;


    private ISubscriber<SelectCharaNameMessage> selectorNameSub;

    private ISubscriber<sbyte, TauntApproachSimulate> approachSimuSub;

    private ISubscriber<TauntSimulateCancell> simuCancellSub;

    private IAsyncSubscriber<BattlePrepareMessage> prepareASub;

    private ISubscriber<EnemyTargetFrontSimulate> frontTargetSub;
    private ISubscriber<EnemyTargetSingleCharaSimulate> singleTargetSub;

    private ISubscriber<DamageNoticeMessage> damageSub;


    // private IPublisher<GetTauntUserName> tauntTargetPub;
    // private ISubscriber<ReturnTargetName> returnNameSub;

    private System.IDisposable disposableOnDestory;
    private System.IDisposable disposableTaunt;
    private System.IDisposable disposableCancell;

    void Awake()
    {

        selectorNameSub = GlobalMessagePipe.GetSubscriber<SelectCharaNameMessage>();

        approachSimuSub = GlobalMessagePipe.GetSubscriber<sbyte, TauntApproachSimulate>();
        simuCancellSub = GlobalMessagePipe.GetSubscriber<TauntSimulateCancell>();

        prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattlePrepareMessage>();

        frontTargetSub = GlobalMessagePipe.GetSubscriber<EnemyTargetFrontSimulate>();
        singleTargetSub = GlobalMessagePipe.GetSubscriber<EnemyTargetSingleCharaSimulate>();

        //tauntTargetPub = GlobalMessagePipe.GetPublisher<GetTauntUserName>();
        //returnNameSub = GlobalMessagePipe.GetSubscriber<ReturnTargetName>();

        damageSub = GlobalMessagePipe.GetSubscriber<DamageNoticeMessage>();

        var turnEndASub = GlobalMessagePipe.GetAsyncSubscriber<TurnEndMessage>();


        var dropEnemySub = GlobalMessagePipe.GetSubscriber<DropEnemyMessage>();

        var bag = DisposableBag.CreateBuilder();

        //バトル開始時，編成されているか確認後の初期化処理
        prepareASub.Subscribe(async (get, ct) =>
        {
            sbyte formNum = FormationScope.FirstEnemy();
            int listNum = 0;
            foreach(EnemyActionSimulateComp comp in simulateComp)
            {
                //Debug.Log("listNum"+FormationScope.FormToListEnemy(formNum));
                if (!formCommander.GetEnemyParticipant(listNum))
                {
                    comp.allCanvas.enabled = false;
                    listNum++;
                    formNum++;
                    continue;
                }

                comp.allCanvas.enabled = true ;
                comp.nameText.SetText(formCommander.GetEnemyName(listNum));
                comp.changeTargetCanvas.enabled = false;




                listNum++;
                formNum++;
            }
        }).AddTo(bag);


        dropEnemySub.Subscribe(get =>
        {
            int num = FormationScope.FormToListEnemy(get.pos);
            simulateComp[num].allCanvas.enabled = false;
        }).AddTo(bag);

        selectorNameSub.Subscribe(get =>
        {

            tauntUserName = get.selectorName;
            disposableCancell?.Dispose();
            //tauntUserNum = get.selectorNum;
            //DisableCanvas(FormationScope.FormToListEnemy(get.selectorNum));

        }).AddTo(bag);

        singleTargetSub.Subscribe(get =>
        {
            int listNum = FormationScope.FormToListEnemy(get.user);
            simulateComp[listNum].skillText.SetText(formCommander.GetSettedSkillName(listNum));
            simulateComp[listNum].targetText.SetText(formCommander.GetCharaName(FormationScope.FormToListChara(get.target)));
        }).AddTo(bag);

        //tauntが効く行動のEnemyからPubを受け取ってSubを開始する
        frontTargetSub.Subscribe(info =>
        {
            //Debug.Log("frontTarget" + info.enemyForm);
            //disposableTaunt?.Dispose();

            SimulateFrontChara(FormationScope.FormToListEnemy(info.enemyForm));

            var bagT = DisposableBag.CreateBuilder();
            approachSimuSub.Subscribe(info.enemyForm, async info =>
            {
                await UniTask.NextFrame();

                //Debug.Log("approach");
                SimulateTaunt(FormationScope.FormToListEnemy(info.enemyForm));
                simulatedNum = info.enemyForm;

                disposableCancell?.Dispose();
                var bagC = DisposableBag.CreateBuilder();

                //From_MoveTargetOption
                //ターゲット変更時にSub
                simuCancellSub.Subscribe(get =>
                {
                    //Debug.Log("cancel");
                    simulateComp[FormationScope.FormToListEnemy(simulatedNum)].changeTargetCanvas.enabled = false;
                    //simulateComp[FormationScope.FormToListEnemy(get.form)].changeTargetCanvas.enabled = true;
                    //simulatedNum = get.form;
                    //var simulatePub = GlobalMessagePipe.GetPublisher<sbyte, TauntApproachSimulate>();
                    //simulatePub.Publish(get.form, new TauntApproachSimulate(get.form));
                }).AddTo(bagC);

                var changeSub = GlobalMessagePipe.GetSubscriber<MoveOptionChange>();
                changeSub.Subscribe(info =>
                {
                    simulateComp[FormationScope.FormToListEnemy(simulatedNum)].changeTargetCanvas.enabled = false;
                    disposableCancell?.Dispose();
                }).AddTo(bagC);

                disposableCancell = bagC.Build();

            }).AddTo(bagT);

            if(disposableTaunt is not null)
            {
                var d = bagT.Build();
                //Debug.Log("combine");
                disposableTaunt = DisposableBag.Create(disposableTaunt, d); // combine disposable.
            }
            else
            {
                disposableTaunt = bagT.Build();
            }


            //disposableTaunt = bagT.Build();

        }).AddTo(bag);

        damageSub.Subscribe(get =>
        {
            if (get.chara)
            {
                return;
            }
            int i = FormationScope.FormToListEnemy(get.target);

            simulateComp[i].hpCircle.fillAmount = formCommander.GetEnemyRatioOnHP(i);
        }).AddTo(bag);

        turnEndASub.Subscribe(async(get,ct) =>
        {
            disposableTaunt?.Dispose();

            foreach (EnemyActionSimulateComp comp in simulateComp)
            {
                comp.changeTargetCanvas.enabled = false;
            }
        }).AddTo(bag);

        disposableOnDestory = bag.Build();

    }

    /*
    builder.AddMessageBroker<sbyte, GetNextTargetName>();
        builder.AddMessageBroker<sbyte, GetPreTargetName>();
    */

    void OnDestroy()
    {
        disposableOnDestory?.Dispose();
        disposableTaunt?.Dispose();
        disposableCancell?.Dispose();
    }

    private void SimulateFrontChara(int listNum)
    {
        simulateComp[listNum].skillText.SetText(formCommander.GetSettedSkillName(listNum));
        simulateComp[listNum].targetText.SetText(FormationScope.FrontCharaText());
    }
    


   

    private void SimulateTaunt(int listNum)
    {
        simulateComp[listNum].changeTargetText.SetText(tauntUserName);
        simulateComp[listNum].changeTargetCanvas.enabled = true;
    }



    private void DisableCanvas(int listNum)
    {
        if (simulateComp[listNum].changeTargetCanvas.enabled)
        {
            return;
        }
        simulateComp[listNum].changeTargetCanvas.enabled = false;
    }

    private void EnableCanvas(int listNum)
    {
        simulateComp[listNum].changeTargetCanvas.enabled = true;
    }

}
