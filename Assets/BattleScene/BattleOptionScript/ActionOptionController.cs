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

public class ActionOptionController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int listNum;



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

    private SkillListHolderSO skillListHolderSO;

    //MessagePipe

    private CancellationTokenSource cts;
    private IPublisher<DescriptionDemendMessage> descDemendPub;
    private IPublisher<DescriptionFinishMessage> descFinishPub;

    private IPublisher<ASkillSimulateMessage> simulatePub;

    //select変更用MessagePipe
    //BuiltInContainer
    private IPublisher<SelectMessage, SelectChange> selectPublisher;
    private ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    //SkillList受け入れ
    private ISubscriber<SelectSkillListChangeMessage> skillListSub;

    private System.IDisposable disposableOnDestroy;

    private IAsyncSubscriber<ActionSelectBookMessage> bookASub;
    private IPublisher<BookCommonActiveKeyMessage> bookKeyPub;


    private System.IDisposable disposableInput;

    //Input受け入れ用MessagePipe



    //関数
    void Awake()
    {
        image = GetComponent<Image>();

        listNum = 0;

        //BuiltInContainer

        descDemendPub = GlobalMessagePipe.GetPublisher<DescriptionDemendMessage>();
        descFinishPub = GlobalMessagePipe.GetPublisher<DescriptionFinishMessage>();

        simulatePub = GlobalMessagePipe.GetPublisher<ASkillSimulateMessage>();

        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        skillListSub = GlobalMessagePipe.GetSubscriber<SelectSkillListChangeMessage>();

        bookASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectBookMessage>();
        bookKeyPub = GlobalMessagePipe.GetPublisher<BookCommonActiveKeyMessage>();

        var bag = DisposableBag.CreateBuilder();

        selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
        {
            SelectThisComponent();
        }).AddTo(bag);

        skillListSub.Subscribe(get =>
        {

            listNum = 0;
            skillListHolderSO = get.skillListHolderSO;
            text.SetText(skillListHolderSO.aSkillCatalog[listNum].GetSkillName());
            simulatePub.Publish(new ASkillSimulateMessage(skillListHolderSO.aSkillCatalog[listNum].GetSkillName()));
            skillListHolderSO.aSkillCatalog[listNum].skillTarget.SetTargetSort();
        }).AddTo(bag);

        bookASub.Subscribe(async (get,ct) =>
        {
            bookKeyPub.Publish(new BookCommonActiveKeyMessage(skillListHolderSO.aSkillCatalog[listNum].GetSkillKey()));
            skillListHolderSO.aSkillCatalog[listNum].AcriveSkillBootBook();
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        disposableInput?.Dispose();
    }

    private async UniTask SelectThisComponent()
    {
        //Debug.Log("AOSelect");
        holder.selectDispPub.Publish(holder.inputLayerSO, new DisposeSelect());

        //現在走っているPublishを受け入れないために1frame待つ
        await UniTask.NextFrame();

        image.sprite = sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();


        //Debug.Log("selectedAOC");


        holder.upSub.Subscribe(holder.inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        holder.downSub.Subscribe(holder.inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);
        holder.enterSub.Subscribe(holder.inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);

        holder.selectDispSub.Subscribe(holder.inputLayerSO, get =>
        {
            UnselectThisComponent();
        }).AddTo(bag);

        holder.rightSub.Subscribe(holder.inputLayerSO, i => {
            AddListNum();
            text.SetText(skillListHolderSO.aSkillCatalog[listNum].GetSkillName());
            //text.SetText($"aiueo{FormationScope.FrontCharaText()}");
            //対応するスキル名をPub
            simulatePub.Publish(new ASkillSimulateMessage(skillListHolderSO.aSkillCatalog[listNum].GetSkillName()));

            skillListHolderSO.aSkillCatalog[listNum].skillTarget.SetTargetSort();

        }).AddTo(bag);

        holder.leftSub.Subscribe(holder.inputLayerSO, i =>
        {
            DeductListNum();
            text.SetText(skillListHolderSO.aSkillCatalog[listNum].GetSkillName());
            simulatePub.Publish(new ASkillSimulateMessage(skillListHolderSO.aSkillCatalog[listNum].GetSkillName()));

            skillListHolderSO.aSkillCatalog[listNum].skillTarget.SetTargetSort();

        }).AddTo(bag);


        disposableInput = bag.Build();
    }

    public async void OnPointerEnter(PointerEventData pointerEventData)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        try
        {
            await WaitForSecond(cts);
            descDemendPub.Publish(new DescriptionDemendMessage(skillListHolderSO.aSkillCatalog[listNum].GetDescription()));
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
        {
#if UNITY_EDITOR
                    if (cts.IsCancellationRequested)
                    {
                        // 引数のCancellationTokenが原因なので、それを保持したOperationCanceledExceptionとして投げる
                        //throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // タイムアウトが原因なので、TimeoutException(或いは独自の例外)として投げる
                        throw new TimeoutException("The request was canceled due to the configured Timeout ");
                    }
#endif


        }

    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        cts?.Cancel();
        descFinishPub.Publish(new DescriptionFinishMessage());
    }


    public async void OnPointerClick(PointerEventData pointerEventData)
    {
        SelectThisComponent();
    }


    private async UniTask WaitForSecond(CancellationTokenSource cts)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: cts.Token);

    }

    private void AddListNum()
    {
        
        if(skillListHolderSO.aSkillCatalog.Count-1 != listNum)
        {
            listNum++;

        }
        else
        {
            listNum = 0;

        }
        
    }

    private void DeductListNum()
    {
        
        if (listNum !=0)
        {
            listNum--;
        }
        else
        {
            listNum = skillListHolderSO.aSkillCatalog.Count - 1;
        }
        
    }

    private void UnselectThisComponent()
    {
        disposableInput?.Dispose();
        image.sprite = sourceImageSO.offSelect;
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
}
