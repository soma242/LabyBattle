using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

[CreateAssetMenu(menuName = "skillEffect/moveSkill/normalOnBack")]
public class MSO_NormalMoveBackSO : MSO_MoveBackSkillSO
{
    public override void MoveSkillBoot(SkillStruct.MoveSkillPosition movePos) 
    {
        MoveToFront(movePos.user);
    }



    public override void MoveSkillSimulate(sbyte target) 
    {
        var simulatePub = GlobalMessagePipe.GetPublisher<MoveToFrontSimulate>();
        simulatePub.Publish(new MoveToFrontSimulate());
    }

}
