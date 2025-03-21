using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

#pragma warning disable CS1998 // disable warning

public class InputLayerController_BattleScene : MonoBehaviour
{
    [SerializeField]
    private InputLayerSO battleOptionLayer;

    [SerializeField]
    private InputLayerSO battleLogLayer;

    private IPublisher<InputLayer> layerPub;

    private IAsyncSubscriber<ActionSelectEndMessage> selectEndASub;

    private IAsyncSubscriber<TurnStartMessage> turnStartASub;

    private System.IDisposable disposable;

    [Inject]
    private readonly ISubscriber<InputLayerSO, CancelInput> cancelSub;

    private IPublisher<ActionSelectCancelMessage> cancelSelectPub;

    void Awake()
    {
        layerPub = GlobalMessagePipe.GetPublisher<InputLayer>();
        selectEndASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectEndMessage>();
        turnStartASub = GlobalMessagePipe.GetAsyncSubscriber<TurnStartMessage>();

        cancelSelectPub = GlobalMessagePipe.GetPublisher<ActionSelectCancelMessage>();

        var bag = DisposableBag.CreateBuilder();

        selectEndASub.Subscribe(async (get, ct) =>
        {
            //Debug.Log(name);
            layerPub.Publish(new InputLayer(battleLogLayer));
        }).AddTo(bag);

        turnStartASub.Subscribe(async (get, ct) =>
        {
            layerPub.Publish(new InputLayer(battleOptionLayer));
        }).AddTo(bag);

        cancelSub.Subscribe(battleOptionLayer, get =>
        {
            cancelSelectPub.Publish(new ActionSelectCancelMessage());
        }).AddTo(bag);

        disposable = bag.Build();
    }


    void OnDestroy()
    {
        disposable?.Dispose();
    }
}
