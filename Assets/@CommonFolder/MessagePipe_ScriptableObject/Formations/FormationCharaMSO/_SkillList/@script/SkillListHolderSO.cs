using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "skillList/Holder")]
public class SkillListHolderSO : ScriptableObject
{
    //[SerializeField]
    //private sbyte formNum;

    
    public List<ActiveSkillHolderSO> aSkillCatalog = new List<ActiveSkillHolderSO>();
    public List<MoveSkillOnFrontHolderSO> mFrontSkillCatalog = new List<MoveSkillOnFrontHolderSO>();
    public List<MoveSkillOnBackHolderSO> mBackSkillCatalog = new List<MoveSkillOnBackHolderSO>();

    public virtual void RegistThisSkill() { }
}
