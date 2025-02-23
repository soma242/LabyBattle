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

public class MoveOptionController : MonoBehaviour
{
    private int listNum;

    //Layer
    [SerializeField]
    protected InputLayerSO inputLayerSO;

    [SerializeField]
    protected TMP_Text text;

    //自分の識別番号と自分からつながる識別番号
    [SerializeField]
    protected int myNum;
    [SerializeField]
    protected int upNum;
    [SerializeField]
    protected int downNum;

    [SerializeField]
    protected SelectSourceImageSO sourceImageSO;

    //アタッチされたオブジェクトのイメージ
    protected Image image;

    protected SkillListHolderSO skillListHolderSO;
    protected MovePosition currentPos;

    //MessagePipe

    //select変更用MessagePipe
    //BuiltInContainer
    protected IPublisher<SelectMessage, SelectChange> selectPublisher;
    protected ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    //SkillList受け入れ
    protected ISubscriber<SelectSkillListChangeMessage> skillListSub;

    protected System.IDisposable disposableOnDestroy;



    //Input受け入れ用MessagePipe
    //VContainer
    [Inject] protected readonly ISubscriber<InputLayerSO, UpInput> upSubscriber;
    [Inject] protected readonly ISubscriber<InputLayerSO, DownInput> downSubscriber;
    [Inject] protected readonly ISubscriber<InputLayerSO, RightInput> rightSubscriber;
    [Inject] protected readonly ISubscriber<InputLayerSO, LeftInput> leftSubscriber;

    protected System.IDisposable disposableInput;

    //関数
    protected void Awake()
    {
        image = GetComponent<Image>();

        listNum = 0;

        //BuiltInContainer
        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        skillListSub = GlobalMessagePipe.GetSubscriber<SelectSkillListChangeMessage>();

        var bag = DisposableBag.CreateBuilder();

        selectSubscriber.Subscribe(new SelectMessage(inputLayerSO, myNum), i =>
        {
            SelectThisComponent();
        }).AddTo(bag);

        skillListSub.Subscribe(get =>
        {
            listNum = 0;
            skillListHolderSO = get.skillListHolderSO;
            currentPos = get.currentPos;
            if(currentPos == MovePosition.front)
            {
                text.SetText(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillName());
            }
            else
            {
                text.SetText(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());
            }
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy.Dispose();
    }

    protected async UniTask SelectThisComponent()
    {

        image.sprite = sourceImageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();

        //現在走っているPublishを受け入れないために1frame待つ
        await UniTask.NextFrame();


        upSubscriber.Subscribe(inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        downSubscriber.Subscribe(inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);

        if(currentPos == MovePosition.front)
        {
            rightSubscriber.Subscribe(inputLayerSO, i => {
                AddListNumOnFront();
                text.SetText(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillName());
            }).AddTo(bag);

            leftSubscriber.Subscribe(inputLayerSO, i =>
            {
                DeductListNumOnFront();
                text.SetText(skillListHolderSO.mFrontSkillCatalog[listNum].GetSkillName());
            }).AddTo(bag);
        }
        else
        {
            rightSubscriber.Subscribe(inputLayerSO, i => {
                AddListNumOnBack();
                text.SetText(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());
            }).AddTo(bag);

            leftSubscriber.Subscribe(inputLayerSO, i =>
            {
                DeductListNumOnBack();
                text.SetText(skillListHolderSO.mBackSkillCatalog[listNum].GetSkillName());
            }).AddTo(bag);
        }
        


        disposableInput = bag.Build();
    }

    protected void AddListNumOnFront()
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

    protected void DeductListNumOnFront()
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

    protected void AddListNumOnBack()
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

    protected void DeductListNumOnBack()
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

    protected void UnselectThisComponent()
    {
        disposableInput.Dispose();
        image.sprite = sourceImageSO.offSelect;
    }

    protected void NextUpSelect()
    {

        UnselectThisComponent();
        selectPublisher.Publish(new SelectMessage(inputLayerSO, upNum), new SelectChange());

    }

    protected void NextDownSelect()
    {
        UnselectThisComponent();

        selectPublisher.Publish(new SelectMessage(inputLayerSO, downNum), new SelectChange());

    }
}
