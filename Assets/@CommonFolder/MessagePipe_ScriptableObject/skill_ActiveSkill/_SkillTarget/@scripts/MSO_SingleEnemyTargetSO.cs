using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "activeSkill/skillTarget/SingleEnemy")]
public class MSO_SingleEnemyTargetSO : MSO_ActiveSkillTargetSO
{
    private IPublisher<Active_SingleEnemyTarget> targetPub;



    public override void SetTargetSort() 
    {
        var targetPub = GlobalMessagePipe.GetPublisher<Active_SingleEnemyTarget>();
        targetPub.Publish(new Active_SingleEnemyTarget());
    }
}
