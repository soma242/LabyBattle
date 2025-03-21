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

using BattleSceneMessage;



public class InputHolding_BattleScene : MonoBehaviour
{

    //Publisher(VContainer)
    [Inject] private readonly IPublisher<InputLayerSO, RightInput> rightPub;
    [Inject] private readonly IPublisher<InputLayerSO, LeftInput> leftPub;
    [Inject] private readonly IPublisher<InputLayerSO, UpInput> upPub;
    [Inject] private readonly IPublisher<InputLayerSO, DownInput> downPub;

    [Inject] private readonly IPublisher<InputLayerSO, EnterInput> enterPub;

    //Subscriber(Vcontainer)
    [Inject] private readonly ISubscriber<InputLayerSO, RightInput> rightSub;
    [Inject] private readonly ISubscriber<InputLayerSO, LeftInput> leftSub;
    [Inject] private readonly ISubscriber<InputLayerSO, UpInput> upSub;
    [Inject] private readonly ISubscriber<InputLayerSO, DownInput> downSub;

    [Inject] private readonly ISubscriber<InputLayerSO, EnterInput> enterSub;

    [Inject] private readonly ISubscriber<Holdout> holdoutSub;

    private System.IDisposable disposable;

    private CancellationTokenSource cts;

    private float waitTime = 0.2f;

    [SerializeField] private List<InputLayerSO> layerList = new List<InputLayerSO>();

    private CurrentInputLayerOfBattleScene currentLayer;
    private InputLayerSO tempLayer;

    void Awake()
    {
        currentLayer = GetComponent<CurrentInputLayerOfBattleScene>();

        //cts = new CancellationTokenSource();
        //token = cts.Token;

        var bag = DisposableBag.CreateBuilder();

        holdoutSub.Subscribe(i =>
        {
            cts.Cancel();
        }).AddTo(bag);


        foreach (InputLayerSO layer in layerList)
        {
            rightSub.Subscribe(layer, i =>
            {
                //このタイミングで行うと始まってすぐ終わってしまうので駄目
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //ctsを都度生成しないと二回目以降が発動しない。

                cts = new CancellationTokenSource();
                try
                {
                    HoldInput(i, cts);

                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {
#if UNITY_EDITOR
                    if (cts.IsCancellationRequested)
                    {
                        // 引数のCancellationTokenが原因なので、それを保持したOperationCanceledExceptionとして投げる
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // タイムアウトが原因なので、TimeoutException(或いは独自の例外)として投げる
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);

            leftSub.Subscribe(layer, i =>
            {
                //このタイミングで行うと始まってすぐ終わってしまうので駄目
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //ctsを都度生成しないと二回目以降が発動しない。
                cts = new CancellationTokenSource();
                try
                {
                    HoldInput(i, cts);

                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {
#if UNITY_EDITOR
                    if (cts.IsCancellationRequested)
                    {
                        // 引数のCancellationTokenが原因なので、それを保持したOperationCanceledExceptionとして投げる
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // タイムアウトが原因なので、TimeoutException(或いは独自の例外)として投げる
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);

            
            upSub.Subscribe(layer, i =>
            {
                //このタイミングで行うと始まってすぐ終わってしまうので駄目
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //ctsを都度生成しないと二回目以降が発動しない。
                cts = new CancellationTokenSource();
                try
                {
                    //HoldInputは呼ばれようがなかろうがSelectのへんこうが三度行われるのは変わらない。
                    //Pub=>Subの工程でPub終わる前にSub追加してたっぽい。Subが一つだと問題にならなかったので
                    //（二つに増やしたことで）Containerに入力されたPubがまだ生きている（Subの処理をしている）状態に間に合って居たっぽい。
                    HoldInput(i, cts);

                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {

#if UNITY_EDITOR
                    if (cts.IsCancellationRequested)
                    {
                        // 引数のCancellationTokenが原因なので、それを保持したOperationCanceledExceptionとして投げる
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // タイムアウトが原因なので、TimeoutException(或いは独自の例外)として投げる
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);
            

            downSub.Subscribe(layer, i =>
            {
                //このタイミングで行うと始まってすぐ終わってしまうので駄目
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //ctsを都度生成しないと二回目以降が発動しない。
                cts = new CancellationTokenSource();
                try
                {
                    HoldInput(i, cts);

                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {
#if UNITY_EDITOR
                    if (cts.IsCancellationRequested)
                    {
                        // 引数のCancellationTokenが原因なので、それを保持したOperationCanceledExceptionとして投げる
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // タイムアウトが原因なので、TimeoutException(或いは独自の例外)として投げる
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);
            enterSub.Subscribe(layer, i =>
            {
                //このタイミングで行うと始まってすぐ終わってしまうので駄目
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //ctsを都度生成しないと二回目以降が発動しない。
                cts = new CancellationTokenSource();
                try
                {
                    HoldInput(i, cts);

                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {
#if UNITY_EDITOR
                    if (cts.IsCancellationRequested)
                    {
                        // 引数のCancellationTokenが原因なので、それを保持したOperationCanceledExceptionとして投げる
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // タイムアウトが原因なので、TimeoutException(或いは独自の例外)として投げる
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);
        }

        


        disposable = bag.Build();

    }

    public void Destroy()
    {
        cts.Cancel();
        cts.Dispose();
        disposable.Dispose();
    }



    //受け取ったstructに応じたInputを返す処理

    async UniTask HoldInput(RightInput right, CancellationTokenSource cts)
    {
        for(int i = 0; i < 100; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: cts.Token);
            rightPub.Publish(tempLayer, right);
        }
        
    }

    async UniTask HoldInput(LeftInput left, CancellationTokenSource cts)
    {
        for (int i = 0; i < 100; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: cts.Token);
            leftPub.Publish(tempLayer, left);
        }

    }

    async UniTask HoldInput(UpInput up, CancellationTokenSource cts)
    {
        for (int i = 0; i < 100; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: cts.Token);
            upPub.Publish(tempLayer, up);
            //Debug.Log("UpPub");
        }

    }

    async UniTask HoldInput(DownInput down, CancellationTokenSource cts)
    {
        for (int i = 0; i < 100; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: cts.Token);
            downPub.Publish(tempLayer, down);
        }

    }
    async UniTask HoldInput(EnterInput enter, CancellationTokenSource cts)
    {
        for (int i = 0; i < 100; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: cts.Token);
            enterPub.Publish(tempLayer, enter);
        }

    }

    

}
