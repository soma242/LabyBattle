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
    //byte[]からbitArrayを再生成する際
    //元々の順を保つが，8の倍数に足りていなかった分は自動的に追加されてしまう
    //今回の場合は無視してよい（boolよりも小さく，追加された部分には触れない）
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
