using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;
using BattleSceneMessage;

[CreateAssetMenu(menuName = "MessageHolder/formation")]
public class MSO_FormationMessageHolderSO : MessageableScriptableObject
{
    public IPublisher<BuffNoticeMessage> buffNoticePub;

    public ISubscriber<sbyte, AttackBuffMessage> attackBuffSub;

    public IPublisher<sbyte, ActionSelectCancelPerfomeMessage> cancelPerfomePub;
    public ISubscriber<sbyte, ActionSelectCancelPerfomeMessage> cancelPerfomeSub;


    public override void MessageStart()
    {
        buffNoticePub = GlobalMessagePipe.GetPublisher<BuffNoticeMessage>();

        attackBuffSub = GlobalMessagePipe.GetSubscriber<sbyte, AttackBuffMessage>();

        cancelPerfomePub = GlobalMessagePipe.GetPublisher<sbyte, ActionSelectCancelPerfomeMessage>();
        cancelPerfomeSub = GlobalMessagePipe.GetSubscriber<sbyte, ActionSelectCancelPerfomeMessage>();

    }
}
