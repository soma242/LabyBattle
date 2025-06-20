using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

public class DungeonInputSupporter : MonoBehaviour
{

    [SerializeField]
    private InputLayerSO dungeonLayer;

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        var layerPub = GlobalMessagePipe.GetPublisher<InputLayer>();
        layerPub.Publish(new InputLayer(dungeonLayer));

        var bag = DisposableBag.CreateBuilder();

        var menuInputSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, MenuInput>();
        menuInputSub.Subscribe(dungeonLayer, get =>
        {
            var menuPub = GlobalMessagePipe.GetPublisher<DungeonToMenuMessage>();
            menuPub.Publish(new DungeonToMenuMessage());
        }).AddTo(bag);

        //var cancelInputSub

        disposableOnDestroy = bag.Build();

    }
    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }


}
