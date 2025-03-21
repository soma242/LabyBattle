using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

using MessagePipe;
using SkillStruct;
using BattleSceneMessage;

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

    public Image hpCircle;

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
                    listNum++;
                    formNum++;
                    continue;
                }

                comp.nameText.SetText(formCommander.GetEnemyName(listNum));
                comp.changeTargetCanvas.enabled = false;




                listNum++;
                formNum++;
            }
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

        frontTargetSub.Subscribe(info =>
        {
            SimulateFrontChara(FormationScope.FormToListEnemy(info.enemyForm));

            var bagT = DisposableBag.CreateBuilder();
            approachSimuSub.Subscribe(info.enemyForm, info =>
            {
                SimulateTaunt(FormationScope.FormToListEnemy(info.enemyForm));
                simulatedNum = info.enemyForm;
                disposableCancell = simuCancellSub.Subscribe(get =>
                {
                    simulateComp[FormationScope.FormToListEnemy(simulatedNum)].changeTargetCanvas.enabled = false;
                });
            }).AddTo(bagT);
            disposableTaunt = bagT.Build();
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
