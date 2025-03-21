using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using Cysharp.Threading.Tasks;
using System.Threading;
#pragma warning disable CS1998 // disable warning
#pragma warning disable CS4014 // disable warning


using BattleSceneMessage;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/ScopeSupporter")]
public class MSP_ScopeOutSupporterSO : MessageableScriptableObject
{
    private ISubscriber<sbyte, GetNextTargetName> nextTargetSub;
    private IPublisher<sbyte, GetNextTargetName> nextTargetPub;
    private ISubscriber<sbyte, GetPreTargetName> preTargetSub;
    private IPublisher<sbyte, GetPreTargetName> preTargetPub;

    /*
    private ISubscriber<sbyte, NormalDamageCalcMessage> normalDamageSub;
    private IPublisher<sbyte, NormalDamageCalcMessage> normalDamagePub;
    */




    private System.IDisposable disposable;

    public override void MessageStart()
    {

        nextTargetSub = GlobalMessagePipe.GetSubscriber<sbyte, GetNextTargetName>();
        nextTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetNextTargetName>();
        preTargetSub = GlobalMessagePipe.GetSubscriber<sbyte, GetPreTargetName>();
        preTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetPreTargetName>();

        /*
        normalDamageSub = GlobalMessagePipe.GetSubscriber<sbyte, NormalDamageCalcMessage>();
        normalDamagePub = GlobalMessagePipe.GetPublisher<sbyte, NormalDamageCalcMessage>();
        */

        var bag = DisposableBag.CreateBuilder();

        //BattleOption‚Ìƒ^[ƒQƒbƒg‘I‘ð
        //enemyTarget
        nextTargetSub.Subscribe(SbyteHandler.NextForm(FormationScope.LastEnemy()), get =>
        {
            nextTargetPub.Publish(FormationScope.FirstEnemy(), new GetNextTargetName());
        }).AddTo(bag);

        preTargetSub.Subscribe(SbyteHandler.PreForm(FormationScope.FirstEnemy()), get =>
        {
            preTargetPub.Publish(FormationScope.LastEnemy(), new GetPreTargetName());
        }).AddTo(bag);


        //charaTarget
        nextTargetSub.Subscribe(SbyteHandler.NextForm(FormationScope.LastChara()), get =>
        {
            nextTargetPub.Publish(FormationScope.FirstChara(), new GetNextTargetName());
        }).AddTo(bag);

        preTargetSub.Subscribe(SbyteHandler.PreForm(FormationScope.FirstChara()), get =>
        {
            preTargetPub.Publish(FormationScope.LastChara(), new GetPreTargetName());
        }).AddTo(bag);


        //‘O‰q‚Ö‚Ì”ÍˆÍUŒ‚
        /*
        normalDamageSub.Subscribe(FormationScope.FrontChara(), info =>
        {
            normalDamagePub.Publish(, info);
        }).AddTo(bag);
        */

        disposable = bag.Build();
    }


}
