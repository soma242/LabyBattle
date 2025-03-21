using UnityEngine;

using MessagePipe;


using BattleSceneMessage;


public class InputLayerChangeTester : MonoBehaviour
{
    private  IPublisher<InputLayerSO, InputLayerChanged> inputLayerChangePublisher;

    [SerializeField] private InputLayerSO inputLayerSO;

    void Awake()
    {
        inputLayerChangePublisher = GlobalMessagePipe.GetPublisher<InputLayerSO, InputLayerChanged>();
    }

    void Start()
    {
        Debug.Log("change");
        inputLayerChangePublisher.Publish(inputLayerSO, new InputLayerChanged());
    }
}
