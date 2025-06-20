
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using MessagePipe;

using StartSceneMessage;
using VContainer;
using VContainer.Unity;

using Cysharp.Threading.Tasks;

public class NewGameButton : MonoBehaviour, IPointerClickHandler
{
    public StartSelectHolderSO holder;

    private ISubscriber<StartSceneDisableMessage> disSub;

    [SerializeField]
    private sbyte preKey;
    [SerializeField]
    private sbyte key;
    [SerializeField]
    private sbyte nextKey;

    //private bool dataExist;

    [SerializeField]
    private SelectSourceImageSO sourceImageSO;
    
    [SerializeField]
    private MSO_NewGameControllSO boolHolder;

    //アタッチされたオブジェクトのイメージ
    private Image image;

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        image = GetComponent<Image>();



        disSub = GlobalMessagePipe.GetSubscriber<StartSceneDisableMessage>();
        EnableStartCanvas();

    }
    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }

    private void EnableStartCanvas()
    {
        if (boolHolder.played)
        {
            StartSelectSub();
        }
        else
        {
            StartInputSub();
        }
    }
    /*
    private void SetDisableSub()
    {
        disposableOnDestroy?.Dispose();
        var enaSub = GlobalMessagePipe.GetSubscriber<StartSceneEnableMessage>();
        disposableOnDestroy = enaSub.Subscribe(get =>
        {
            disposableOnDestroy?.Dispose();
            EnableStartCanvas();
        });
    }
    */

    //async, await 無しだと連続発動してしまう
    private async void StartInputSub()
    {
        image.sprite = sourceImageSO.onSelect;
        await UniTask.NextFrame();


        var bag = DisposableBag.CreateBuilder();

        holder.upSub.Subscribe(holder.startLayer, get =>
        {
            disposableOnDestroy?.Dispose();
            holder.selectPub.Publish(new SelectMessage(holder.startLayer, preKey), new SelectChange());
            StartSelectSub();
        }).AddTo(bag);
        holder.downSub.Subscribe(holder.startLayer, get =>
        {
            disposableOnDestroy?.Dispose();
            holder.selectPub.Publish(new SelectMessage(holder.startLayer, nextKey), new SelectChange());
            StartSelectSub();

        }).AddTo(bag);
        holder.enterSub.Subscribe(holder.startLayer, get =>
        {
            var startPub = GlobalMessagePipe.GetPublisher<StartSelectMessage>();
            startPub.Publish(new StartSelectMessage(boolHolder.played));
            //Debug.Log(name);
        }).AddTo(bag);

        /*
        disSub.Subscribe(get =>
        {
            SetDisableSub();
        }).AddTo(bag);
        */

        disposableOnDestroy = bag.Build();
    }

    private async void StartSelectSub()
    {
        image.sprite = sourceImageSO.offSelect;
        await UniTask.NextFrame();

        var bag = DisposableBag.CreateBuilder();

        holder.selectSub.Subscribe(new SelectMessage(holder.startLayer, key), get =>
        {
            //Debug.Log("select");
            disposableOnDestroy?.Dispose();
            StartInputSub();
        }).AddTo(bag);
        /*
        disSub.Subscribe(get =>
        {
            SetDisableSub();
        }).AddTo(bag);
        */
        disposableOnDestroy = bag.Build();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //To_CheckFrameController
        var startPub = GlobalMessagePipe.GetPublisher<StartSelectMessage>();
        startPub.Publish(new StartSelectMessage(boolHolder.played));
        //Debug.Log(name);
    }

}

