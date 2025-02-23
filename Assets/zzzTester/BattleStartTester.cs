using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using Cysharp.Threading.Tasks;


using BattleSceneMessage;
using SkillStruct;

public class BattleStartTester : MonoBehaviour
{

    private IPublisher<BattleStartMessage> battleStartPub;
    private IPublisher<BattleSceneMessage.TurnEndMessage> turnEndPub;
    private IAsyncPublisher<sbyte, RegistSkillStart> registAPub;



    void Start()
    {
        battleStartPub = GlobalMessagePipe.GetPublisher<BattleStartMessage>();
        //turnEndPub = GlobalMessagePipe.GetPublisher<TurnEndMessage>();
        battleStartPub.Publish(new BattleStartMessage());
        //turnEndPub.Publish(new TurnEndMessage());

        registAPub = GlobalMessagePipe.GetAsyncPublisher<sbyte, RegistSkillStart>();
        registAPub.Publish(1, new RegistSkillStart());
    }


}
