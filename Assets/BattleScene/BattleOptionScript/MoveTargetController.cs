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



public class MoveTargetController : BaseSelectMessageHolder, IPointerClickHandler
{
    private sbyte targetPos;

    private bool targeting;


    [SerializeField]
    private TMP_Text text;

    //�����̎��ʔԍ��Ǝ�������Ȃ��鎯�ʔԍ�
    [SerializeField]
    private int myNum;
    [SerializeField]
    private int upNum;
    [SerializeField]
    private int downNum;

    [SerializeField]
    private SelectSourceImageSO sourceImageSO;

    //�A�^�b�`���ꂽ�I�u�W�F�N�g�̃C���[�W
    private Image image;

    //MessagePipe

    //select�ύX�pMessagePipe
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

        //Targeting�̎�ނɂ���ď�����ς���
        //From_MoveSkillTargetSO
        noneTargetSub.Subscribe(get =>
        {
            disposableSelect?.Dispose();
            targetPos = FormationScope.NoneChara();
            targeting = false;

            text.SetText(FormationScope.NoneTargetText());
            moveSimuPub.Publish(new MoveSimulateMessage(FormationScope.NoneChara()));
            disposableSelect = selectSubscriber.Subscribe(new SelectMessage(inputLayerSO, myNum), i =>
            {
                SelectThisComponent();
            });
        }).AddTo(bag);

        singleEnemySub.Subscribe(infom =>
        {
            disposableSelect?.Dispose();
            targetPos = FormationScope.FirstEnemy();
            targeting = true;

            moveSimuPub.Publish(new MoveSimulateMessage(targetPos));
            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                disposableReturn?.Dispose();
            });
            //next,pre�͎󂯎�葤�Ŏ��ɉ񂷂��߂̕����Ȃ̂ŁC���݂̂��̂��󂯎�肽����΂ǂ���ł�����
            //=>�t��Publish�����Ŏ��ɂ͂����Ă���Ȃ�

            disposableSelect = selectSubscriber.Subscribe(new SelectMessage(inputLayerSO, myNum), i =>
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
            //targetPos�̍X�V�̃^�C�~���O��Sub���Ȃ̂ŘA�����͂����Ȃ��͂��i�����ł�InputSystem�Ȃ̂ŒZ���Ԃ̘A�����͕͂s�j
            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                disposableReturn?.Dispose();
            });
            //next,pre�͎󂯎�葤�Ŏ��ɉ񂷂��߂̕����Ȃ̂ŁC���݂̂��̂��󂯎�肽����΂ǂ���ł�����
            //=>�t��Publish�����Ŏ��ɂ͂����Ă���Ȃ�
            nextTargetPub.Publish(targetPos, new GetNextTargetName());

            disposableSelect = selectSubscriber.Subscribe(new SelectMessage(inputLayerSO, myNum), i =>
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
        selectDispPub.Publish(inputLayerSO, new DisposeSelect());


        //���ݑ����Ă���Publish���󂯓���Ȃ����߂�1frame�҂�
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

    private void ChangeTargetPrepare()
    {
        var bag = DisposableBag.CreateBuilder();

        rightSubscriber.Subscribe(inputLayerSO, i => {
            tSimuCancellPub.Publish(new TauntSimulateCancell());
            //Debug.Log(targetPos);
            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                moveSimuPub.Publish(new MoveSimulateMessage(targetPos));

                //Debug.Log("sub");
                disposableReturn?.Dispose();
            });

            nextTargetPub.Publish(NextFormNum(targetPos), new GetNextTargetName());
            //await UniTask.Delay(TimeSpan.FromSeconds(3f));
            //Debug.Log(NextFormNum(targetPos));

        }).AddTo(bag);

        leftSubscriber.Subscribe(inputLayerSO, i =>
        {
            tSimuCancellPub.Publish(new TauntSimulateCancell());
            disposableReturn = returnTargetSub.Subscribe(get =>
            {
                targetPos = get.targetPos;
                text.SetText(get.targetName);
                moveSimuPub.Publish(new MoveSimulateMessage(targetPos));

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

        UnselectThisComponent();
        selectPublisher.Publish(new SelectMessage(inputLayerSO, upNum), new SelectChange());

    }

    private void NextDownSelect()
    {
        UnselectThisComponent();

        selectPublisher.Publish(new SelectMessage(inputLayerSO, downNum), new SelectChange());

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


