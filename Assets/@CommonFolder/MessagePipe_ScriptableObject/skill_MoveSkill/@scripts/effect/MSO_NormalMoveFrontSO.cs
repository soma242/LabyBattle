using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

[CreateAssetMenu(menuName = "skillEffect/moveSkill/normalOnFront")]
public class MSO_NormalMoveFrontSO : MSO_MoveFrontSkillSO
{
    public override void MoveSkillBoot(SkillStruct.MoveSkillPosition movePos)
    {
        MoveToBack(movePos.user);
    }

    public override void MoveSkillSimulate(sbyte target)
    {
        var simulatePub = GlobalMessagePipe.GetPublisher<MoveToBackSimulate>();
        simulatePub.Publish(new MoveToBackSimulate());
    }

}
