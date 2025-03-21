using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

[CreateAssetMenu(menuName = "skillEffect/moveSkill/waitOnFront")]
public class MSO_WaitMoveFrontSO : MSO_MoveFrontSkillSO
{
    /*
    private IPublisher<MoveWaitSimulate> simulatePub;

    public override void MessageStart()
    {
        simulatePub = GlobalMessagePipe.GetPublisher<MoveWaitSimulate>();
    }
    */

    //public override void MoveSkillBoot(sbyte formNum) { }

    public override void MoveSkillSimulate(sbyte target)
    {
        var simulatePub = GlobalMessagePipe.GetPublisher<MoveWaitSimulate>();
        simulatePub.Publish(new MoveWaitSimulate());
    }
}
