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

        //�X�L���̔�������^�C�~���O�ɉ�����Sub����
        var passiveStartSub = GlobalMessagePipe.GetSubscriber<PassiveOnBattleStartMessage>();

        //�L�����N�^�[�̃X�L���o�^�O�ɏd���������ׂ�Pub
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

    //��L�Ƃقړ����Ȃ̂ő����b���p
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




    //regist���s���O��dispose���s�킹��B
    public override void RegistThisSkill(sbyte formNum)
    {


        UniTask.NextFrame();

        _ = new BreakPostureOnTauntComponent(formNum);


    }



}
