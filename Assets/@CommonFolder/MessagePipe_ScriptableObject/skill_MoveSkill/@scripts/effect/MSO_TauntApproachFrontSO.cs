using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

[CreateAssetMenu(menuName = "skillEffect/moveSkill/tauntApproach")]
public class MSO_TauntApproachFrontSO : MSO_MoveFrontSkillSO
{

    private System.IDisposable disposable;
    public override void MoveSkillBoot(SkillStruct.MoveSkillPosition movePos)
    {
        TauntApproach(movePos);
    }

    public override void MoveSkillSimulate(sbyte target)
    {
        var simulatePub = GlobalMessagePipe.GetPublisher<sbyte, TauntApproachSimulate>();
        simulatePub.Publish(target, new TauntApproachSimulate(target));
        var waitPub = GlobalMessagePipe.GetPublisher<MoveWaitSimulate>();
        waitPub.Publish(new MoveWaitSimulate());

    }

}