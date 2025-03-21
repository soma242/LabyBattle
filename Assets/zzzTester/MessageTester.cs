using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

public class MessageTester : MonoBehaviour
{
    //スキル使用
    private IPublisher<ActiveSkillCommand> commandPub;

    [SerializeField] private MSO_ActiveSkillSO skill;
    [SerializeField] private MSO_FormationCharaSO fromSO;
    //private sbyte to = 50;

    void Awake()
    {
        commandPub = GlobalMessagePipe.GetPublisher<ActiveSkillCommand>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("pub");
        //commandPub.Publish(new ActiveSkillCommand(skill.activeKey.activeNum, new ActiveSkillPosition(fromSO, to)));
    }
}
