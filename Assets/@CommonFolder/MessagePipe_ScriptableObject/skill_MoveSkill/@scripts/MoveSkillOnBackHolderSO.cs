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


    public override void RegistThisSkill(sbyte formNum, DisposableBagBuilder bag)
    {
        var registPub = GlobalMessagePipe.GetPublisher<sbyte, RegistMoveSkillOnBack>();
        if (registed)
            return;
        registed = true;
        registFinishSub.Subscribe(get =>
        {
            registed = false;
        }).AddTo(bag);
        registPub.Publish(formNum, new RegistMoveSkillOnBack(this));
    }

    public virtual int GetSkillKey()
    {
        return skillEffectSO.moveKey;
    }
}
