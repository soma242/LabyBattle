using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using SkillStruct;

[CreateAssetMenu(menuName = "skillEffect/activeSkill/normalAttack")]
public class MSO_NormalAttackSO : MSO_ActiveSkillSO
{

    public float activeRatio;

    //private IPublisher<NormalAttack> activePublisher;


    //Method
    /*
    public override void MessageStart() {
        activePublisher = GlobalMessagePipe.GetPublisher<NormalAttack>();
    }
    */

    //public virtual void SetTargetSort() { }

    public override void ActiveSkillBoot(ActiveSkillPosition acitvePos) {
        var activePub  = GlobalMessagePipe.GetPublisher<NormalAttack>();
        activePub.Publish(new NormalAttack(acitvePos, activeRatio));
    }

}
