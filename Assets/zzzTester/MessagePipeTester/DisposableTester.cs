using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

public class DisposableTester : MonoBehaviour
{

    [SerializeField] protected InputLayerSO inputLayerSO;

    [Inject] private ISubscriber<InputLayerSO, LeftInput> leftSub;
    [Inject] private ISubscriber<InputLayerSO, RightInput> rightSub;

    private System.IDisposable disposable;


    // Start is called before the first frame update
    void Awake()
    {
        disposable = leftSub.Subscribe(inputLayerSO, i =>
        {
            LeftGet();
        });
    }

    private void LeftGet()
    {
        Debug.Log("left");
        var d = rightSub.Subscribe(inputLayerSO, i =>
        {
            RightGet();
        });

        disposable = DisposableBag.Create(disposable, d);
    }

    private void RightGet()
    {
        Debug.Log("rightDis");
        disposable.Dispose();
    }

}
