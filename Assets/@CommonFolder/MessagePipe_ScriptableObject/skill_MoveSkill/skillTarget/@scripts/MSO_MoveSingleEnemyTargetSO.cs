using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "moveSkill/skillTarget/singleEnemy")]
public class MSO_MoveSingleEnemyTargetSO : MSO_MoveSkillTargetSO
{


    public override void SetTargetSort()
    {
        var targetPub = GlobalMessagePipe.GetPublisher<Move_SingleEnemyTarget>();

        targetPub.Publish(new Move_SingleEnemyTarget());
        //Debug.Log(this.name);
    }

}

