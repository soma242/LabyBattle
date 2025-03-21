using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BattleSceneMessage;

//using TMPro;
using UnityEngine.UI;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning

using UnityEngine.EventSystems;


public class SelectFinishController : BaseSelectMessageHolder, IPointerClickHandler
{
    [SerializeField]
    private Canvas canvas;
    //[SerializeField]
    //private TMP_Text text;

    //é©ï™ÇÃéØï î‘çÜÇ∆é©ï™Ç©ÇÁÇ¬Ç»Ç™ÇÈéØï î‘çÜ
    [SerializeField]
    private int myNum;
    [SerializeField]
    private int upNum;
    [SerializeField]
    private int downNum;


    //selectïœçXópMessagePipe
    //BuiltInContainer
    private IPublisher<SelectMessage, SelectChange> selectPublisher;
    private ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    

    private IAsyncPublisher<ActionSelectBookMessage> bookAPub;
    private IPublisher<BookCompleteMessage> completePub;

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        //var bag = DisposableBag.CreateBuilder();

        //canvas = GetComponent<Canvas>();

        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        bookAPub = GlobalMessagePipe.GetAsyncPublisher<ActionSelectBookMessage>();
        completePub = GlobalMessagePipe.GetPublisher<BookCompleteMessage>();

        disposableOnDestroy = selectSubscriber.Subscribe(new SelectMessage(inputLayerSO, myNum), i =>
        {
            SelectThisComponent();


        });
    }

    private async UniTask SelectThisComponent()
    {
        selectDispPub.Publish(inputLayerSO, new DisposeSelect());
        //åªç›ëñÇ¡ÇƒÇ¢ÇÈPublishÇéÛÇØì¸ÇÍÇ»Ç¢ÇΩÇﬂÇ…1frameë“Ç¬
        await UniTask.NextFrame();

        canvas.enabled = true ;

        var bag = DisposableBag.CreateBuilder();




        upSubscriber.Subscribe(inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        downSubscriber.Subscribe(inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);

        enterSub.Subscribe(inputLayerSO, async get =>
        {
            await bookAPub.PublishAsync(new ActionSelectBookMessage());


            completePub.Publish(new BookCompleteMessage());
            NextDownSelect();
        }).AddTo(bag);

        selectDispSub.Subscribe(inputLayerSO, i =>
        {
            UnselectThisComponent();
        }).AddTo(bag);


        disposableInput = bag.Build();
    }

    private void UnselectThisComponent()
    {
        disposableInput?.Dispose();

        canvas.enabled = false;
    }

    private void NextUpSelect()
    {

        UnselectThisComponent();
        selectPublisher.Publish(new SelectMessage(inputLayerSO, upNum), new SelectChange());

    }

    private void NextDownSelect()
    {
        UnselectThisComponent();

        selectPublisher.Publish(new SelectMessage(inputLayerSO, downNum), new SelectChange());

    }

    public async void OnPointerClick(PointerEventData pointerEventData)
    {
        await bookAPub.PublishAsync(new ActionSelectBookMessage());


        completePub.Publish(new BookCompleteMessage());
        NextDownSelect();
    }

}
