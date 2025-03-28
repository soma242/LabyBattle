using UnityEngine;

using MessagePipe;



using BattleSceneMessage;


//#####################������������_���B�ʂ̃V�[���ŗp����Ȃ炱������R�s�[����using BattleSceneMessage;�����̃V�[����namespace�ύX

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

        //�󂯎����InputLayer��firstSelect��I�����邽�߂�Publish�B
        inputLayerChangedPublisher = GlobalMessagePipe.GetPublisher<InputLayerSO, InputLayerChanged>();


        inputLayerChangeSubscriber = GlobalMessagePipe.GetSubscriber<InputLayer>();

        //inputLayer�̕ύX���󂯎�������C�󂯎����InputLayer��firstSelect��I�����邽�߂�Publish���s���B
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
