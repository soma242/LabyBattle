using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillStruct;

using MessagePipe;

//ActiveSkillを増やした時
//ActiveSkillのSOを作成
//  Injecterに追加
//  ActiveSkillCommanderのリストに追加
//＊新しい分類なら新規に作成したPubに対応するSubをもつSoldierSOを作成
//  Injecter（を作成，Prepareリストに追加後）に追加
//＊新しい計算方法なら(新しいMessageに対応する）DamageCalcに計算方法を追加



public class MSO_ActiveSkillSO : ScriptableObject
{
    public int activeKey;

    public virtual void ActiveSkillBoot(ActiveSkillPosition acitvePos) { }

    public int GetSkillEffectKey()
    {
        return activeKey;
    }


}
