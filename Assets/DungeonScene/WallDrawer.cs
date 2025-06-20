using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using MessagePipe;
using DungeonSceneMessage;


public class WallDrawer : MonoBehaviour
{
    //private ISubscriber<sbyte, WallDrawMessage> wallSub;

    [SerializeField]
    private List<Canvas> wallList = new List<Canvas>();

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        var wallSub = GlobalMessagePipe.GetSubscriber<WallDrawMessage>();

        var bag = DisposableBag.CreateBuilder();

        wallSub.Subscribe(get =>
        {
            //Debug.Log(get.wall + "+" + get.index) ;
            wallList[get.index].enabled = get.wall;
        }).AddTo(bag);


        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }
}
