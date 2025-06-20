using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MenuScene;

using UnityEngine.EventSystems;

using MessagePipe;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

public class MenuDetailGameQuit : MonoBehaviour, IPointerClickHandler
{
    private Image image;

    [SerializeField]
    private MenuSelectHolderSO holder;






    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableInput;

    void Awake()
    {
        image = GetComponent<Image>();

        disposableOnDestroy = holder.selectSub.Subscribe(new SelectMessage(holder.detailLayer, DetailNum.quitNum), i =>
        {
            SelectThisComponent();

        });

    }

    void OnDestroy()
    {
        disposableInput?.Dispose();
        disposableOnDestroy?.Dispose();
    }

    public async void OnPointerClick(PointerEventData pointerEventData)
    {
        SelectThisComponent();
    }

    private async UniTask SelectThisComponent()
    {
        holder.selectDispPub.Publish(holder.detailLayer, new DisposeSelect());
        await UniTask.NextFrame();

        image.sprite = holder.sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();

        holder.upSub.Subscribe(holder.detailLayer, i => {
            holder.selectPub.Publish(new SelectMessage(holder.detailLayer, DetailNum.titleNum), new SelectChange());
        }).AddTo(bag);

        holder.downSub.Subscribe(holder.detailLayer, i => {
            holder.selectPub.Publish(new SelectMessage(holder.detailLayer, DetailNum.titleNum), new SelectChange());
        }).AddTo(bag);

        holder.leftSub.Subscribe(holder.detailLayer, i => {
            MoveMain();
        }).AddTo(bag);
        
        holder.menuSub.Subscribe(holder.detailLayer, i => {
            MoveMain();
        }).AddTo(bag);

        holder.enterSub.Subscribe(holder.detailLayer, i => {

            Debug.Log("enter");
            Application.Quit();
        }).AddTo(bag);

        holder.selectDispSub.Subscribe(holder.detailLayer, get =>
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

    private void MoveMain()
    {
        UnselectThisComponent();

        holder.pointerPub.Publish(new MenuIPointerMessage());

        holder.layerPub.Publish(new InputLayer(holder.mainLayer));
        //Main‚ÅUnSelect‚µ‚Ä‚¢‚È‚¢‚Ì‚Å‘I‘ð‚³‚ê‚½‚Ü‚Ü
    }
}
