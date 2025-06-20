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


public class MoveOptionController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int listNum;

    private int listMax;

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

    private MovePosition currentPos;

    //MessagePipe

    private CancellationTokenSource cts;
    private IPublisher<DescriptionDemendMessage> descDemendPub;
    private IPublisher<DescriptionFinishMessage> descFinishPub;

    private IPublisher<ASkillSimulateMessage> simulatePub;

    private IPublisher<MoveOptionChange> changePub;

    //select変更用MessagePipe
    //BuiltInContainer
    private IPublisher<SelectMessage, SelectChange> selectPublisher;
    private ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    //SkillList受け入れ
    private ISubscriber<SelectSkillListChangeMessage> skillListSub;

    private System.IDisposable disposableOnDestroy;

    private ISubscriber<MoveSimulateMessage> moveSimuSub;
    private System.IDisposable disposableSimu;


    private System.IDisposable disposableInput;


    

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


        changePub = GlobalMessagePipe.GetPublisher<MoveOptionChange>();



        var bag = DisposableBag.CreateBuilder();

        //SkillListSubの前にここに入ってしまっている。
        selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
        {
            
            disposableSkill?.Dispose();
            SelectThisComponent();

            //左右のSub
            if (currentPos == MovePosition.front)
            {
                //
                ChangeSkillOnFrontPrepare();


            }
            else
            {
                ChangeSkillOnBackPrepare();
                
            }
            

        }).AddTo(bag);

        skillListSub.Subscribe(get =>
        {
            Debug.Log("listSub");
            listNum = 0;
            skillListHolderSO = get.skillListHolderSO;
            currentPos = get.currentPos;

            disposableSimu?.Dispose();
            disposableBook?.Dispose();

            if (currentPos == MovePosition.front)
            {
                listMax = skillListHolderSO.mFrontSkillCatalog.Count - 1;

                text.SetText(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillName());
                skillListHolderSO.mFrontSkillCatalog[listNum].skillTarget.SetTargetSort();
                disposableSimu = moveSimuSub.Subscribe(get =>
                {
                    skillListHolderSO.mFrontSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate(get.target);

                });
                disposableBook = bookASub.Subscribe(async (get, ct) =>
                {
                    Debug.Log(listNum);
                    bookKeyPub.Publish(new BookCommonMoveKeyMessage(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillKey()));
                });

                ChangeSkillOnFrontPrepare();


            }
            else
            {
                listMax = skillListHolderSO.mBackSkillCatalog.Count - 1;


                text.SetText(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());
                skillListHolderSO.mBackSkillCatalog[listNum].skillTarget.SetTargetSort();
                disposableSimu = moveSimuSub.Subscribe(get =>
                {
                    skillListHolderSO.mBackSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate(get.target);

                });
                disposableBook = bookASub.Subscribe(async (get, ct) =>
                {
                    Debug.Log(listNum);
                    bookKeyPub.Publish(new BookCommonMoveKeyMessage(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillKey()));
                });

                ChangeSkillOnBackPrepare();

            }
        }).AddTo(bag);



        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        cts?.Cancel();
        disposableBook?.Dispose();
        disposableSimu?.Dispose();
        disposableInput?.Dispose();
        disposableOnDestroy?.Dispose();
        disposableSkill?.Dispose();
    }

    private async UniTask SelectThisComponent()
    {
        //二回layerSOの変更が入ると二回選択されて挙動がおかしく成るっぽい？
        //順番大事
        holder.selectDispPub.Publish(holder.inputLayerSO, new DisposeSelect());

        //現在走っているPublishを受け入れないために1frame待つ
        await UniTask.NextFrame();

        //disposableInput?.Dispose();


        image.sprite = sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();



        //var downSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, DownInput>();


        holder.upSub.Subscribe(holder.inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        holder.downSub.Subscribe(holder.inputLayerSO, i => {
            //Debug.Log("downMO");

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

    private void ChangeSkillOnFrontPrepare()
    {
        disposableSkill?.Dispose();

        var bag = DisposableBag.CreateBuilder();

        holder.rightSub.Subscribe(holder.inputLayerSO, i => {
            AddListNum();
            text.SetText(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillName());

            skillListHolderSO.mFrontSkillCatalog[listNum].skillTarget.SetTargetSort();
            //skillListHolderSO.mFrontSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate();


        }).AddTo(bag);

        holder.leftSub.Subscribe(holder.inputLayerSO, i =>
        {
            DeductListNum();
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

        holder.rightSub.Subscribe(holder.inputLayerSO, i => {
            AddListNum();
            text.SetText(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());

            skillListHolderSO.mBackSkillCatalog[listNum].skillTarget.SetTargetSort();
            //skillListHolderSO.mBackSkillCatalog[listNum].skillEffectSO.MoveSkillSimulate();

            //Debug.Log(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());
        }).AddTo(bag);

        holder.leftSub.Subscribe(holder.inputLayerSO, i =>
        {
            DeductListNum();
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


    private void AddListNum()
    {
        changePub.Publish(new MoveOptionChange());
        if (listMax != listNum)
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
        changePub.Publish(new MoveOptionChange());

        if (listNum != 0)
        {
            listNum--;
        }
        else
        {
            listNum = listMax;
        }

    }


    private void UnselectThisComponent()
    {
        //Debug.Log("disp");

        disposableInput?.Dispose();
        disposableSkill?.Dispose();
        image.sprite = sourceImageSO.offSelect;
    }

    private void NextUpSelect()
    {

        UnselectThisComponent();
        selectPublisher.Publish(new SelectMessage(holder.inputLayerSO, upNum), new SelectChange());

    }

    private void NextDownSelect()
    {
        UnselectThisComponent();

        selectPublisher.Publish(new SelectMessage(holder.inputLayerSO, downNum), new SelectChange());

    }
}
