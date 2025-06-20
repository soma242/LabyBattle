using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

public class FormCharaTester : MonoBehaviour
{
    private IPublisher<FormCharacterBootMessage> testPub;
    [SerializeField] private List<CharacterDataSO> charaList = new List<CharacterDataSO>();
    sbyte i;

    void Awake()
    {
        testPub = GlobalMessagePipe.GetPublisher<FormCharacterBootMessage>();
    }

    void Start()
    {
        i = 1;
        foreach(CharacterDataSO chara in charaList)
        {
            testPub.Publish(new FormCharacterBootMessage(chara.charaKey.charaNum, i));
            i++;
        }
    }


}
