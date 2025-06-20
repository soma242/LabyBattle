using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;
using BattleSceneMessage;

#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998// disable warning


//16byte��2=>1�ɁC��X�o�t�����Ȃǂ�Message�𑝂₷���Ƃ��l���Ď���
public class EffectMessagePipeHolder
{
    public IAsyncSubscriber<BattleFinishMessage> endASub;
    //public IAsyncSubscriber<BattlePrepareMessage> prepareASub;
    public IAsyncSubscriber<BattleSceneMessage.TurnEndMessage> turnEndASub;

    public EffectMessagePipeHolder()
    {
        endASub = GlobalMessagePipe.GetAsyncSubscriber<BattleFinishMessage>();
        //prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage>();
        turnEndASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.TurnEndMessage>();

    }

}

public enum BuffType
{
    attack,
    magic,
    def,
    agi
}

public static class EffectsString
{
    public static string AttackString = "�U��";
}

//�N���X�Ƃ��Ēǉ��C����(Effects)�ɒǉ��C�R���X�g���N�^�ɒǉ��C�Ή�����v�Z���ɒǉ�
//���ۂɎg�p������ʂ̏W�܂�
public class Effects 
{
    

    public AttackBuff attackBuff;
    public MagicBuff magicBuff;
    public DefBuff defBuff;
    public AgiBuff agiBuff;

    public BreakePosture breakePosture;


    private System.IDisposable disposable;

    public Effects(sbyte pos)
    {
        EffectMessagePipeHolder holder = new EffectMessagePipeHolder();

        attackBuff = new AttackBuff();
        attackBuff.holder = holder;
        magicBuff = new MagicBuff();
        magicBuff.holder = holder;
        defBuff = new DefBuff();
        defBuff.holder = holder;
        agiBuff = new AgiBuff();
        agiBuff.holder = holder;

        breakePosture = new BreakePosture(pos);
        breakePosture.holder = holder;



    }

    ~Effects()
    {
        disposable?.Dispose();
    }

    /*
    private void ResetEffectsValue()
    {
        attackBuff.ResetValue();
        magicBuff.ResetValue();
        defBuff.ResetValue();
        agiBuff.ResetValue();

        breakePosture.ResetValid();
    }
    */



}


//�^�[�������̃o�t�f�o�t�̊��N���X
public class BaseFloatMultiEffect
{
    public float value { get; private set; }

    public int remaining { get; private set; }

    public EffectMessagePipeHolder holder;

    //
    //protected IAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage> prepareASub;

    protected System.IDisposable disposable;

    //�R���X�g���N�^
    public BaseFloatMultiEffect( )
    {
        //�|���Z�Ȃ̂ŏ����l��1
        this.value = 1.0f;
        this.remaining = 0;

        //this.turnEndASub = GlobalMessagePipe.GetAsyncSubscriber<BattleSceneMessage.TurnEndMessage>();



        
    }

    //�t�@�C�i���C�U
    //����J�n���ɌĂ΂�Ă���B
    ~BaseFloatMultiEffect()
    {
        //Debug.Log("finally");
        disposable?.Dispose();
    }


    //�֐�

    //���l�̕ύX�i�o�t�f�o�t�̕t�^�j
    //=>�^�[���o�߂̃J�E���g�J�n
    public void SetValue(float value, int remaining)
    {
        this.value = value;
        this.remaining = remaining;
        SetSubscriber();
    }

    protected void SetSubscriber()
    {
        var bag = DisposableBag.CreateBuilder();

        holder.turnEndASub.Subscribe(async (get,ct) =>
        {
            //Debug.Log("turnEnd");

            RemainingCheck();
        }).AddTo(bag);
        holder.endASub.Subscribe(async (get, ct) =>
        {
            ResetValue();
        }).AddTo(bag);
        disposable = bag.Build();
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

    public EffectMessagePipeHolder holder;


    protected System.IDisposable disposable;

    public BaseBoolFloatEffect()
    {
        valid = false;
        this.remaining = 0;



    }

    ~BaseBoolFloatEffect()
    {
        //Debug.Log("finally");
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
    public sbyte pos;
    public BreakePosture(sbyte pos)
    {
        this.pos = pos;
    }

    //
    public bool SetValid()
    {
        if (valid)
        {
            return false;
        }
        disposable?.Dispose();

        valid = true;
        remaining = 1;
        var bag = DisposableBag.CreateBuilder();
        holder.turnEndASub.Subscribe(async (info, ct) =>
        {
            RemainingCheck();
        }).AddTo(bag);

        holder.endASub.Subscribe(async (info, ct) =>
        {
            ResetValid();
        }).AddTo(bag);

        disposable = bag.Build();
        return true;
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