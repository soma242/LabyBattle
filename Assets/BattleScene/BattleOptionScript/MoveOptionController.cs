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

    //�����̎��ʔԍ��Ǝ�������Ȃ��鎯�ʔԍ�
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

    //�A�^�b�`���ꂽ�I�u�W�F�N�g�̃C���[�W
    private Image image;

    private SkillListHolderSO skillListHolderSO;

    private MovePosition currentPos;

    //MessagePipe

    private CancellationTokenSource cts;
    private IPublisher<DescriptionDemendMessage> descDemendPub;
    private IPublisher<DescriptionFinishMessage> descFinishPub;

    private IPublisher<ASkillSimulateMessage> simulatePub;

    private IPublisher<MoveOptionChange> changePub;

    //select�ύX�pMessagePipe
    //BuiltInContainer
    private IPublisher<SelectMessage, SelectChange> selectPublisher;
    private ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    //SkillList�󂯓���
    private ISubscriber<SelectSkillListChangeMessage> skillListSub;

    private System.IDisposable disposableOnDestroy;

    private ISubscriber<MoveSimulateMessage> moveSimuSub;
    private System.IDisposable disposableSimu;


    private System.IDisposable disposableInput;


    

    private System.IDisposable disposableSkill;

    private IAsyncSubscriber<ActionSelectBookMessage> bookASub;
    private IPublisher<BookCommonMoveKeyMessage> bookKeyPub;

    private System.IDisposable disposableBook;


    //�֐�
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

        //SkillListSub�̑O�ɂ����ɓ����Ă��܂��Ă���B
        selectSubscriber.Subscribe(new SelectMessage(holder.inputLayerSO, myNum), i =>
        {
            
            disposableSkill?.Dispose();
            SelectThisComponent();

            //���E��Sub
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
        //���layerSO�̕ύX������Ɠ��I������ċ�������������������ۂ��H
        //���ԑ厖
        holder.selectDispPub.Publish(holder.inputLayerSO, new DisposeSelect());

        //���ݑ����Ă���Publish���󂯓���Ȃ����߂�1frame�҂�
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
                        // ������CancellationToken�������Ȃ̂ŁA�����ێ�����OperationCanceledException�Ƃ��ē�����
                        //throw new OperationCanceledException(ex.Message, ex, cts.Token);
                    }
                    else
                    {
                        // �^�C���A�E�g�������Ȃ̂ŁATimeoutException(�����͓Ǝ��̗�O)�Ƃ��ē�����
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
