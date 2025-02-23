using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MessageableSO/Component/Formation/Enemy")]
public class MSO_FormationEnemySO : MSO_FormationBaseMSO
{
    public MSO_EnemyData enemy;


    public Effects effects { get; private set; }


    public override void MessageStart()
    {
        effects = new Effects();
    }

}
