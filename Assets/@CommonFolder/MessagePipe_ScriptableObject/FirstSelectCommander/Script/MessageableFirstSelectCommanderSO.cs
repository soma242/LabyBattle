using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

//シーンごとにこのnamespaceを変更すること。
using BattleSceneMessage;



[CreateAssetMenu(menuName = "MessageableSO/Component/FirstSelectCommanderSO")]
public class MessageableFirstSelectCommanderSO : MessageableScriptableObject
{
    public int firstNumber;
    //private SelectMessage selectMessage;
    //private IPublisher<SelectMessage, SelectChange> firstSelectPublisher;


    //InputLayer変更受け取り用MessagePipe
    [SerializeField] private InputLayerSO inputLayerSO;
    private ISubscriber<InputLayerSO, InputLayerChanged> inputLayerChangeSubscriber;
    private System.IDisposable disposable;

    

    public override void MessageStart()
    {
        //Debug.Log("MessageStart");


        //publisher

        //subscribe
        inputLayerChangeSubscriber = GlobalMessagePipe.GetSubscriber<InputLayerSO, InputLayerChanged>();

        var bag = DisposableBag.CreateBuilder();

        inputLayerChangeSubscriber.Subscribe(inputLayerSO, i =>
        {
            var firstSelectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
            firstSelectPublisher.Publish(new SelectMessage(inputLayerSO, firstNumber), new SelectChange());
        }).AddTo(bag);

        disposable = bag.Build();


    }
    /*
    void OnEnable()
    {
        var bag = DisposableBag.CreateBuilder();

        inputLayerChangeSubscriber.Subscribe(inputLayerSO, i => {
            FirstSelect();
        }).AddTo(bag);

        disposable = bag.Build();
    }

    void Destroy()
    {
        disposable.Dispose();
    }
    */


}
