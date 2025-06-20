using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning


using MessagePipe;
using StartSceneMessage;

public class CheckFrameController : MonoBehaviour
{
    private Canvas canvas;

    [SerializeField]
    private CheckSelectButton yes;
    [SerializeField]
    private CheckSelectButton no;

    [SerializeField]
    private SelectSourceImageSO sourceImage;

    public StartSelectHolderSO holder;

    private bool selectYes = true;


    [SerializeField]
    private TMP_Text text;

    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableYes;
    void Awake()
    {
        canvas = GetComponent<Canvas>();

        var bag = DisposableBag.CreateBuilder();



        var startSub = GlobalMessagePipe.GetSubscriber<StartSelectMessage>();
        startSub.Subscribe(get =>
        {
            //Debug.Log(get.played);
            if (get.played)
            {
                canvas.enabled = true;
                text.SetText("これまでのデータは削除されます。宜しいですか？");

                holder.layerPub.Publish(new InputLayer(holder.checkNewGameLayer));

                selectYes = false;
                no.image.sprite = sourceImage.onSelect;

                var bag = DisposableBag.CreateBuilder();
                var yesSub = GlobalMessagePipe.GetSubscriber<YesCheckMessage>();
                yesSub.Subscribe(async get =>
                {
                    NewGameFunc();
                }).AddTo(bag);

                holder.rightSub.Subscribe(holder.checkNewGameLayer, get =>
                {
                    SelectOther();
                }).AddTo(bag);
                holder.leftSub.Subscribe(holder.checkNewGameLayer, get =>
                {
                    SelectOther();
                }).AddTo(bag);
                holder.enterSub.Subscribe(holder.checkNewGameLayer, get =>
                {
                    if (selectYes)
                    {
                        yes.CheckPerfome();
                    }
                    else
                    {
                        no.CheckPerfome();
                    }
                }).AddTo(bag);

                disposableYes = bag.Build();
            }
            else
            {
                NewGameFunc();

            }
        }).AddTo(bag);

        var noSub = GlobalMessagePipe.GetSubscriber<NoCheckMessage>();
        noSub.Subscribe(get =>
        {
            canvas.enabled = false;
            disposableYes?.Dispose();
            holder.layerPub.Publish(new InputLayer(holder.startLayer));
        }).AddTo(bag);

        var yesSub = GlobalMessagePipe.GetSubscriber<YesCheckMessage>();
        yesSub.Subscribe(get =>
        {
            canvas.enabled = false;
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();

    }
    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        disposableYes?.Dispose();
    }

    private async UniTask NewGameFunc()
    {
        var newgamePub = GlobalMessagePipe.GetAsyncPublisher<NewGameMessage>();
        await newgamePub.PublishAsync(new NewGameMessage());

        var disablePub = GlobalMessagePipe.GetPublisher<StartSceneDisableMessage>();
        disablePub.Publish(new StartSceneDisableMessage());
    }

    private void SelectOther()
    {
        if (selectYes)
        {
            selectYes = false;
            yes.image.sprite = sourceImage.offSelect;
            no.image.sprite = sourceImage.onSelect;
        }
        else
        {
            selectYes = true;
            yes.image.sprite = sourceImage.onSelect;
            no.image.sprite = sourceImage.offSelect;
        }
    }
}
