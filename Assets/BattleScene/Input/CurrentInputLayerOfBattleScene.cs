using UnityEngine;

using MessagePipe;



using BattleSceneMessage;


//#####################一つしか作っちゃダメ。別のシーンで用いるならここからコピーしてusing BattleSceneMessage;をそのシーンのnamespace変更

public class CurrentInputLayerOfBattleScene : MonoBehaviour
{
    private IPublisher<InputLayerSO, InputLayerChanged> inputLayerChangedPublisher;
    private ISubscriber<InputLayer> inputLayerChangeSubscriber;

    private System.IDisposable disposable;

    public InputLayerSO inputLayerSO;


    void Awake()
    {
        var bag = DisposableBag.CreateBuilder();

        //Debug.Log("CurrentInputLayerSO");

        //受け取ったInputLayerのfirstSelectを選択するためのPublish。
        inputLayerChangedPublisher = GlobalMessagePipe.GetPublisher<InputLayerSO, InputLayerChanged>();


        inputLayerChangeSubscriber = GlobalMessagePipe.GetSubscriber<InputLayer>();

        //inputLayerの変更を受け取った時，受け取ったInputLayerのfirstSelectを選択するためにPublishを行う。
        inputLayerChangeSubscriber.Subscribe(i => {
            inputLayerSO = i.inputLayerSO;
            //Debug.Log("receive: InputLayerChange = " + i.inputLayerSO);
            inputLayerChangedPublisher.Publish(i.inputLayerSO, new InputLayerChanged());

        }).AddTo(bag);

        disposable = bag.Build();
    }



    public void OnDestroy()
    {

        disposable.Dispose();
    }
}
