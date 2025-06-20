using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillStruct;
using BattleSceneMessage;

using MessagePipe;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

public class BreakPostureOnTauntComponent
{
    private System.IDisposable disposableRegist;
    private System.IDisposable disposableBoot;

    private sbyte formNum;

    public BreakPostureOnTauntComponent(sbyte formNum)
    {
        this.formNum = formNum;
        //Debug.Log("construct");
        //temp();

        var bag = DisposableBag.CreateBuilder();

        var unregistSub = GlobalMessagePipe.GetSubscriber<sbyte, UnregistPassiveSkill>();

        //スキルの発動するタイミングに応じたSubを代入
        var passiveStartSub = GlobalMessagePipe.GetSubscriber<PassiveOnBattleStartMessage>();

        //キャラクターのスキル登録前に重複を避ける為にPub
        unregistSub.Subscribe(formNum, get =>
        {
            disposableRegist?.Dispose();
            //Debug.Log("dispo");
        }).AddTo(bag);

        passiveStartSub.Subscribe(get =>
        {
            disposableBoot?.Dispose();
            var tauntSuccessSub = GlobalMessagePipe.GetSubscriber<sbyte, TauntSuccessMessage>();
            var endSub = GlobalMessagePipe.GetSubscriber<BattleFinishMessage>();

            var bag = DisposableBag.CreateBuilder();

            tauntSuccessSub.Subscribe(formNum, get =>
            {
                //Debug.Log(formNum);
                var breakePub = GlobalMessagePipe.GetPublisher<sbyte, BreakePostureMessage>();
                breakePub.Publish(get.target, new BreakePostureMessage());
            }).AddTo(bag);


            endSub.Subscribe(get =>
            {
                disposableBoot?.Dispose();
            }).AddTo(bag);

            disposableBoot = bag.Build();
            //Debug.Log("subing");
        }).AddTo(bag);


        disposableRegist = bag.Build();
    }

    /*
     ~BreakPostureOnTauntComponent()
    {
        //Debug.Log("finalize");
    }
    */

    //上記とほぼ同じなので多分暫時用
    /*
    private void temp()
    {
        var tauntSuccessSub = GlobalMessagePipe.GetSubscriber<sbyte, TauntSuccessMessage>();
        var endSub = GlobalMessagePipe.GetSubscriber<BattleFinishMessage>();

        var bag = DisposableBag.CreateBuilder();

        tauntSuccessSub.Subscribe(formNum, get =>
        {
            //Debug.Log(formNum);
            var breakePub = GlobalMessagePipe.GetPublisher<sbyte, BreakePostureMessage>();
            breakePub.Publish(get.target, new BreakePostureMessage());
        }).AddTo(bag);


        endSub.Subscribe(get =>
        {
            disposableBoot?.Dispose();
        }).AddTo(bag);

        disposableBoot = bag.Build();
        //Debug.Log("subing");
    }
    */
}


[CreateAssetMenu(menuName = "skillEffect/passiveSkill/BreakPostureOnTaunt")]
public class BreakPostureOnTaunt : PassiveSkillSO
{




    //registを行う前にdisposeを行わせる。
    public override void RegistThisSkill(sbyte formNum)
    {


        UniTask.NextFrame();

        _ = new BreakPostureOnTauntComponent(formNum);


    }



}
