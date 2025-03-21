using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "moveSkill/skillTarget/NoneTarget")]
public class MSO_MoveNoneTargetSO : MSO_MoveSkillTargetSO
{


    public override void SetTargetSort() 
    {
        var targetPub = GlobalMessagePipe.GetPublisher<Move_NoneTarget>();
        var simuCancellPub = GlobalMessagePipe.GetPublisher<TauntSimulateCancell>();

        simuCancellPub.Publish(new TauntSimulateCancell());
        targetPub.Publish(new Move_NoneTarget());
        //Debug.Log(this.name);
    }

}
