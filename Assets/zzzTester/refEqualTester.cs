using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class refEqualTester : MonoBehaviour
{
    [SerializeField] private MSO_CharacterDataSO charaData;

    void Start()
    {
        charaData.FormingChara(1);
        Debug.Log("testStart");
    }
}
