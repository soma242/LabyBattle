using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JagArrayContTester : MonoBehaviour
{
    public JagArrayTester array;
    public DungeonMapDataSO test;

    void Awake()
    {
        //test.Testervoid();
        Debug.Log(test);
    }
    
    /*
    void Awake()
    {
        //������������Null�ɂȂ�̂ŕۑ��͏o���ĂȂ�
        MapDataInject(array);

        foreach (bool item in array.map[0])
        {
            Debug.Log(item);
        }
        //Debug.Log(array.map[0]);
    }

    private void MapDataInject(JagArrayTester array)
    {
        array.map = new bool[2][];
        array.map[0] = new bool[] { IToB(0), IToB(0), IToB(0), IToB(0), IToB(0), IToB(0), IToB(0), IToB(0), IToB(0), IToB(0), IToB(0), IToB(0) };
        //array.map[1] =  [0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0]
    }

    private bool IToB(int num)
    {
        return System.Convert.ToBoolean(num);
    }
    */
}
