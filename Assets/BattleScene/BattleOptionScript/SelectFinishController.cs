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


public class SelectFinishController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Canvas canvas;
    //[SerializeField]
    //private TMP_Text text;

    //é©ï™ÇÃéØï î‘çÜÇ∆é©ï™Ç©ÇÁÇ¬Ç»Ç™ÇÈéØï î‘çÜ
    [SerializeField]
    private int upNum;
    [SerializeField]
    private int myNum;
    [SerializeField]
    private int downNum;

    [SerializeField]
    private BaseSelectMessageHolder holder;

    //selectïœçXópMessagePipe
    //BuiltInContainer
    private IPublisher<SelectMessage, SelectChange> selectPublisher;
    private ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    

    private IAsyncPublisher<ActionSelectBookMessage> bookAPub;
    private IPublisher<BookCompleteMessage> completePub;

    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableInput;

    void Awake()
    {
        //var bag = DisposableBag.CreateBuilder();

        //canvas = GetComponent<Canvas>();

        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        bookAPub = GlobalMessagePipe.GetAsyncPublisher<ActionSelectBookMessage>();
        completePub = GlobalMessagePipe.GetPublisher<BookCompleteMessage>();

        disposableOnDestroy = selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
        {
            SelectThisComponent();


        });
    }

    void OnDestroy()
    {
        disposableInput?.Dispose();
        disposableOnDestroy?.Dispose();
    }

    private async UniTask SelectThisComponent()
    {
        holder.selectDispPub.Publish(holder.inputLayerSO, new DisposeSelect());
        //åªç›ëñÇ¡ÇƒÇ¢ÇÈPublishÇéÛÇØì¸ÇÍÇ»Ç¢ÇΩÇﬂÇ…1frameë“Ç¬
        await UniTask.NextFrame();

        canvas.enabled = true ;

        var bag = DisposableBag.CreateBuilder();




        holder.upSub.Subscribe(holder.inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        holder.downSub.Subscribe(holder.inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);

        holder.enterSub.Subscribe(holder.inputLayerSO, async get =>
        {
            await bookAPub.PublishAsync(new ActionSelectBookMessage());


            completePub.Publish(new BookCompleteMessage());
            NextDownSelect();
        }).AddTo(bag);

        holder.selectDispSub.Subscribe(holder.inputLayerSO, i =>
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

        //UnselectThisComponent();
        selectPublisher.Publish(new SelectMessage(holder.inputLayerSO, upNum), new SelectChange());

    }

    private void NextDownSelect()
    {
        //UnselectThisComponent();

        selectPublisher.Publish(new SelectMessage(holder.inputLayerSO, downNum), new SelectChange());

    }

    public async void OnPointerClick(PointerEventData pointerEventData)
    {
        await bookAPub.PublishAsync(new ActionSelectBookMessage());


        completePub.Publish(new BookCompleteMessage());
        NextDownSelect();
    }

}
