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

using BattleSceneMessage;
using SkillStruct;

public class ActionTargetController : MonoBehaviour
{
    private int listNum;

    //Layer
    [SerializeField]
    private InputLayerSO inputLayerSO;

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

    private System.IDisposable disposableOnDestroy;



    //Input�󂯓���pMessagePipe
    [Inject] private readonly ISubscriber<InputLayerSO, UpInput> upSubscriber;
    [Inject] private readonly ISubscriber<InputLayerSO, DownInput> downSubscriber;
    [Inject] private readonly ISubscriber<InputLayerSO, RightInput> rightSubscriber;
    [Inject] private readonly ISubscriber<InputLayerSO, LeftInput> leftSubscriber;

    private System.IDisposable disposableInput;

    void Awake()
    {
        image = GetComponent<Image>();

        listNum = 0;

        //BuiltInContainer
        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();


        var bag = DisposableBag.CreateBuilder();

        selectSubscriber.Subscribe(new SelectMessage(inputLayerSO, myNum), i =>
        {
            SelectThisComponent();
        }).AddTo(bag);

        

        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy.Dispose();
    }

    private async UniTask SelectThisComponent()
    {

        image.sprite = sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();

        //���ݑ����Ă���Publish���󂯓���Ȃ����߂�1frame�҂�
        await UniTask.NextFrame();


        upSubscriber.Subscribe(inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        downSubscriber.Subscribe(inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);

        rightSubscriber.Subscribe(inputLayerSO, i => {
            AddListNum();
            //text.SetText(skillListHolderSO.aSkillCatalog[listNum].GetSkillName());
        }).AddTo(bag);

        leftSubscriber.Subscribe(inputLayerSO, i =>
        {
            DeductListNum();
            //text.SetText(skillListHolderSO.aSkillCatalog[listNum].GetSkillName());
        }).AddTo(bag);


        disposableInput = bag.Build();
    }

    private void UnselectThisComponent()
    {
        disposableInput.Dispose();
        image.sprite = sourceImageSO.offSelect;
    }

    private void AddListNum()
    {



    }

    private void DeductListNum()
    {



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
