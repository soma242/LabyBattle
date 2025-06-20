using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

using MessagePipe;

using StartSceneMessage;


public class ContinueButton : MonoBehaviour, IPointerClickHandler
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

    [SerializeField]
    private MSO_NewGameControllSO boolHolder;

    //アタッチされたオブジェクトのイメージ
    private Image image;


    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposable;

    void Awake()
    {
        image = GetComponent<Image>();

        

        
        EnableStartCanvas();
        holder.layerPub.Publish(new InputLayer(holder.startLayer));
    }
    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        disposable?.Dispose();
    }

    private void EnableStartCanvas()
    {
        if (boolHolder.played)
        {
            StartInputSub();

        }
        else
        {

            //データが存在しないならば選択できない
            Canvas canvas = GetComponent<Canvas>();
            canvas.enabled = false;
            var bagE = DisposableBag.CreateBuilder();

            holder.selectSub.Subscribe(new SelectMessage(holder.startLayer, key), get =>
            {
                var bag = DisposableBag.CreateBuilder();

                holder.upSub.Subscribe(holder.startLayer, get =>
                {
                    disposableOnDestroy?.Dispose();
                    holder.selectPub.Publish(new SelectMessage(holder.startLayer, preKey), new SelectChange());
                }).AddTo(bag);
                holder.downSub.Subscribe(holder.startLayer, get =>
                {
                    disposableOnDestroy?.Dispose();
                    holder.selectPub.Publish(new SelectMessage(holder.startLayer, nextKey), new SelectChange());

                }).AddTo(bag);


                disposableOnDestroy = bag.Build();
            }).AddTo(bagE) ;
            /*
            var disSub = GlobalMessagePipe.GetSubscriber<StartSceneDisableMessage>();
            disSub.Subscribe(get =>
            {
                disposable?.Dispose();
                var enaSub = GlobalMessagePipe.GetSubscriber<StartSceneEnableMessage>();
                disposable = enaSub.Subscribe(get =>
                {
                    disposable?.Dispose();
                    EnableStartCanvas();
                });

            }).AddTo(bagE);
            */

            disposable = bagE.Build();

        }
    }

    private async void StartInputSub()
    {
        image.sprite = sourceImageSO.onSelect;
        await UniTask.NextFrame();


        var bag = DisposableBag.CreateBuilder();

        holder.upSub.Subscribe(holder.startLayer, get =>
        {
            //Debug.Log()
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
            ContinueGame();
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }

    private async void StartSelectSub()
    {
        image.sprite = sourceImageSO.offSelect;

        await UniTask.NextFrame();

        disposableOnDestroy = holder.selectSub.Subscribe(new SelectMessage(holder.startLayer, key), get =>
        {
            //Debug.Log("selectCont");

            disposableOnDestroy?.Dispose();
            StartInputSub();
        });
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        ContinueGame();
    }

    private async UniTask ContinueGame()
    {
        var contPub = GlobalMessagePipe.GetAsyncPublisher<GameContinueMessage>();
        await contPub.PublishAsync(new GameContinueMessage());
        var disablePub = GlobalMessagePipe.GetPublisher<StartSceneDisableMessage>();
        disablePub.Publish(new StartSceneDisableMessage());
    }

}

