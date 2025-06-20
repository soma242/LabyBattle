using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

public class InputSupporterMenu : MonoBehaviour
{
    [SerializeField]
    private MenuSelectHolderSO holder;

    private System.IDisposable disposableOnDestroy;

    void Start()
    {
        var layerPub = GlobalMessagePipe.GetPublisher<InputLayer>();
        layerPub.Publish(new InputLayer(holder.mainLayer));

    }

    void Awake()
    {
        var bag = DisposableBag.CreateBuilder();

        holder.menuSub.Subscribe(holder.mainLayer, get =>
        {
            var closePub = GlobalMessagePipe.GetPublisher<MenuToDungeonMessage>();
            closePub.Publish(new MenuToDungeonMessage());
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
        
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        holder.disposableReturn?.Dispose();
    }
}
