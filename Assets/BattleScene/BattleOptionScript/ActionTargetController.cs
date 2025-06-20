using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning


using BattleSceneMessage;
using SkillStruct;

using UnityEngine.EventSystems;


public class ActionTargetController : MonoBehaviour, IPointerClickHandler
{

    private sbyte targetPos;

    private bool targeting;


    [SerializeField]
    private TMP_Text text;

    //自分の識別番号と自分からつながる識別番号
    [SerializeField]
    private int upNum;
    [SerializeField]
    private int myNum;
    [SerializeField]
    private int downNum;

    [SerializeField]
    private SelectSourceImageSO sourceImageSO;

    [SerializeField]
    private BaseSelectMessageHolder holder;

    //アタッチされたオブジェクトのイメージ
    private Image image;

    //MessagePipe

    //select変更用MessagePipe
    //BuiltInContainer
    private IPublisher<SelectMessage, SelectChange> selectPublisher;
    private ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    private System.IDisposable disposableSelect;

    private System.IDisposable disposableOnDestroy;




    private System.IDisposable disposableTarget;


    private ISubscriber<ReturnTargetName> returnTargetSub;
    private IPublisher<sbyte, GetNextTargetName> nextTargetPub;
    private IPublisher<sbyte, GetPreTargetName> preTargetPub;

    private System.IDisposable disposableReturn;

    //private ISubscriber<Active_SingleEnemyTarget> singleEnemySub;
    //private ISubscriber<Active_SingleCharaTarget> singleCharaSub;


    private IAsyncSubscriber<ActionSelectBookMessage> bookASub;
    private IPublisher<BookCommonActiveTargetMessage> bookTargetPub;

    private System.IDisposable disposableInput;


    void Awake()
    {
        image = GetComponent<Image>();


        //cts = new CancellationTokenSource();


        //BuiltInContainer
        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        var singleEnemySub = GlobalMessagePipe.GetSubscriber<Active_SingleEnemyTarget>();
        var singleCharaSub = GlobalMessagePipe.GetSubscriber<Active_SingleCharaTarget>();
        var allCharaSub = GlobalMessagePipe.GetSubscriber<Active_AllCharaTarget>();

        returnTargetSub = GlobalMessagePipe.GetSubscriber<ReturnTargetName>();
        nextTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetNextTargetName>();
        preTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetPreTargetName>();

        bookASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectBookMessage>();
        bookTargetPub = GlobalMessagePipe.GetPublisher<BookCommonActiveTargetMessage>();


        var bag = DisposableBag.CreateBuilder();

        /*
        selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
        {
            SelectThisComponent();
        }).AddTo(bag);
        */

        singleEnemySub.Subscribe( get =>
        {
            disposableSelect?.Dispose();
            //増えてくるようならSOに纏めてアタッチ
            targetPos = FormationScope.FirstEnemy();
            targeting = true;

            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                disposableReturn?.Dispose();
            });
            //next,preは受け取り側で次に回すための分割なので，現在のものを受け取りたければどちらでもいい
            //=>逆にPublishだけで次にはいってくれない
            nextTargetPub.Publish(targetPos, new GetNextTargetName());
            
            disposableSelect = selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
            {
                SelectThisComponent();
                ChangeTargetPrepare();
            });


        }).AddTo(bag);

        singleCharaSub.Subscribe(get =>
        {
            disposableSelect?.Dispose();

            targetPos = FormationScope.FirstChara();
            targeting = true;

            //targetPosの更新のタイミングがSub時なので連続入力も問題ないはず（ここではInputSystemなので短時間の連続入力は不可）
            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                disposableReturn?.Dispose();
            });
            //next,preは受け取り側で次に回すための分割なので，現在のものを受け取りたければどちらでもいい
            //=>逆にPublishだけで次にはいってくれない
            nextTargetPub.Publish(targetPos, new GetNextTargetName());

            disposableSelect = selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
            {
                SelectThisComponent();
                ChangeTargetPrepare();
            });
        }).AddTo(bag);

        allCharaSub.Subscribe(ActionTargetController =>
        {
            //増えてくるようならSOに纏めてアタッチ
            targetPos = FormationScope.AllChara();
            targeting = true;
            text.SetText(FormationScope.AllCharaText());


        }).AddTo(bag);

        bookASub.Subscribe(async (get, ct) =>
        {
            bookTargetPub.Publish(new BookCommonActiveTargetMessage(targetPos));
        }).AddTo(bag);


        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        disposableTarget?.Dispose();
        disposableSelect?.Dispose();
        disposableInput?.Dispose();
        disposableReturn?.Dispose();
    }

    private async UniTask SelectThisComponent()
    {
        holder.selectDispPub.Publish(holder.inputLayerSO, new DisposeSelect());


        //現在走っているPublishを受け入れないために1frame待つ
        await UniTask.NextFrame();

        image.sprite = sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();




        holder.upSub.Subscribe(holder.inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        holder.downSub.Subscribe(holder.inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);
        holder.enterSub.Subscribe(holder.inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);

        holder.selectDispSub.Subscribe(holder.inputLayerSO, i => 
        {
            UnselectThisComponent();
        }).AddTo(bag);


        disposableInput = bag.Build();
    }

    //Targetの変更を行わない可能性があるので分離，
    //await UniTask.NextFrame();を追加していないのでSelectThisComponentの後ろに登録する。

    private void ChangeTargetPrepare()
    {
        var bag = DisposableBag.CreateBuilder();

        holder.rightSub.Subscribe(holder.inputLayerSO, i => {
            //Debug.Log(targetPos);
            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                //Debug.Log("sub");
                disposableReturn?.Dispose();
            });

            nextTargetPub.Publish(NextFormNum(targetPos), new GetNextTargetName());
            //await UniTask.Delay(TimeSpan.FromSeconds(3f));
            //Debug.Log(NextFormNum(targetPos));

        }).AddTo(bag);

        holder.leftSub.Subscribe(holder.inputLayerSO, i =>
        {
            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                //Debug.Log("sub");
                disposableReturn.Dispose();
            });

            preTargetPub.Publish(PreFormNum(targetPos), new GetPreTargetName());
        }).AddTo(bag);

        disposableTarget = bag.Build();

    }

    private void UnselectThisComponent()
    {

        disposableInput?.Dispose();
        disposableTarget?.Dispose();
        image.sprite = sourceImageSO.offSelect;
    }

    
    public async void OnPointerClick(PointerEventData pointerEventData)
    {
        SelectThisComponent();
        if (targeting)
        {
            ChangeTargetPrepare();
        }
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

    private sbyte NextFormNum(sbyte i)
    {
        i++;
        return i;
    }
    private sbyte PreFormNum(sbyte i)
    {
        i--;
        return i;
    }
}
