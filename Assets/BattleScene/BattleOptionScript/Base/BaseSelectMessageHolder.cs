using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;


public class BaseSelectMessageHolder : MonoBehaviour
{
    [Inject] protected readonly ISubscriber<InputLayerSO, UpInput> upSubscriber;
    [Inject] protected readonly ISubscriber<InputLayerSO, DownInput> downSubscriber;
    [Inject] protected readonly ISubscriber<InputLayerSO, RightInput> rightSubscriber;
    [Inject] protected readonly ISubscriber<InputLayerSO, LeftInput> leftSubscriber;

    [Inject] protected readonly ISubscriber<InputLayerSO, EnterInput> enterSub;

    [Inject] protected readonly IPublisher<InputLayerSO, DisposeSelect> selectDispPub;
    [Inject] protected readonly ISubscriber<InputLayerSO, DisposeSelect> selectDispSub;

    [SerializeField]
    protected InputLayerSO inputLayerSO;

    protected System.IDisposable disposableInput;

}
