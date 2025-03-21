using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/skillHolder/passive")]
public class MSO_PassiveSkillHolderSO : MSO_SkillHolderSO
{

    //[SerializeField]
    public PassiveSkillSO skillEffectSO;
    //public MSO_ActiveSkillTargetSO skillTarget;


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
        if (registed)
        {
            return;
        }
        registed = true;
        disposable = registFinishSub.Subscribe(get =>
        {
            registed = false;
        });
        skillEffectSO.RegistThisSkill(formNum);
    }


    /*
    public int GetSkillKey()
    {
        return skillEffectSO.activeKey.activeNum;
    }

    */

}