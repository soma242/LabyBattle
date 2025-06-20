using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

public class AdvancedMapSetter : MonoBehaviour
{
    [SerializeField]
    private MSO_DungeonMapCommander mapCommander;
    void Awake()
    {
        var mapPub = GlobalMessagePipe.GetPublisher<DungeonMapMessage>();
        mapPub.Publish(mapCommander.settedMap);
    }
}
