using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSO_BaseSkillTreeSO : MessageableScriptableObject
{
    [SerializeField]
    private string treeName;
    public List<MSO_SkillHolderSO> skillCatalog = new List<MSO_SkillHolderSO>();

    public  void RegistMasterySkill(int level, sbyte formNum)
    {
        if(level==0)
            return;
        //0-4, 0-4
        //=>level4‚Ì‚Æ‚«0-3
        for (int i = -1; i < level-1 ; i++)
        {
            skillCatalog[i+1].RegistThisSkill(formNum);
        }
    }
}
