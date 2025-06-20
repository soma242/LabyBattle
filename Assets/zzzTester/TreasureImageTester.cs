using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MessagePipe;
using DungeonSceneMessage;

using Cysharp.Threading.Tasks;


public struct TestMessage { }

public class TreasureImageTester : MonoBehaviour
{
    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableUndraw;

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();

        var drawSub = GlobalMessagePipe.GetSubscriber<TestMessage>();
        disposableOnDestroy = drawSub.Subscribe(get =>
        {
            image.enabled = true;
            
            //disposableOnDestroy?.Dispose();
            var bag = DisposableBag.CreateBuilder();
            var posSub = GlobalMessagePipe.GetSubscriber<PosChangeMessage>();
            var direSub = GlobalMessagePipe.GetSubscriber<RotateDirectionMessage>();

            UniTask.NextFrame();

            posSub.Subscribe(get =>
            {
                image.enabled = false;
                disposableOnDestroy?.Dispose();
            }).AddTo(bag);
            direSub.Subscribe(get =>
            {
                image.enabled = false;
                disposableOnDestroy?.Dispose();
            }).AddTo(bag);

            disposableUndraw = bag.Build();
        });
    }
    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        disposableUndraw?.Dispose();
    }
}
