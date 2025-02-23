using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

public class DisposeSelectCommander_BattleScene : MonoBehaviour
{
    [Inject] private readonly IPublisher<DisposeSelect> disposeSelectPublisher;

    void OnApplicationQuit()
    {
        disposeSelectPublisher.Publish(new DisposeSelect());
    }
}
