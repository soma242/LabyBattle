using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998// disable warning
 
//クラスとして追加，ここに追加，コンストラクタに追加，対応する計算式に追加
public class Effects
{
    public AttackBuff attackBuff;
    public DefBuff defBuff;
    public AgiBuff agiBuff;
    
    public Effects()
    {
        attackBuff = new AttackBuff();
        defBuff = new DefBuff();
        agiBuff = new AgiBuff();
    }
}


//ターン制限のバフデバフの基底クラス
public class BaseFloatMultiEffect
{
    public float value { get; private set; }

    public int remaining { get; private set; }

    protected IAsyncSubscriber<BattleSceneMessage.TurnEndMessage> turnEndASub;
    protected IAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage> prepareASub;

    protected System.IDisposable disposeOnDestroy;
    protected System.IDisposable disposable;

    //コンストラクタ
    public BaseFloatMultiEffect()
    {
        //掛け算なので初期値は1
        this.value = 1.0f;
        this.remaining = 0;

        this.turnEndASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.TurnEndMessage>();
        this.prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage>();


        disposeOnDestroy = prepareASub.Subscribe(async (get,ct) =>
        {
            //Debug.Log("StartBattle");
            ResetValue();
        });
        
    }

    //ファイナライザ
    //次回開始時に呼ばれている。
    ~BaseFloatMultiEffect()
    {
        //Debug.Log("finally");
        disposeOnDestroy.Dispose();
        disposable.Dispose();
    }


    //関数

    //数値の変更（バフデバフの付与）
    //=>ターン経過のカウント開始
    public void SetValue(float value, int remaining)
    {
        this.value = value;
        this.remaining = remaining;
        SetSubscriber();
    }

    protected void SetSubscriber()
    {
        disposable = turnEndASub.Subscribe(async (get,ct) =>
        {
            //Debug.Log("turnEnd");

            RemainingCheck();
        });
    }

    protected void RemainingCheck()
    {
        remaining--;
        if (remaining < 1)
        {
            ResetValue();
            disposable.Dispose();
        }
    }

    public void ResetValue()
    {
        value = 1.0f;
        remaining = 0;
    }


}

public class AttackBuff : BaseFloatMultiEffect
{

}

public class DefBuff : BaseFloatMultiEffect
{

}

public class AgiBuff: BaseFloatMultiEffect
{

}