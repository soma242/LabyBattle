using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998// disable warning
 
//�N���X�Ƃ��Ēǉ��C�����ɒǉ��C�R���X�g���N�^�ɒǉ��C�Ή�����v�Z���ɒǉ�
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


//�^�[�������̃o�t�f�o�t�̊��N���X
public class BaseFloatMultiEffect
{
    public float value { get; private set; }

    public int remaining { get; private set; }

    protected IAsyncSubscriber<BattleSceneMessage.TurnEndMessage> turnEndASub;
    protected IAsyncSubscriber<BattleSceneMessage.BattlePrepareMessage> prepareASub;

    protected System.IDisposable disposeOnDestroy;
    protected System.IDisposable disposable;

    //�R���X�g���N�^
    public BaseFloatMultiEffect()
    {
        //�|���Z�Ȃ̂ŏ����l��1
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

    //�t�@�C�i���C�U
    //����J�n���ɌĂ΂�Ă���B
    ~BaseFloatMultiEffect()
    {
        //Debug.Log("finally");
        disposeOnDestroy.Dispose();
        disposable.Dispose();
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