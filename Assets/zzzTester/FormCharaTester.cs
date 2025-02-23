using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

public class FormCharaTester : MonoBehaviour
{
    private IPublisher<FormCharacterBootMessage> testPub;
    [SerializeField] private List<MSO_CharacterDataSO> charaList = new List<MSO_CharacterDataSO>();
    sbyte i;

    void Awake()
    {
        testPub = GlobalMessagePipe.GetPublisher<FormCharacterBootMessage>();
    }

    void Start()
    {
        i = 1;
        foreach(MSO_CharacterDataSO chara in charaList)
        {
            testPub.Publish(new FormCharacterBootMessage(chara.charaKey.charaNum, i));
            i++;
        }
    }


}
