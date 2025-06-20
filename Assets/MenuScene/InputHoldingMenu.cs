using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;


using System;
using System.Threading;
using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning

using BattleSceneMessage;



public class InputHoldingMenu : MonoBehaviour
{

    //Publisher(VContainer)
    private IPublisher<InputLayerSO, RightInput> rightPub;
    private IPublisher<InputLayerSO, LeftInput> leftPub;
    private IPublisher<InputLayerSO, UpInput> upPub;
    private IPublisher<InputLayerSO, DownInput> downPub;

    private IPublisher<InputLayerSO, EnterInput> enterPub;

    //Subscriber(Vcontainer)
    private ISubscriber<InputLayerSO, RightInput> rightSub;
    private ISubscriber<InputLayerSO, LeftInput> leftSub;
    private ISubscriber<InputLayerSO, UpInput> upSub;
    private ISubscriber<InputLayerSO, DownInput> downSub;

    private ISubscriber<InputLayerSO, EnterInput> enterSub;

    private ISubscriber<Holdout> holdoutSub;

    private System.IDisposable disposable;

    private CancellationTokenSource cts;

    private float waitTime = 0.2f;

    [SerializeField] private List<InputLayerSO> layerList = new List<InputLayerSO>();

    [SerializeField]
    private MSO_InputLayerHolder currentLayer;
    private InputLayerSO tempLayer;

    void Awake()
    {
        //Debug.Log(this.enabled);
        //currentLayer = GetComponent<InputLayerHolder>();


        rightPub = GlobalMessagePipe.GetPublisher<InputLayerSO, RightInput>();
        leftPub = GlobalMessagePipe.GetPublisher<InputLayerSO, LeftInput>();
        upPub = GlobalMessagePipe.GetPublisher<InputLayerSO, UpInput>();
        downPub = GlobalMessagePipe.GetPublisher<InputLayerSO, DownInput>();
        //enterPub = GlobalMessagePipe.GetPublisher<InputLayerSO, EnterInput>();

        rightSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, RightInput>();
        leftSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, LeftInput>();
        upSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, UpInput>();
        downSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, DownInput>();
        //enterSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, EnterInput>();


        holdoutSub = GlobalMessagePipe.GetSubscriber<Holdout>();

        //cts = new CancellationTokenSource();
        //token = cts.Token;

        var bag = DisposableBag.CreateBuilder();

        holdoutSub.Subscribe(i =>
        {
            cts?.Cancel();
        }).AddTo(bag);


        foreach (InputLayerSO layer in layerList)
        {
            rightSub.Subscribe(layer, i =>
            {
                //���̃^�C�~���O�ōs���Ǝn�܂��Ă����I����Ă��܂��̂őʖ�
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //cts��s�x�������Ȃ��Ɠ��ڈȍ~���������Ȃ��B

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
                        // ������CancellationToken�������Ȃ̂ŁA�����ێ�����OperationCanceledException�Ƃ��ē�����
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // �^�C���A�E�g�������Ȃ̂ŁATimeoutException(�����͓Ǝ��̗�O)�Ƃ��ē�����
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);

            leftSub.Subscribe(layer, i =>
            {
                //���̃^�C�~���O�ōs���Ǝn�܂��Ă����I����Ă��܂��̂őʖ�
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //cts��s�x�������Ȃ��Ɠ��ڈȍ~���������Ȃ��B
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
                        // ������CancellationToken�������Ȃ̂ŁA�����ێ�����OperationCanceledException�Ƃ��ē�����
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // �^�C���A�E�g�������Ȃ̂ŁATimeoutException(�����͓Ǝ��̗�O)�Ƃ��ē�����
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);


            upSub.Subscribe(layer, i =>
            {
                //���̃^�C�~���O�ōs���Ǝn�܂��Ă����I����Ă��܂��̂őʖ�
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //cts��s�x�������Ȃ��Ɠ��ڈȍ~���������Ȃ��B
                cts = new CancellationTokenSource();
                try
                {
                    //HoldInput�͌Ă΂�悤���Ȃ��낤��Select�̂ւ񂱂����O�x�s����͕̂ς��Ȃ��B
                    //Pub=>Sub�̍H����Pub�I���O��Sub�ǉ����Ă����ۂ��BSub������Ɩ��ɂȂ�Ȃ������̂�
                    //�i��ɑ��₵�����ƂŁjContainer�ɓ��͂��ꂽPub���܂������Ă���iSub�̏��������Ă���j��ԂɊԂɍ����ċ������ۂ��B
                    HoldInput(i, cts);

                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {

#if UNITY_EDITOR
                    if (cts.IsCancellationRequested)
                    {
                        // ������CancellationToken�������Ȃ̂ŁA�����ێ�����OperationCanceledException�Ƃ��ē�����
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // �^�C���A�E�g�������Ȃ̂ŁATimeoutException(�����͓Ǝ��̗�O)�Ƃ��ē�����
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);


            downSub.Subscribe(layer, i =>
            {
                //���̃^�C�~���O�ōs���Ǝn�܂��Ă����I����Ă��܂��̂őʖ�
                cts?.Cancel();

                tempLayer = currentLayer.inputLayerSO;

                //cts��s�x�������Ȃ��Ɠ��ڈȍ~���������Ȃ��B
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
                        // ������CancellationToken�������Ȃ̂ŁA�����ێ�����OperationCanceledException�Ƃ��ē�����
                        throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // �^�C���A�E�g�������Ȃ̂ŁATimeoutException(�����͓Ǝ��̗�O)�Ƃ��ē�����
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


                }
            }).AddTo(bag);
        }




        disposable = bag.Build();

    }

    public void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
        disposable?.Dispose();
    }



    //�󂯎����struct�ɉ�����Input��Ԃ�����

    async UniTask HoldInput(RightInput right, CancellationTokenSource cts)
    {
        for (int i = 0; i < 100; i++)
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
