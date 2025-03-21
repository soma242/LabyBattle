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


public class MoveOptionController : BaseSelectMessageHolder, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int listNum;


    [SerializeField]
    private TMP_Text text;

    //自分の識別番号と自分からつながる識別番号
    [SerializeField]
    private int myNum;
    [SerializeField]
    private int upNum;
    [SerializeField]
    private int downNum;

    [SerializeField]
    private SelectSourceImageSO sourceImageSO;

    //アタッチされたオブジェクトのイメージ
    private Image image;

    private SkillListHolderSO skillListHolderSO;

    private MovePosition currentPos;

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

    private ISubscriber<MoveSimulateMessage> moveSimuSub;
    private System.IDisposable disposableSimu;


    

    private System.IDisposable disposableSkill;

    private IAsyncSubscriber<ActionSelectBookMessage> bookASub;
    private IPublisher<BookCommonMoveKeyMessage> bookKeyPub;

    private System.IDisposable disposableBook;


    //関数
    private void Awake()
    {
        image = GetComponent<Image>();

        listNum = 0;

        //BuiltInContainer


        descDemendPub = GlobalMessagePipe.GetPublisher<DescriptionDemendMessage>();
        descFinishPub = GlobalMessagePipe.GetPublisher<DescriptionFinishMessage>();

        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        skillListSub = GlobalMessagePipe.GetSubscriber<SelectSkillListChangeMessage>();

        moveSimuSub = GlobalMessagePipe.GetSubscriber<MoveSimulateMessage>();

        bookASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectBookMessage>();
        bookKeyPub = GlobalMessagePipe.GetPublisher<BookCommonMoveKeyMessage>();



        var bag = DisposableBag.CreateBuilder();

        //SkillListSubの前に個々に入ってしまっている。
        selectSubscriber.Subscribe(new SelectMessage(inputLayerSO, myNum), i =>
        {
            disposableSkill?.Dispose();
            disposableBook?.Dispose();
            SelectThisComponent();
            if (currentPos == MovePosition.front)
            {
                ChangeSkillOnFrontPrepare();


            }
            else
            {
                ChangeSkillOnBackPrepare();
                
            }

        }).AddTo(bag);

        skillListSub.Subscribe(get =>
        {
            listNum = 0;
            skillListHolderSO = get.skillListHolderSO;
            currentPos = get.currentPos;



            if(currentPos == MovePosition.front)
            {
                disposableSimu?.Dispose();
                text.SetText(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillName());
                skillListHolderSO.mFrontSkillCatalog[listNum].skillTarget.SetTargetSort();
                disposableSimu = moveSimuSub.Subscribe(get =>
                {
                    skillListHolderSO.mFrontSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate(get.target);

                });
                disposableBook = bookASub.Subscribe(async (get, ct) =>
                {
                    bookKeyPub.Publish(new BookCommonMoveKeyMessage(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillKey()));
                });

                ChangeSkillOnFrontPrepare();


            }
            else
            {

                disposableSimu?.Dispose();
                text.SetText(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());
                skillListHolderSO.mBackSkillCatalog[listNum].skillTarget.SetTargetSort();
                disposableSimu = moveSimuSub.Subscribe(get =>
                {
                    skillListHolderSO.mBackSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate(get.target);

                });
                disposableBook = bookASub.Subscribe(async (get, ct) =>
                {
                    bookKeyPub.Publish(new BookCommonMoveKeyMessage(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillKey()));
                });

                ChangeSkillOnBackPrepare();

            }
        }).AddTo(bag);



        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableInput?.Dispose();
        disposableOnDestroy?.Dispose();
        disposableSkill?.Dispose();
    }

    private async UniTask SelectThisComponent()
    {

        //順番大事
        selectDispPub.Publish(inputLayerSO, new DisposeSelect());

        //現在走っているPublishを受け入れないために1frame待つ
        await UniTask.NextFrame();

        image.sprite = sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();






        upSubscriber.Subscribe(inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        downSubscriber.Subscribe(inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);
        enterSub.Subscribe(inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);

        selectDispSub.Subscribe(inputLayerSO, i =>
        {
            UnselectThisComponent();
        }).AddTo(bag);


        


        disposableInput = bag.Build();
    }

    private void ChangeSkillOnFrontPrepare()
    {
        disposableSkill?.Dispose();

        var bag = DisposableBag.CreateBuilder();

        rightSubscriber.Subscribe(inputLayerSO, i => {
            AddListNumOnFront();
            text.SetText(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillName());

            skillListHolderSO.mFrontSkillCatalog[listNum].skillTarget.SetTargetSort();
            //skillListHolderSO.mFrontSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate();


        }).AddTo(bag);

        leftSubscriber.Subscribe(inputLayerSO, i =>
        {
            DeductListNumOnFront();
            text.SetText(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillName());

            skillListHolderSO.mFrontSkillCatalog[listNum].skillTarget.SetTargetSort();
            //skillListHolderSO.mFrontSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate();


        }).AddTo(bag);

        disposableSkill = bag.Build();
    }

    private void ChangeSkillOnBackPrepare()
    {
        disposableSkill?.Dispose();

        var bag = DisposableBag.CreateBuilder();

        rightSubscriber.Subscribe(inputLayerSO, i => {
            AddListNumOnBack();
            text.SetText(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());

            skillListHolderSO.mBackSkillCatalog[listNum].skillTarget.SetTargetSort();
            //skillListHolderSO.mBackSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate();

            //Debug.Log(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());
        }).AddTo(bag);

        leftSubscriber.Subscribe(inputLayerSO, i =>
        {
            DeductListNumOnBack();
            text.SetText(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());

            skillListHolderSO.mBackSkillCatalog[listNum].skillTarget.SetTargetSort();
           // skillListHolderSO.mBackSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate();


        }).AddTo(bag);

        disposableSkill = bag.Build();

    }

    public async void OnPointerEnter(PointerEventData pointerEventData)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        try
        {
            await WaitForSecond(cts);
            if (currentPos == MovePosition.front)
            {
                descDemendPub.Publish(new DescriptionDemendMessage(skillListHolderSO.mFrontSkillCatalog[listNum].GetDescription()));

            }
            else
            {
                descDemendPub.Publish(new DescriptionDemendMessage(skillListHolderSO.mBackSkillCatalog[listNum].GetDescription()));

            }

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
        if (currentPos == MovePosition.front)
        {
            ChangeSkillOnFrontPrepare();
        }
        else
        {
            ChangeSkillOnBackPrepare();
        }

    }

    private async UniTask WaitForSecond(CancellationTokenSource cts)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: cts.Token);

    }


    private void AddListNumOnFront()
    {

        if (skillListHolderSO.mFrontSkillCatalog.Count - 1 != listNum)
        {
            listNum++;

        }
        else
        {
            listNum = 0;

        }

    }

    private void DeductListNumOnFront()
    {

        if (listNum != 0)
        {
            listNum--;
        }
        else
        {
            listNum = skillListHolderSO.mFrontSkillCatalog.Count - 1;
        }

    }

    private void AddListNumOnBack()
    {

        if (skillListHolderSO.mBackSkillCatalog.Count - 1 != listNum)
        {
            listNum++;

        }
        else
        {
            listNum = 0;

        }

    }

    private void DeductListNumOnBack()
    {

        if (listNum != 0)
        {
            listNum--;
        }
        else
        {
            listNum = skillListHolderSO.mBackSkillCatalog.Count - 1;
        }

    }

    private void UnselectThisComponent()
    {
        disposableInput?.Dispose();
        disposableSkill?.Dispose();
        image.sprite = sourceImageSO.offSelect;
    }

    private void NextUpSelect()
    {

        UnselectThisComponent();
        selectPublisher.Publish(new SelectMessage(inputLayerSO, upNum), new SelectChange());

    }

    private void NextDownSelect()
    {
        UnselectThisComponent();

        selectPublisher.Publish(new SelectMessage(inputLayerSO, downNum), new SelectChange());

    }
}
