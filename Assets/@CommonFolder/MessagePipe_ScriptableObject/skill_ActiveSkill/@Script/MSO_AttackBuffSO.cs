using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillStruct;
using MessagePipe;

[CreateAssetMenu(menuName = "MessageableSO/Component/activeSkill/attackBuff")]
public class MSO_AttackBuffSO : MSO_ActiveSkillSO
{
    public float activeRatio;
    public int remain;

    public override void ActiveSkillBoot(ActiveSkillPosition activePos)
    {

        //‘ÎÛ‚Ìsbyte‚ğƒL[‚É ratio‚Æremain‚ğŠÜ‚ñ‚¾Message
        var activePub = GlobalMessagePipe.GetPublisher<sbyte, SkillStruct.AttackBuffMessage>();
        activePub.Publish(activePos.target, new AttackBuffMessage(activeRatio, remain));
    }
}
