
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;

using MessagePipe;

using StartSceneMessage;


public class ShutDownButton : MonoBehaviour, IPointerClickHandler
{

    public StartSelectHolderSO holder;

    [SerializeField]
    private sbyte preKey;
    [SerializeField]
    private sbyte key;
    [SerializeField]
    private sbyte nextKey;


    [SerializeField]
    private SelectSourceImageSO sourceImageSO;

    //アタッチされたオブジェクトのイメージ
    private Image image;


    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        image = GetComponent<Image>();



        StartSelectSub();
    }
    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }

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
            Application.Quit();
            Debug.Log(name);
        }).AddTo(bag);
        
        /*
        var disSub = GlobalMessagePipe.GetSubscriber<StartSceneDisableMessage>();
        disSub.Subscribe(get =>
        {
            disposableOnDestroy?.Dispose();
            StartInputSub();
        }).AddTo(bag);
        */

        disposableOnDestroy = bag.Build();
    }

    private async void StartSelectSub()
    {
        image.sprite = sourceImageSO.offSelect;

        await UniTask.NextFrame();

        disposableOnDestroy = holder.selectSub.Subscribe(new SelectMessage(holder.startLayer, key), get =>
        {
            disposableOnDestroy?.Dispose();
            StartInputSub();
        });
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Application.Quit();
        Debug.Log(name);
    }

}

