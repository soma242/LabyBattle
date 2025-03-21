using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "activeSkill/skillTarget/SingleChara")]
public class MSO_SingleCharaTargetSO : MSO_ActiveSkillTargetSO
{
    private IPublisher<Active_SingleCharaTarget> targetPub;



    public override void SetTargetSort()
    {
        var targetPub = GlobalMessagePipe.GetPublisher<Active_SingleCharaTarget>();
        targetPub.Publish(new Active_SingleCharaTarget());
    }

    public override void SetEnemyTargetSort()
    {
        var enemyTargetPub = GlobalMessagePipe.GetPublisher<EnemyTargetingSingleChara>();
        enemyTargetPub.Publish(new EnemyTargetingSingleChara());

        Debug.Log(name);
    }
}
