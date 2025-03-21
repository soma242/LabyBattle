using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using SkillStruct;

[CreateAssetMenu(menuName = "skillEffect/activeSkill/normalMagic")]
public class MSO_NormalMagicSO : MSO_ActiveSkillSO
{

    public float activeRatio;

    /*
    private IPublisher<NormalMagic> activePublisher;


    //Method
    public override void MessageStart()
    {
        activePublisher = GlobalMessagePipe.GetPublisher<NormalMagic>();
    }
    */

    //public virtual void SetTargetSort() { }

    public override void ActiveSkillBoot(ActiveSkillPosition acitvePos)
    {
        var activePub  = GlobalMessagePipe.GetPublisher<NormalMagic>();
        activePub.Publish(new NormalMagic(acitvePos, activeRatio));
    }

}

