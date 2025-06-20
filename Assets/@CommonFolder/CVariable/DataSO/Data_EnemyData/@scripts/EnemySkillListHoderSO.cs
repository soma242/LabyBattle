using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyActiveSkill
{
    public EnemyActiveSkillHolderSO skillHolderSO;
    public int skillWeight = 10;
}

[CreateAssetMenu(menuName = "skillList/enemyHolder")]
public class EnemySkillListHoderSO : ScriptableObject
{
    public List<EnemyActiveSkill> activeCatalog = new List<EnemyActiveSkill>();

    public int GetTotalSkillWeight()
    {
        int i = 0;
        foreach (EnemyActiveSkill skill in activeCatalog)
        {
            i += skill.skillWeight;
        }
        return i;

    }

    public int GetSelectSkill(int rand)
    {
        int value = 0;
        for(int i = 0; i< activeCatalog.Count; i++)
        {
            //Debug.Log(activeCatalog.Count);
            //Debug.Log(i);
            rand -= activeCatalog[i].skillWeight; 
            if(rand < 1)
            {
                value = i;
                break;
            }
        }
        return value;
    }

    public int GetSkillEffectKey(int value)
    {
        return activeCatalog[value].skillHolderSO.GetSkillEffectKey();
    }

}
