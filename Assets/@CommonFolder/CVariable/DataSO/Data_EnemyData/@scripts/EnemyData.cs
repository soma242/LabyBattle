using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using SkillStruct;

public class EnemyData : ScriptableObject
{
    //protected MSO_EnemyData thisComp;

    public EnemyImageSO enemyImage;

    public EnemySkillListHoderSO skillCatalog;

    public EnemyStateResistanceSO resist;

    [SerializeField]
    private string enemyName;


    // Status
    [SerializeField]
    private int HP;
    [SerializeField]
    private int attack;
    [SerializeField]
    private int magic; 
    [SerializeField]
    private int defence;
    [SerializeField]
    private int magicDefence; 
    [SerializeField]
    private int agility;

    //BattleŠJn‚ÉÀs
    //‘Ï«‚ª‚ ‚éê‡‚È‚Ç‚ÍŒp³æ‚Å‚»‚ÌSub‚ğ–³Œø‰»‚·‚é
    public virtual void SetStateSub(sbyte form, DisposableBagBuilder bag, Effects effects)
    {
        _ = new EnemyStateResistDetail(form, bag, effects, resist);

    }
    


    public virtual string GetEnemyName()
    {
        return enemyName;
    }

    public virtual int GetMaxHP()
    {
        return HP;
    }

    public virtual int GetAttack()
    {
        return attack;
    }

    public virtual int GetMagic()
    {
        return magic;
    }

    public virtual int GetDefence()
    {
        return defence;
    }

    public virtual int GetMagicDefence()
    {
        return magicDefence;
    }

    public virtual int GetAgility()
    {
        return agility;
    }

    public void SetEnemySkillTarget(int skillNum)
    {
        //Debug.Log("num"+skillNum);
        skillCatalog.activeCatalog[skillNum].skillHolderSO.skillTarget.SetEnemyTargetSort();
    }
    public void SetEnemySkillTiming(int skillNum)
    {
        //Debug.Log("num"+skillNum);
        skillCatalog.activeCatalog[skillNum].skillHolderSO.skillTiming.AcriveSkillBootBook();
    }

    public string GetSettedSkillName(int skillNum)
    {
        return skillCatalog.activeCatalog[skillNum].skillHolderSO.GetSkillName();

    }
    
    public int GetSettedSkillKey(int skillNum)
    {
        return skillCatalog.GetSkillEffectKey(skillNum);

    }
}
