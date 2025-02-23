using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

using BattleSceneMessage;



//publishの集約

public class BattleSceneCommand : MonoBehaviour
{
    /*

    //[Inject] private readonly ISubscriber<BattleStartMessage> startSubscriber;
    [Inject] private readonly IPublisher<BattleOptionSelectStart> optionSelectStartPublisher;


    [Inject] private readonly IPublisher<BattleOptionSelecting> optionSelectihgPublisher;
    [Inject] private readonly ISubscriber<BattleOptionSelected> optionSelectedSubscriber;

    [Inject] private readonly IPublisher<BattleProcessStart> processStart;
    */

    private CancellationTokenSource cts;

    
    private sbyte selectStartForm = 1;
    private sbyte selectFinish_NextForm = 5;

    private ISubscriber<BattleStartMessage> startSub;
    private IAsyncPublisher<BattlePrepareMessage> prepareAPub;

    private IAsyncPublisher<TurnStartMessage> turnStartAPub;

    private IPublisher<sbyte, ActionSelectStartMessage> selectStartPub;
    private ISubscriber<sbyte, ActionSelectStartMessage> lastSelectStartSub;
    private IAsyncPublisher<ActionSelectEndMessage> selectEndAPub;
    private IAsyncSubscriber<ActionSelectEndMessage> selectEndASub;

    private IAsyncPublisher<TurnEndMessage> turnEndAPub;

    private System.IDisposable disposable;


    void Awake()
    {
        cts = new CancellationTokenSource();
        startSub = GlobalMessagePipe.GetSubscriber<BattleStartMessage>();
        prepareAPub = GlobalMessagePipe.GetAsyncPublisher<BattlePrepareMessage>();

        turnStartAPub = GlobalMessagePipe.GetAsyncPublisher<TurnStartMessage>();

        selectStartPub = GlobalMessagePipe.GetPublisher<sbyte, ActionSelectStartMessage>();
        lastSelectStartSub = GlobalMessagePipe.GetSubscriber<sbyte, ActionSelectStartMessage>();

        selectEndAPub = GlobalMessagePipe.GetAsyncPublisher<ActionSelectEndMessage>();
        selectEndASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectEndMessage>();

        turnEndAPub = GlobalMessagePipe.GetAsyncPublisher<TurnEndMessage>();


        var bag = DisposableBag.CreateBuilder();
        //BattleStartを受け取って，コマンドを開始する
        startSub.Subscribe(get =>
        {

            BattleCommand(cts.Token);

        }).AddTo(bag);

        //さいごの行動選択が終了したら受け取り，次のステップに進む
        lastSelectStartSub.Subscribe(selectFinish_NextForm, get =>
        {
            selectEndAPub.Publish(new ActionSelectEndMessage());
        }).AddTo(bag);

        disposable = bag.Build();
    }

    private async UniTask BattleCommand(CancellationToken ct)
    {
        await prepareAPub.PublishAsync(new BattlePrepareMessage());

        //以降繰り返し処理。

        //ターン開始
        await turnStartAPub.PublishAsync(new TurnStartMessage());

        for (sbyte i = selectStartForm; i < selectFinish_NextForm; i++)
        {
            //行動選択者の変更
            selectStartPub.Publish(i, new ActionSelectStartMessage());

            //何度も起動することは確かめた。
            //Destory時のError: NullReferenceException: Object reference not set to an instance of an object
            //GetAsyncSubし忘れ
            var ev = await selectEndASub.FirstAsync(ct);
        }
        

        //ターン終了
        await turnEndAPub.PublishAsync(new TurnEndMessage());
    }


    






    private void OnDestroy()
    {
        cts.Cancel();
        disposable.Dispose();
    }


}
