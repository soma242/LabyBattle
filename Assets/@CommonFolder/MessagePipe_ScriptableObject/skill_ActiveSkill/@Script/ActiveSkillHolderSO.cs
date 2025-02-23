using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "skillHolder/active")]
public class ActiveSkillHolderSO : MSO_SkillHolderSO
{
    
    //[SerializeField]
    public MSO_ActiveSkillSO skillEffectSO;

    private IPublisher<sbyte, RegistActiveSkill> registPub;

    public override void MessageStart()
    {
        base.MessageStart();
        registPub = GlobalMessagePipe.GetPublisher<sbyte, RegistActiveSkill>();
    }
    public override void RegistThisSkill(sbyte formNum)
    {
        if (registed)
        {
            return;
        }
        //Debug.Log(this.name);
        registed = true;
        disposable = registFinishSub.Subscribe(get =>
        {
            registed = false;
        });
        registPub.Publish(formNum, new RegistActiveSkill(this));
    }


}
