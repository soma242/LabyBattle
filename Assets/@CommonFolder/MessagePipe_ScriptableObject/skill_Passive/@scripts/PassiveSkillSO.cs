using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillStruct;

using MessagePipe;





public class PassiveSkillSO : ScriptableObject
{

    //protected System.IDisposable disposableRegist;
    //protected System.IDisposable disposableStart;


    //regist���s���O��dispose���s�킹��B
    public virtual void RegistThisSkill(sbyte formNum) { }



}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillStruct;

using MessagePipe;
public class PassiveSkillOnBattleSO : PassiveSkillSO
{

    //protected System.IDisposable disposableRegist;


    //regist���s���O��dispose���s�킹��B
    public override void RegistThisSkill(sbyte formNum) 
    {
        var bag = DisposableBag.CreateBuilder();

        var unregistSub = 

        var passiveStartSub = GlobalMessagePipe.GetSubscriber<sbyte, PassiveSkillOnBattleSO>();

        passiveStartSub.Subscribe(get =>
        {

        }).AddTo(bag);

        
    }



}
*/

