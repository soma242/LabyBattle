using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "activeSkill/skillTarget/FrontChara")]
public class MSO_AllFrontCharaTargetSO : MSO_ActiveSkillTargetSO
{
    private IPublisher<Active_FrontCharaTarget> targetPub;
    private IPublisher<EnemyTargetingAllFrontChara> enemyTargetPub;

    /*
    public override void MessageStart()
    {
        targetPub = GlobalMessagePipe.GetPublisher<Active_FrontCharaTarget>();
        enemyTargetPub = GlobalMessagePipe.GetPublisher<EnemyTargetingAllFrontChara>();
    }
    */

    public override void SetTargetSort()
    {
        //Debug.Log(this.name);
        var targetPub = GlobalMessagePipe.GetPublisher<Active_FrontCharaTarget>();
        targetPub.Publish(new Active_FrontCharaTarget());
    }

    public override void SetEnemyTargetSort()
    {
        var enemyTargetPub =  GlobalMessagePipe.GetPublisher<EnemyTargetingAllFrontChara>();
        enemyTargetPub.Publish(new EnemyTargetingAllFrontChara());
        Debug.Log(name);
    }
}
