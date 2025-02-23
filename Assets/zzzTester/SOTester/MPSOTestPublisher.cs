using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

public class MPSOTestPublisher : MonoBehaviour
{

    private IPublisher<BattleStartMessage> testPublisher;


    void Start()
    {
        testPublisher = GlobalMessagePipe.GetPublisher<BattleStartMessage>();
    }

    // Start is called before the first frame update
    void Update()
    {
        //Debug.Log("tick");
        //testPublisher.Publish(new BattleStartMessage());
    }

    
}
