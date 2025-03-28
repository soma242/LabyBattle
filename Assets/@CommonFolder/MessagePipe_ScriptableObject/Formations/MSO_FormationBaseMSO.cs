using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;
using BattleSceneMessage;

public class MSO_FormationBaseMSO : MessageableScriptableObject
{
    public FormationEnumSO formNumSO;

    public bool participant;

    protected int currentHP;

    //commonMessage
    protected ISubscriber<sbyte, NormalDamageCalcMessage> normalDamageSub;
    protected ISubscriber<sbyte, NormalMagicDamageCalcMessage> normalMagicDamageSub;
    protected IPublisher<DamageNoticeMessage> damageNoticePub;


    protected void CommonSubRegist()
    {
        normalDamageSub = GlobalMessagePipe.GetSubscriber<sbyte, NormalDamageCalcMessage>();
        normalMagicDamageSub = GlobalMessagePipe.GetSubscriber<sbyte, NormalMagicDamageCalcMessage>();
        damageNoticePub = GlobalMessagePipe.GetPublisher<DamageNoticeMessage>();

    }

    public sbyte GetFormNum()
    {
        return formNumSO.formationNum;
    }

    protected sbyte NextForm(sbyte form)
    {
        form++;
        return form;
    }
    protected sbyte PreForm(sbyte form)
    {
        form--;
        return form;
    }


}
