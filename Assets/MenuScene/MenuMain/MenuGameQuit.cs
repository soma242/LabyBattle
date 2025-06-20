using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

using MenuScene;


using MessagePipe;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

public class MenuGameQuit : MonoBehaviour, IPointerClickHandler
{
    private Image image;
    private GraphicRaycaster raycaster;

    [SerializeField]
    private Canvas detailCanvas;

    [SerializeField]
    private MenuSelectHolderSO holder;

    [SerializeField]
    private int upNum;
    [SerializeField]
    private int myNum;
    [SerializeField]
    private int downNum;




    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableInput;
    private System.IDisposable disposableIPointer;


    void Awake()
    {
        image = GetComponent<Image>();
        raycaster = GetComponent<GraphicRaycaster>();

        disposableOnDestroy = holder.selectSub.Subscribe(new SelectMessage(holder.mainLayer, myNum), i =>
        {
            SelectThisComponent();

        });

        disposableIPointer = holder.pointerSub.Subscribe(get =>
        {
            //raycastTarget‚ð‘€ì‚µ‚Ä‚àƒNƒŠƒbƒN‚Å‚«‚Ä‚µ‚Ü‚¤

            raycaster.enabled = !raycaster.enabled;
        });

    }

    void OnDestroy()
    {
        disposableInput?.Dispose();
        disposableIPointer?.Dispose();
        disposableOnDestroy?.Dispose();
    }



    public async void OnPointerClick(PointerEventData pointerEventData)
    {
        SelectThisComponent();
        MoveDetail();

    }

    private async UniTask SelectThisComponent()
    {
        holder.selectDispPub.Publish(holder.mainLayer, new DisposeSelect());
        await UniTask.NextFrame();

        image.sprite = holder.sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();

        holder.upSub.Subscribe(holder.mainLayer, i => {
            holder.selectPub.Publish(new SelectMessage(holder.mainLayer, upNum), new SelectChange());
        }).AddTo(bag);

        holder.downSub.Subscribe(holder.mainLayer, i => {
            holder.selectPub.Publish(new SelectMessage(holder.mainLayer, downNum), new SelectChange());
        }).AddTo(bag);

        holder.enterSub.Subscribe(holder.mainLayer, i => {
            MoveDetail();
        }).AddTo(bag);
        
        holder.rightSub.Subscribe(holder.mainLayer, i => {
            MoveDetail();
        }).AddTo(bag);

        holder.selectDispSub.Subscribe(holder.mainLayer, get =>
        {
            UnselectThisComponent();
        }).AddTo(bag);

        disposableInput = bag.Build();

    }

    private void UnselectThisComponent()
    {
        disposableInput?.Dispose();
        image.sprite = holder.sourceImageSO.offSelect;
    }

    private async UniTask MoveDetail()
    {
        holder.pointerPub.Publish(new MenuIPointerMessage());
        await UniTask.NextFrame();

        detailCanvas.enabled = true;
        holder.layerPub.Publish(new InputLayer(holder.detailLayer));
        holder.selectPub.Publish(new SelectMessage(holder.detailLayer, 1), new SelectChange());
        holder.disposableReturn = holder.pointerSub.Subscribe(get=>
        {
            holder.layerPub.Publish(new InputLayer(holder.mainLayer));
            holder.disposableReturn?.Dispose();
            detailCanvas.enabled = false;
        });

    }
}
