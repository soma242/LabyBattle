using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

public class MSO_SkillHolderSO : MessageableScriptableObject
{
    [SerializeField]
    protected string skillName;
    [SerializeField]
    [TextArea]
    protected string description;

    protected bool registed = false;

    protected ISubscriber<RegistSkillFinish> registFinishSub;

    protected System.IDisposable disposable;


    public override void MessageStart()
    {
        registFinishSub = GlobalMessagePipe.GetSubscriber<RegistSkillFinish>();
        registed = false;
        //Debug.Log(this.name);
    }


    public virtual string GetSkillName() {
        return skillName;
    }

    public virtual string GetDescription()
    {
        return description;
    }

    public virtual void RegistThisSkill(sbyte formNum)
    {

    }
    


}
