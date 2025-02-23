using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MSO_EnemyData : MessageableScriptableObject
{
    protected MSO_EnemyData thisComp;

    public EnemyImageSO enemyImage;

    // Status
    [SerializeField]
    private int HP;
    [SerializeField]
    private int attack; //{ get; private set; }
    [SerializeField]
    private int agility;

    public override void MessageStart()
    {
        thisComp = this;
        //Debug.Log(this.name);
    }

    public virtual int GetAttack()
    {
        return attack;
    }

    public virtual int GetAgility()
    {
        return agility;
    }
}
