using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "skillHolder/enemyActive")]
public class EnemyActiveSkillHolderSO : ScriptableObject
{
    [SerializeField]
    protected string skillName;
    [SerializeField]
    protected string description;

    public ActiveSkillTimingSO skillTiming;


    public MSO_ActiveSkillSO skillEffectSO;
    public MSO_ActiveSkillTargetSO skillTarget;

    public string GetSkillName()
    {
        return skillName;
    }

    public int GetSkillEffectKey()
    {
        return skillEffectSO.GetSkillEffectKey();
    }
}