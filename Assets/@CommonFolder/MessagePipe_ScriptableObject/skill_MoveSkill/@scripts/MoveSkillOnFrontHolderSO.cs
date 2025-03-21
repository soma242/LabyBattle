using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/skillHolder/moveOnFront")]
public class MoveSkillOnFrontHolderSO : MSO_SkillHolderSO
{
 //   [SerializeField]
    public MSO_MoveFrontSkillSO skillEffectSO;
    public MSO_MoveSkillTargetSO skillTarget;


    //private IPublisher<sbyte, RegistMoveSkillOnFront> registPub;


    public override void RegistThisSkill(sbyte formNum)
    {
        var registPub = GlobalMessagePipe.GetPublisher<sbyte, RegistMoveSkillOnFront>();

        if (registed)
        {
            return;
        }

        registed = true;
        disposable = registFinishSub.Subscribe(get =>
        {
            registed = false;
        });
        registPub.Publish(formNum, new RegistMoveSkillOnFront(this));
    }


    public virtual int GetSkillKey()
    {
        return skillEffectSO.moveKey;
    }
}
