using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using DungeonSceneMessage;

public class DungeonStartTester : MonoBehaviour
{

    [SerializeField]
    private DungeonMapDataSO mapData;



    [SerializeField]
    private int direction;
    [SerializeField]
    private bool horizon;
    //private DungeonPos firstPos;

    /*
    //byte[]����bitArray���Đ��������
    //���X�̏���ۂ��C8�̔{���ɑ���Ă��Ȃ��������͎����I�ɒǉ�����Ă��܂�
    //����̏ꍇ�͖������Ă悢�ibool�����������C�ǉ����ꂽ�����ɂ͐G��Ȃ��j
    [SerializeField]
    private BitArray[] myBA1 = new BitArray[1];
        //new BitArray(5, false);

    byte[] byteDate = new byte[2];
    */

    // Start is called before the first frame update
    void Start()
    {
        /*
        myBA1[1] = true;
        myBA1.CopyTo(byteDate, 0);
        foreach(var bit in myBA1)
        {
            Debug.Log(bit);
        }
        myBA1 = new BitArray(byteDate);
        Debug.Log("change");
        int count = 0;
        foreach (var bit in myBA1)
        {
            count++;
            Debug.Log(bit);
        }
        Debug.Log(count);
        //Debug.Log(myBA1[1]);
        //Debug.Log(myBA1[0]);
        */

        var mapPub = GlobalMessagePipe.GetPublisher<DungeonMapMessage>();
        mapPub.Publish(new DungeonMapMessage(mapData, new DungeonPos(1, 1), direction, horizon));
    }


}
