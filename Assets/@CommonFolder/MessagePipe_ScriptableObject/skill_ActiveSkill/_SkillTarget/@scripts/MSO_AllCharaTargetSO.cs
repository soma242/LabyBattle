
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "activeSkill/skillTarget/allChara")]
public class MSO_AllCharaTargetSO : MSO_ActiveSkillTargetSO
{


    /*
    public override void MessageStart()
    {
        targetPub = GlobalMessagePipe.GetPublisher<Active_AllCharaTarget>();
        enemyTargetPub = GlobalMessagePipe.GetPublisher<EnemyTargetingAllFrontChara>();
    }
    */

    public override void SetTargetSort()
    {
        //Debug.Log(this.name);
        var targetPub = GlobalMessagePipe.GetPublisher<Active_AllCharaTarget>();
        targetPub.Publish(new Active_AllCharaTarget());
    }

    public override void SetEnemyTargetSort()
    {
        var enemyTargetPub = GlobalMessagePipe.GetPublisher<EnemyTargetingAllChara>();
        enemyTargetPub.Publish(new EnemyTargetingAllChara());
        //Debug.Log(name);
    }
}
