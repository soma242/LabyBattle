using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;

using BattleSceneMessage;

#pragma warning disable CS1998 // disable warning


public class ZoneCanvasController : MonoBehaviour
{
    private IAsyncSubscriber<ActionSelectEndMessage> selectEndASub;

    [SerializeField]
    private InputLayerSO battleLogLayer;
    [SerializeField]
    private InputLayerSO battleOptionLayer;

    private ISubscriber<InputLayerSO, InputLayerChanged> layerChangedSub;

    private System.IDisposable disposable;

    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();

        selectEndASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectEndMessage>();

        layerChangedSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, InputLayerChanged>();

        var bag = DisposableBag.CreateBuilder();

        selectEndASub.Subscribe(async (get, ct) =>
        {
            canvas.enabled = false;
        }).AddTo(bag);

        layerChangedSub.Subscribe(battleLogLayer, get =>
        {
            canvas.enabled = false;
        }).AddTo(bag);

        layerChangedSub.Subscribe(battleOptionLayer, get =>
        {
            canvas.enabled = true; 
        }).AddTo(bag);

        disposable = bag.Build();

    }

    void OnDestroy()
    {
        disposable?.Dispose();
    }
}
