using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "skillHolder/moveOnBack")]
public class MoveSkillOnBackHolderSO : MSO_SkillHolderSO
{
    [SerializeField]
    private MSO_MoveBackSkillSO skillEffectSO;

    private IPublisher<sbyte, RegistMoveSkillOnBack> registPub;

    public override void MessageStart()
    {
        base.MessageStart();
        registPub = GlobalMessagePipe.GetPublisher<sbyte, RegistMoveSkillOnBack>();
    }
    public override void RegistThisSkill(sbyte formNum)
    {
        if (registed)
            return;
        registed = true;
        disposable = registFinishSub.Subscribe(get =>
        {
            registed = false;
        });
        registPub.Publish(formNum, new RegistMoveSkillOnBack(this));
    }
}
