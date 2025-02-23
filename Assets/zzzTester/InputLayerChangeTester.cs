using UnityEngine;

using MessagePipe;


using BattleSceneMessage;


public class InputLayerChangeTester : MonoBehaviour
{
    private  IPublisher<InputLayer, InputLayerChanged> inputLayerChangePublisher;

    [SerializeField] private InputLayerSO inputLayerSO;

    void Awake()
    {
        inputLayerChangePublisher = GlobalMessagePipe.GetPublisher<InputLayer, InputLayerChanged>();
    }

    void Start()
    {
        Debug.Log("change");
        inputLayerChangePublisher.Publish(new InputLayer(inputLayerSO), new InputLayerChanged());
    }
}
