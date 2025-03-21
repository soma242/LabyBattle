using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;
using SkillStruct;


//キーボードの入力に対してDebug.Logを返す。

public class InputSystemTester : MonoBehaviour
{
    private int count;

    [Inject] private readonly ISubscriber<InputLayerSO, RightInput> _RightInputSubscriber;
    [Inject] private readonly ISubscriber<InputLayerSO, LeftInput> _LeftInputSubscriber;
    [Inject] private readonly ISubscriber<InputLayerSO, UpInput> _UpInputSubscriber;
    [Inject] private readonly ISubscriber<InputLayerSO, DownInput> _DownInputSubscriber;
    [Inject] private readonly ISubscriber<InputLayerSO, EnterInput> _EnterInputSubscriber;
    [Inject] private readonly ISubscriber<InputLayerSO, CancelInput> _CancelInputSubscriber;

    //[Inject] private readonly ISubscriber<RightInput> RightInputSubscriber;

    [SerializeField] private InputLayerSO inputLayerSO;
    [SerializeField] private InputLayerSO logLayer;


    private System.IDisposable disposable;


    private IPublisher<DamageNoticeMessage> damagePub;


    // Start is called before the first frame update
    void OnEnable()
    {

        var bag = DisposableBag.CreateBuilder();

        damagePub = GlobalMessagePipe.GetPublisher<DamageNoticeMessage>();

        _RightInputSubscriber.Subscribe(logLayer, i => {
            //CheckRightInput();
            count++;
            //damagePub.Publish(new DamageNoticeMessage(true, FormationScope.FirstChara(), count));
        }).AddTo(bag);

        /*RightInputSubscriber.Subscribe( i => {
            CheckRightInput();
        }).AddTo(bag);*/

        _LeftInputSubscriber.Subscribe(inputLayerSO, i =>
        {
            //CheckLeftInput();
            //var unregistPub = GlobalMessagePipe.GetPublisher<sbyte, UnregistPassiveSkill>();
            //unregistPub.Publish(FormationScope.FirstChara(), new UnregistPassiveSkill());
        }).AddTo(bag);

        _UpInputSubscriber.Subscribe(inputLayerSO, i =>
        {
            //CheckUpInput();
            //var passiveStartPub = GlobalMessagePipe.GetPublisher<PassiveOnBattleStartMessage>();
            //passiveStartPub.Publish(new PassiveOnBattleStartMessage());
        }).AddTo(bag);

        _DownInputSubscriber.Subscribe(inputLayerSO, i => 
        {
            CheckDownInput();
        }).AddTo(bag);

        _EnterInputSubscriber.Subscribe(inputLayerSO, i =>
        {
            //CheckEnterInput();
        }).AddTo(bag);

        _CancelInputSubscriber.Subscribe(inputLayerSO, i =>
        {
            CheckCancelInput();
        }).AddTo(bag);

        disposable = bag.Build();

    }



    private void CheckRightInput()
    {
        Debug.Log("right");
    }

    private void CheckLeftInput()
    {
        Debug.Log("left");
    }

    private void CheckUpInput()
    {
        Debug.Log("up");
    }

    private void CheckDownInput()
    {
        Debug.Log("down");
    }

    private void CheckEnterInput()
    {
        Debug.Log("Enter");
    }

    private void CheckCancelInput()
    {
        Debug.Log("Cancel");
    }



    private void OnDisable()
    {
        disposable.Dispose();

    }
}
