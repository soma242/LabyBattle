/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;



//InputLayer–ˆ‚É—pˆÓ
public class FirstSelectCommander_BattleScene : MonoBehaviour
{
    public int firstNumber;
    private SelectMessage selectMessage;
    [Inject] private IPublisher<SelectMessage, SelectChange> firstSelectPublisher;


    //InputLayer•ÏXó‚¯æ‚è—pMessagePipe
    public InputLayerSO inputLayerSO;
    [Inject] private readonly ISubscriber<InputLayerChange> inputLayerChangeSubscriber;
    private System.IDisposable disposable;

    void OnEnable()
    {
        var bag = DisposableBag.CreateBuilder();

        inputLayerChangeSubscriber.Subscribe(i => {
            FirstSelect();
        }).AddTo(bag);

        disposable = bag.Build();
    }

    void Destroy()
    {
        disposable.Dispose();
    }

    private void FirstSelect()
    {
        firstSelectPublisher.Publish(new SelectMessage(inputLayer, firstNumber), new SelectChange());
    }
}
*/