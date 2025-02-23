using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/activeSkill/normalAttack")]
public class MSO_NormalAttackSO : MSO_ActiveSkillSO
{

    public float activeRatio;

    private IPublisher<NormalAttack> activePublisher;


    //Method
    public override void MessageStart() {
        activePublisher = GlobalMessagePipe.GetPublisher<NormalAttack>();
    }

    public virtual void SetTargetSort() { }

    public override void ActiveSkillBoot(ActiveSkillPosition acitvePos) {
        activePublisher.Publish(new NormalAttack(acitvePos, activeRatio));
    }

}
