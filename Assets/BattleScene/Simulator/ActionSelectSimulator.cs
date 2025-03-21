using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MessagePipe;
using BattleSceneMessage;
using SkillStruct;

using TMPro;

[System.Serializable]
public class ActionSimulateResultComp
{
    public TMP_Text skillText;
    public TMP_Text targetText;
    public Image hpCircle;
}

public class ActionSelectSimulator : MonoBehaviour
{

    [SerializeField]
    private List<ActionSimulateResultComp> simulateUI = new List<ActionSimulateResultComp>();

    [SerializeField]
    private MSO_FormationCommander formCommander;

    private int listNum;



    //private IAsyncSubscriber<TurnEndMessage> turnEndASub;

    private ISubscriber<sbyte, ActionSelectStartMessage> selectorChangeSub;
    private System.IDisposable disposableOnDestroy;

    private ISubscriber<DamageNoticeMessage> damageSub;



    private ISubscriber<ASkillSimulateMessage> skillNameSub;
    private ISubscriber<ReturnTargetName> returnTargetSub;

    private System.IDisposable disposableCancellOnSecond;

    void Awake()
    {
        var bag = DisposableBag.CreateBuilder();

        //turnEndASub = GlobalMessagePipe.GetAsyncSubscriber<TurnEndMessage>();

        selectorChangeSub = GlobalMessagePipe.GetSubscriber<sbyte, ActionSelectStartMessage>();

        damageSub = GlobalMessagePipe.GetSubscriber<DamageNoticeMessage>();

        skillNameSub = GlobalMessagePipe.GetSubscriber<ASkillSimulateMessage>();
        returnTargetSub = GlobalMessagePipe.GetSubscriber<ReturnTargetName>();

        /*
        turnEndASub.Subscribe(async (get, ct) =>
        {


        }).AddTo(bag);
        */

        for(sbyte TKey = FormationScope.FirstChara(); TKey <= FormationScope.LastChara(); TKey++)
        {
            selectorChangeSub.Subscribe(TKey, get =>
            {
                listNum = FormationScope.FormToListChara(get.charaForm);
                //Debug.Log(listNum);

                


            }).AddTo(bag);
        }

        damageSub.Subscribe(get =>
        {
            if (!get.chara)
            {
                return;
            }
            int i = FormationScope.FormToListChara(get.target);
            //Debug.Log(formCommander.GetCharaRatioOnHP(i));
            simulateUI[i].hpCircle.fillAmount = formCommander.GetCharaRatioOnHP(i);
        }).AddTo(bag);

        skillNameSub.Subscribe(get =>
        {
            simulateUI[listNum].skillText.SetText(get.skillName);
        }).AddTo(bag);

        returnTargetSub.Subscribe(get =>
        {
            simulateUI[listNum].targetText.SetText(get.targetName);

        }).AddTo(bag);






        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy.Dispose();
    }



}
