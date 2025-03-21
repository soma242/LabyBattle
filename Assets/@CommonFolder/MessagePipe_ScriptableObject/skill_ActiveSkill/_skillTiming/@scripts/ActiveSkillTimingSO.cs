using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "activeSkill/skillTimig/common")]
public class ActiveSkillTimingSO : ScriptableObject
{
    public virtual void AcriveSkillBootBook()
    {
        var timingPub = GlobalMessagePipe.GetPublisher<CommonActiveSkillTiming>();
        //Debug.Log(this.name);
        timingPub.Publish(new CommonActiveSkillTiming());
    }
}
