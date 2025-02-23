/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

using VContainer;
using VContainer.Unity;

using BattleSceneMessage;


#pragma warning disable CS4014 // disable warning

public struct TestFA { }

public class FirstAsyncTester : MonoBehaviour
{
    [SerializeField] protected InputLayerSO inputLayerSO;
    [Inject] protected readonly ISubscriber<InputLayerSO, UpInput> upSubscriber;

    private IAsyncPublisher<TestFA> testAPub;
    private IAsyncSubscriber<TestFA> testASub;
    private System.IDisposable disposable;
    private CancellationTokenSource cts;

    // Start is called before the first frame update
    void Awake()
    {
        testASub = GlobalMessagePipe.GetAsyncSubscriber<TestFA>();
        testAPub = GlobalMessagePipe.GetAsyncPublisher<TestFA>();

        cts = new CancellationTokenSource();
        disposable = upSubscriber.Subscribe(inputLayerSO, get =>
        {
            testAPub.Publish(new TestFA());
        });
    }

    // Update is called once per frame
    async UniTask Update()
    {
        await testASub.FirstAsync(cts.Token);
        Debug.Log("sucsess");
    }

    void OnDestroy()
    {
        cts.Cancel();
        disposable.Dispose();
    }
}
*/