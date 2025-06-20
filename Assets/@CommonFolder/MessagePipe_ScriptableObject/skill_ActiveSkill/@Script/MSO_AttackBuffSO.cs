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

        //�Ώۂ�sbyte���L�[�� ratio��remain���܂�Message
        var activePub = GlobalMessagePipe.GetPublisher<sbyte, SkillStruct.AttackBuffMessage>();
        activePub.Publish(activePos.target, new AttackBuffMessage(activeRatio, remain));
    }
}
