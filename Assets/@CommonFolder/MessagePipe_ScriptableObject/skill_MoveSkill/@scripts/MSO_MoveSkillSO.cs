using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

public class MSO_MoveSkillSO : ScriptableObject
{
    public int moveKey;
    //public MSO_MoveSkillTargetSO skillTarget;


    public virtual void MoveSkillBoot(SkillStruct.MoveSkillPosition movePos) { }



    public virtual void MoveSkillSimulate(sbyte target)
    {

    }

    public void MoveToFront(sbyte user)
    {
        var movePub = GlobalMessagePipe.GetPublisher<sbyte, NormalMoveToFront>();
        movePub.Publish(user, new NormalMoveToFront());
    }
    
    public void MoveToBack(sbyte user)
    {
        var movePub = GlobalMessagePipe.GetPublisher<sbyte, NormalMoveToBack>();
        movePub.Publish(user, new NormalMoveToBack());
    }

    public void TauntApproach(MoveSkillPosition movePos)
    {
        var tauntPub = GlobalMessagePipe.GetPublisher<sbyte, TauntApproachMessage>();
        tauntPub.Publish(movePos.target, new TauntApproachMessage(movePos));
    }

    //public void 
}
