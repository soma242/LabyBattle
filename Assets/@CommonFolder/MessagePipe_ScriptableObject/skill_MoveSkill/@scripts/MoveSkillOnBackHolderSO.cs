using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/skillHolder/moveOnBack")]
public class MoveSkillOnBackHolderSO : MSO_SkillHolderSO
{
    //[SerializeField]
    public MSO_MoveBackSkillSO skillEffectSO;
    public MSO_MoveSkillTargetSO skillTarget;


    //private IPublisher<sbyte, RegistMoveSkillOnBack> registPub;


    public override void RegistThisSkill(sbyte formNum)
    {
        var registPub = GlobalMessagePipe.GetPublisher<sbyte, RegistMoveSkillOnBack>();
        if (registed)
            return;
        registed = true;
        disposable = registFinishSub.Subscribe(get =>
        {
            registed = false;
        });
        registPub.Publish(formNum, new RegistMoveSkillOnBack(this));
    }

    public virtual int GetSkillKey()
    {
        return skillEffectSO.moveKey;
    }
}
