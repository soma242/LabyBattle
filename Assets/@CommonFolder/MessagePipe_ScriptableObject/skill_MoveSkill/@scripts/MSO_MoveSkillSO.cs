using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MSO_MoveSkillSO : MessageableScriptableObject
{
    public MoveSkillEnumSO moveKey;


    public virtual void MoveSkillBoot(sbyte form) { }
    public virtual void MoveSkillBoot(sbyte form, sbyte target) { }
}
