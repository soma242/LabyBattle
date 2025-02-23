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



//publish�̏W��

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
        //BattleStart���󂯎���āC�R�}���h���J�n����
        startSub.Subscribe(get =>
        {

            BattleCommand(cts.Token);

        }).AddTo(bag);

        //�������̍s���I�����I��������󂯎��C���̃X�e�b�v�ɐi��
        lastSelectStartSub.Subscribe(selectFinish_NextForm, get =>
        {
            selectEndAPub.Publish(new ActionSelectEndMessage());
        }).AddTo(bag);

        disposable = bag.Build();
    }

    private async UniTask BattleCommand(CancellationToken ct)
    {
        await prepareAPub.PublishAsync(new BattlePrepareMessage());

        //�ȍ~�J��Ԃ������B

        //�^�[���J�n
        await turnStartAPub.PublishAsync(new TurnStartMessage());

        for (sbyte i = selectStartForm; i < selectFinish_NextForm; i++)
        {
            //�s���I���҂̕ύX
            selectStartPub.Publish(i, new ActionSelectStartMessage());

            //���x���N�����邱�Ƃ͊m���߂��B
            //Destory����Error: NullReferenceException: Object reference not set to an instance of an object
            //GetAsyncSub���Y��
            var ev = await selectEndASub.FirstAsync(ct);
        }
        

        //�^�[���I��
        await turnEndAPub.PublishAsync(new TurnEndMessage());
    }


    






    private void OnDestroy()
    {
        cts.Cancel();
        disposable.Dispose();
    }


}
