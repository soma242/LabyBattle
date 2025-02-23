using UnityEngine;

using MessagePipe;



using BattleSceneMessage;


//#####################������������_���B�ʂ̃V�[���ŗp����Ȃ炱������R�s�[����using BattleSceneMessage;�����̃V�[����namespace�ύX

public class CurrentInputLayerOfBattleScene : MonoBehaviour
{
    private  IPublisher<InputLayer, InputLayerChanged> inputLayerChangedPublisher;
    private  ISubscriber<InputLayer> inputLayerChangeSubscriber;

    private System.IDisposable disposable;

    public InputLayerSO inputLayerSO;


    void Awake()
    {
        var bag = DisposableBag.CreateBuilder();

        Debug.Log("CurrentInputLayerSO");

        //�󂯎����InputLayer��firstSelect��I�����邽�߂�Publish�B
        inputLayerChangedPublisher = GlobalMessagePipe.GetPublisher<InputLayer, InputLayerChanged>();


        inputLayerChangeSubscriber = GlobalMessagePipe.GetSubscriber<InputLayer>();

        //inputLayer�̕ύX���󂯎�������C�󂯎����InputLayer��firstSelect��I�����邽�߂�Publish���s���B
        inputLayerChangeSubscriber.Subscribe(i => {
            ChangeValue(i);
        }).AddTo(bag);

        disposable = bag.Build();
    }

    private void ChangeValue(InputLayer i)
    {
        //
        Debug.Log("receive: InputLayerChange = "+i.inputLayerSO);

        //���݂�InputLayer��ύX�B
        inputLayerSO = i.inputLayerSO;

        inputLayerChangedPublisher.Publish(new InputLayer(inputLayerSO), new InputLayerChanged());
    }

    public void OnDestroy()
    {

        disposable.Dispose();
    }
}
