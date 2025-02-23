using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "skillHolder/moveOnFront")]
public class MoveSkillOnFrontHolderSO : MSO_SkillHolderSO
{
    [SerializeField]
    private MSO_MoveFrontSkillSO skillEffectSO;

    private IPublisher<sbyte, RegistMoveSkillOnFront> registPub;

    public override void MessageStart()
    {
        base.MessageStart();
        registPub = GlobalMessagePipe.GetPublisher<sbyte, RegistMoveSkillOnFront>();
    }
    public override void RegistThisSkill(sbyte formNum)
    {

        if (registed)
        {
            return;
        }

        registed = true;
        disposable = registFinishSub.Subscribe(get =>
        {
            registed = false;
        });
        registPub.Publish(formNum, new RegistMoveSkillOnFront(this));
    }
}
