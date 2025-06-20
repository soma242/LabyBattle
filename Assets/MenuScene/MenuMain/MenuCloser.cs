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

public class MenuCloser : MonoBehaviour, IPointerClickHandler
{
    private Image image;
    private GraphicRaycaster raycaster;

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
            //Debug.Log("select");
            SelectThisComponent();

        });
        disposableIPointer = holder.pointerSub.Subscribe(get =>
        {
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
            var closePub = GlobalMessagePipe.GetPublisher<MenuToDungeonMessage>();
            closePub.Publish(new MenuToDungeonMessage());
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
}
