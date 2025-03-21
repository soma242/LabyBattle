using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "skillList/Holder")]
public class SkillListHolderSO : ScriptableObject
{
    //[SerializeField]
    //private sbyte formNum;

    
    public List<MSO_ActiveSkillHolderSO> aSkillCatalog = new List<MSO_ActiveSkillHolderSO>();
    public List<MoveSkillOnFrontHolderSO> mFrontSkillCatalog = new List<MoveSkillOnFrontHolderSO>();
    public List<MoveSkillOnBackHolderSO> mBackSkillCatalog = new List<MoveSkillOnBackHolderSO>();

    //public virtual void RegistThisSkill() { }
}
