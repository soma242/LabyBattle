using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonVariable
{
    public static string NoneString = "";
}

public struct SelectChange { }

public struct SelectMessage
{
    public InputLayerSO inputLayerSO { get; }
    public int selectNum;

    public SelectMessage(InputLayerSO inputLayerSO, int selectNum)
    {
        this.inputLayerSO = inputLayerSO;
        this.selectNum = selectNum;

    }
}

public struct RightInput { }
public struct LeftInput { }
public struct UpInput { }
public struct DownInput { }
public struct EnterInput { }
public struct CancelInput { }

public struct MapInput { }
public struct MenuInput { }

    public class ScrollInput
{
    public Vector2 value;
    //+‚È‚çtrue
    public bool sign;
    public ScrollInput(float value)
    {
        if (value < 0)
        {
            this.value = new Vector2(0, 55);
            sign = true;
        }
        else
        {
            this.value = new Vector2(0, -55);
            sign = false;
        }
    }
}

public struct Holdout { }

public struct DisposeSelect { }


