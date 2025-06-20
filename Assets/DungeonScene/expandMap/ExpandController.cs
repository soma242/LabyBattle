
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


using MessagePipe;
using DungeonSceneMessage;

using VContainer;
using VContainer.Unity;

using Cysharp.Threading.Tasks;

public class ExpandController : MonoBehaviour
{
    private Canvas canvas;

    
    [SerializeField]
    private InputLayerSO layer;
    

    private ISubscriber<InputLayerSO, MapInput> mapInputSub;

    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableExpanded;

    void Awake()
    {

        canvas = GetComponent<Canvas>();

        var bag = DisposableBag.CreateBuilder();

        mapInputSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, MapInput>();

        var expandSub = GlobalMessagePipe.GetSubscriber<MapExpandMessage>();

        expandSub.Subscribe(get =>
        {
            disposableExpanded?.Dispose();
            canvas.enabled = true;


            //UniTask.NextFrame();

            //var bagE = DisposableBag.CreateBuilder();

            disposableExpanded?.Dispose();
            disposableExpanded = mapInputSub.Subscribe(layer, get =>
            {
                CloseMapPub();
            });
            

             


        }).AddTo(bag);

        var closeSub = GlobalMessagePipe.GetSubscriber<MapCloseMessage>();
        closeSub.Subscribe(get =>
        {
            disposableExpanded?.Dispose();
            canvas.enabled = false;
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();

        
        disposableExpanded = mapInputSub.Subscribe(layer, get =>
        {
            var expandPub = GlobalMessagePipe.GetPublisher<MapExpandMessage>();
            expandPub.Publish(new MapExpandMessage());
        });
        
    }
    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        disposableExpanded?.Dispose();
    }

    private void CloseMapPub()
    {
        disposableExpanded?.Dispose();
        var closePub = GlobalMessagePipe.GetPublisher<MapCloseMessage>();
        closePub.Publish(new MapCloseMessage());

        
        disposableExpanded = mapInputSub.Subscribe(layer, get =>
        {
            var expandPub = GlobalMessagePipe.GetPublisher<MapExpandMessage>();
            expandPub.Publish(new MapExpandMessage());
        });
        

    }
}
