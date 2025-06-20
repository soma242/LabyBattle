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



public class MoveTargetController : MonoBehaviour, IPointerClickHandler
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
    private System.IDisposable disposableInput;

    private IPublisher<MoveSimulateMessage> moveSimuPub;

    private IPublisher<TauntSimulateCancell> tSimuCancellPub;

    private ISubscriber<Move_NoneTarget> noneTargetSub;
    private ISubscriber<Move_SingleEnemyTarget> singleEnemySub;

    private IAsyncSubscriber<ActionSelectBookMessage> bookASub;
    private IPublisher<BookCommonMoveTargetMessage> bookTargetPub;

    void Awake()
    {
        image = GetComponent<Image>();




        //BuiltInContainer
        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        noneTargetSub = GlobalMessagePipe.GetSubscriber<Move_NoneTarget>();
        singleEnemySub = GlobalMessagePipe.GetSubscriber<Move_SingleEnemyTarget>();

        returnTargetSub = GlobalMessagePipe.GetSubscriber<ReturnTargetName>();
        nextTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetNextTargetName>();
        preTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetPreTargetName>();

        moveSimuPub = GlobalMessagePipe.GetPublisher<MoveSimulateMessage>();

        tSimuCancellPub = GlobalMessagePipe.GetPublisher<TauntSimulateCancell>();

        bookASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectBookMessage>();
        bookTargetPub = GlobalMessagePipe.GetPublisher<BookCommonMoveTargetMessage>();

        var bag = DisposableBag.CreateBuilder();

        //Targetingの種類によって処理を変える
        //From_MoveSkillTargetSO
        noneTargetSub.Subscribe(get =>
        {
            disposableSelect?.Dispose();
            targetPos = FormationScope.NoneChara();
            targeting = false;

            text.SetText(FormationScope.NoneTargetText());
            moveSimuPub.Publish(new MoveSimulateMessage(FormationScope.NoneChara()));
            disposableSelect = selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
            {
                SelectThisComponent();
            });
        }).AddTo(bag);

        singleEnemySub.Subscribe(infom =>
        {
            disposableSelect?.Dispose();
            targetPos = FormationScope.FirstEnemy();
            targeting = true;

            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                moveSimuPub.Publish(new MoveSimulateMessage(targetPos));
                text.SetText(get.targetName);
                disposableReturn?.Dispose();
            });
            //next,preは受け取り側で次に回すための分割なので，現在のものを受け取りたければどちらでもいい
            //=>逆にPublishだけで次にはいってくれない

            disposableSelect = selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
            {
                SelectThisComponent();
                ChangeTargetPrepare();
            });
            nextTargetPub.Publish(targetPos, new GetNextTargetName());
        }).AddTo(bag);



        bookASub.Subscribe(async (get, ct) =>
        {
            bookTargetPub.Publish(new BookCommonMoveTargetMessage(targetPos));
        }).AddTo(bag);


        

        /*


        singleCharaSub.Subscribe(get =>
        {
            disposableSelect?.Dispose();

            targetPos = 1;
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
            });
        }).AddTo(bag);
        */


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

        //Debug.Log("SelectMT");
        //現在走っているPublishを受け入れないために1frame待つ
        await UniTask.NextFrame();


        image.sprite = sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();




        holder.upSub.Subscribe(holder.inputLayerSO, i => {
            //Debug.Log("up");

            NextUpSelect();
        }).AddTo(bag);

        holder.downSub.Subscribe(holder.inputLayerSO, i => {
            //Debug.Log("down");

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

    private void ChangeTargetPrepare()
    {
        var bag = DisposableBag.CreateBuilder();

        holder.rightSub.Subscribe(holder.inputLayerSO, i => {
            //Debug.Log(targetPos);
            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                moveSimuPub.Publish(new MoveSimulateMessage(targetPos));
                tSimuCancellPub.Publish(new TauntSimulateCancell(targetPos));

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
                moveSimuPub.Publish(new MoveSimulateMessage(targetPos));
                tSimuCancellPub.Publish(new TauntSimulateCancell(targetPos));

                //Debug.Log("sub");
                disposableReturn.Dispose();
            });

            preTargetPub.Publish(PreFormNum(targetPos), new GetPreTargetName());
        }).AddTo(bag);

        disposableTarget = bag.Build();

    }

    private void UnselectThisComponent()
    {
        //Debug.Log("dispOnUns");

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


