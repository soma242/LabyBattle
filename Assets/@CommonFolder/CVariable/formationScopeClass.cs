using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FormationScope
{
    private static sbyte noneChara = 0;

    private static sbyte allChara = 21;
    private static sbyte frontChara = 22;
    private static sbyte backChara = 23;
    private static sbyte allEnemy = 25;


    private static sbyte firstChara = 1;
    private static sbyte lastChara = 4;


    private static sbyte firstEnemy = 11;
    private static sbyte lastEnemy = 13;

    private static string noneTargetString = "";
    private static string frontCharaString = "‘O‰q";


    public static sbyte NoneChara()
    {
        return noneChara;
    }

    public static sbyte AllChara()
    {
        return allChara;
    }
    public static sbyte FrontChara()
    {
        return frontChara;
    }
    public static sbyte BackChara()
    {
        return backChara;
    }
    public static sbyte AllEnemy()
    {
        return allEnemy;
    }


    public static sbyte FirstChara()
    {
        return firstChara;
    }
    public static sbyte LastChara()
    {
        return lastChara;
    }



    public static sbyte FirstEnemy()
    {
        return firstEnemy;
    }
    public static sbyte LastEnemy()
    {
        return lastEnemy;
    }


    /// <summary>
    /// string return
    /// </summary>
    /// <returns></returns>
    public static string FrontCharaText()
    {
        return frontCharaString;
    }

    public static string NoneTargetText()
    {
        return noneTargetString;
    }

    public static int FormToListChara(sbyte form)
    {
        int i = form - firstChara;
        return i;
    }
    public static int FormToListEnemy(sbyte form)
    {
        int i = form - firstEnemy;
        return i;
    }

}