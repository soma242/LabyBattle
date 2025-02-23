using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/activeSkill/skillTarget/SingleEnemy")]
public class MSO_SingleEnemyTargetSO : MSO_ActiveSkillTargetSO
{
    private IPublisher<Active_SingleEnemyTarget> targetPub;

    public override void MessageStart()
    {
        targetPub = GlobalMessagePipe.GetPublisher<Active_SingleEnemyTarget>();
    }

    public override void SetTargetSort() 
    {
        Debug.Log(this.name);
        targetPub.Publish(new Active_SingleEnemyTarget());
    }
}
