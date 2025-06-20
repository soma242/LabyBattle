
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


using MessagePipe;
using DungeonSceneMessage;



using Cysharp.Threading.Tasks;

//IPointerにはGraphicRaycasterが必須（Canvasコンポーネントのみの追加では忘れがち）
public class MapExpander : MonoBehaviour, IPointerClickHandler
{
    private Canvas canvas;

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        canvas = GetComponent<Canvas>();

        var bag = DisposableBag.CreateBuilder();

        var closeSub = GlobalMessagePipe.GetSubscriber<MapCloseMessage>();
        closeSub.Subscribe(get =>
        {
            canvas.enabled = true;
        }).AddTo(bag);

        var expandSub = GlobalMessagePipe.GetSubscriber<MapExpandMessage>();
        expandSub.Subscribe(get =>
        {
            canvas.enabled = false;
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        var expandPub = GlobalMessagePipe.GetPublisher<MapExpandMessage>();
        expandPub.Publish(new MapExpandMessage());
    }


}
