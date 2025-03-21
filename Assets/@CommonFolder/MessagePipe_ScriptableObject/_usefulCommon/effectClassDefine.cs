using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;
using BattleSceneMessage;

#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998// disable warning
 
//クラスとして追加，ここ(Effects)に追加，コンストラクタに追加，対応する計算式に追加
//実際に使用する効果の集まり
public class Effects
{
    public AttackBuff attackBuff;
    public MagicBuff magicBuff;
    public DefBuff defBuff;
    public AgiBuff agiBuff;

    public BreakePosture breakePosture;
    
    public Effects(sbyte formNum)
    {
        attackBuff = new AttackBuff();
        magicBuff = new MagicBuff();
        defBuff = new DefBuff();
        agiBuff = new AgiBuff();

        breakePosture = new BreakePosture(formNum);
    }

    public float GetPosture()
    {
        return breakePosture.GetEffect();
    }
    public float GetPosture(bool special)
    {
        return breakePosture.GetEffect(special);
    }
}


//ターン制限のバフデバフの基底クラス
public class BaseFloatMultiEffect
{
    public float value { get; private set; }

    public int remaining { get; private set; }

    protected IAsyncSubscriber<BattleSceneMessage.TurnEndMessage> turnEndASub;
    protected IAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage> prepareASub;

    protected System.IDisposable disposeStart;
    protected System.IDisposable disposable;

    //コンストラクタ
    public BaseFloatMultiEffect()
    {
        //掛け算なので初期値は1
        this.value = 1.0f;
        this.remaining = 0;

        this.turnEndASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.TurnEndMessage>();
        this.prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage>();


        disposeStart = prepareASub.Subscribe(async (get,ct) =>
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
        disposeStart?.Dispose();
        disposable?.Dispose();
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
        }
    }

    public void ResetValue()
    {
        value = 1.0f;
        remaining = 0;
        disposable?.Dispose();

    }


}

public class BaseBoolFloatEffect
{
    public bool valid { get; protected set; }
    //public float value { get; private set; }
    public int remaining { get; protected set; }

    protected IAsyncSubscriber<BattleSceneMessage.TurnEndMessage> turnEndASub;
    protected IAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage> prepareASub;

    protected System.IDisposable disposeStart;
    protected System.IDisposable disposeValid;
    protected System.IDisposable disposable;

    public BaseBoolFloatEffect()
    {
        valid = false;
        this.remaining = 0;

        this.turnEndASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.TurnEndMessage>();
        this.prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage>();

        disposeStart = prepareASub.Subscribe(async (get, ct) =>
        {
            ResetValid();

        });
    }

    ~BaseBoolFloatEffect()
    {
        //Debug.Log("finally");
        disposeStart?.Dispose();
        disposeValid?.Dispose();
        disposable?.Dispose();
    }



    protected void RemainingCheck()
    {
        remaining--;
        if (remaining < 1)
        {
            ResetValid();
        }
    }


    public void ResetValid()
    {
        valid = false;
        disposable?.Dispose();
    }
}

public class BreakePosture: BaseBoolFloatEffect
{
    public BreakePosture(sbyte formNum)
    {
        var breakSub = GlobalMessagePipe.GetSubscriber<sbyte, BreakePostureMessage>();
        disposeValid = breakSub.Subscribe(formNum, info =>
        {
            Debug.Log("break");
            valid = true;
            remaining = 1;
            disposable = turnEndASub.Subscribe(async (info, ct) =>
            {
                RemainingCheck();
            });

        });
    }

    public float GetEffect()
    {
        Debug.Log(valid);
        if (valid)
        {
            return 1.5f;
        }
        else
        {
            return 1.0f;
        }
    }

    public float GetEffect(bool special)
    {
        if (valid)
        {
            return 2.0f;
        }
        else
        {
            return 1.0f;
        }
    }

}

public class AttackBuff : BaseFloatMultiEffect
{

}

public class MagicBuff : BaseFloatMultiEffect
{

}

public class DefBuff : BaseFloatMultiEffect
{

}

public class AgiBuff: BaseFloatMultiEffect
{

}