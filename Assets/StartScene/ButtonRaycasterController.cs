using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

using MessagePipe;
using StartSceneMessage;


public class ButtonRaycasterController : MonoBehaviour
{
    private GraphicRaycaster raycaster;
    private System.IDisposable disposable;

    void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();

        var bag = DisposableBag.CreateBuilder();
        var startSub = GlobalMessagePipe.GetSubscriber<StartSelectMessage>();
        startSub.Subscribe(get =>
        {
            raycaster.enabled = false;

        }).AddTo(bag);

        var noSub = GlobalMessagePipe.GetSubscriber<NoCheckMessage>();
        noSub.Subscribe(get =>
        {
            raycaster.enabled = true;
        }).AddTo(bag);

        disposable = bag.Build();
    }

    void OnDestroy()
    {
        disposable?.Dispose();
    }
}
