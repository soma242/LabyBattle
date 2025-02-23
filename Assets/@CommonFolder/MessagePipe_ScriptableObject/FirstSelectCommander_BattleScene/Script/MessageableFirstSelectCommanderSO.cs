using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

//�V�[�����Ƃɂ���namespace��ύX���邱�ƁB
using BattleSceneMessage;



[CreateAssetMenu(menuName = "MessageableSO/Component/FirstSelectCommanderSO")]
public class MessageableFirstSelectCommanderSO : MessageableScriptableObject
{
    public int firstNumber;
    //private SelectMessage selectMessage;
    private IPublisher<SelectMessage, SelectChange> firstSelectPublisher;


    //InputLayer�ύX�󂯎��pMessagePipe
    [SerializeField] private InputLayerSO inputLayerSO;
    private ISubscriber<InputLayer, InputLayerChanged> inputLayerChangeSubscriber;
    private System.IDisposable disposable;

    

    public override void MessageStart()
    {
        Debug.Log("MessageStart");


        //publisher
        firstSelectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();

        //subscribe
        inputLayerChangeSubscriber = GlobalMessagePipe.GetSubscriber<InputLayer, InputLayerChanged>();

        var bag = DisposableBag.CreateBuilder();

        inputLayerChangeSubscriber.Subscribe(new InputLayer(inputLayerSO), i =>
        {
            //Debug.Log("test is OK");
            FirstSelect();
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

    private void FirstSelect()
    {
        firstSelectPublisher.Publish(new SelectMessage(inputLayerSO, firstNumber), new SelectChange());
    }
}
