using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/skillHolder/active")]
public class MSO_ActiveSkillHolderSO : MSO_SkillHolderSO
{
    public ActiveSkillTimingSO skillTiming;
    //[SerializeField]
    public MSO_ActiveSkillSO skillEffectSO;
    public MSO_ActiveSkillTargetSO skillTarget;


    //private IPublisher<sbyte, RegistActiveSkill> registPub;

    /*
    public override void MessageStart()
    {
        base.MessageStart();
        registPub = 
    }
    */

    public override void RegistThisSkill(sbyte formNum)
    {
        var registPub = GlobalMessagePipe.GetPublisher<sbyte, RegistActiveSkill>();
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

    public int GetSkillKey()
    {
        return skillEffectSO.activeKey;
    }

    public void AcriveSkillBootBook()
    {
        skillTiming.AcriveSkillBootBook();
    }


}
