using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/calc/MSO_DamageCalcSO")]
public class MSO_DamageCalcSO : MessageableScriptableObject
{
    private int tempAttack;
    private int damage;

    //MessagePipe
    //Publisher
    //  NormalAttack�̎Z�o����Damage��ʒm => �Ґ��ԍ���TKey
    private IPublisher<sbyte, NormalDamageCalcMessage> normalDamagePub;

    //Subscriber
    private ISubscriber<NormalAttack> normalAttack;
    private System.IDisposable disposable;



    //�_���[�W�v�Z�p�̊֐��B
    private int NormalFormula(MSO_FormationCharaSO fromSO, float activeRatio)
    {
        int damageCalc = Mathf.FloorToInt(fromSO.GetActualAttack() * activeRatio);

#if UNITY_EDITOR
        Debug.Log("attack: "+ fromSO.setChara.GetAttack());
        Debug.Log("actualAttack: "+ fromSO.GetActualAttack());
        Debug.Log("activeRatio: "+ activeRatio);
#endif

        return damageCalc;
    }

    public override void MessageStart() {

        //pub
        normalDamagePub = GlobalMessagePipe.GetPublisher<sbyte, NormalDamageCalcMessage>();

        //sub
        normalAttack = GlobalMessagePipe.GetSubscriber<NormalAttack>();



        disposable = normalAttack.Subscribe(i =>
        {
            NormalAttackBoot(i);
        });
    }

    //sub�Ŏ󂯎�����Ώۂ̃X�L���̏������s���֐�
    private void NormalAttackBoot(NormalAttack message)
    {
        damage = NormalFormula(message.activePos.fromSO, message.activeRatio);
        Debug.Log(damage);
        //normalDamagePub.Publish(activePos.to, new NormalDamageCalcMessage(activePos));
    }

}
