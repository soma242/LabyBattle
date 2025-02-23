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
    //  NormalAttackの算出したDamageを通知 => 編成番号をTKey
    private IPublisher<sbyte, NormalDamageCalcMessage> normalDamagePub;

    //Subscriber
    private ISubscriber<NormalAttack> normalAttack;
    private System.IDisposable disposable;



    //ダメージ計算用の関数。
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

    //subで受け取った対象のスキルの処理を行う関数
    private void NormalAttackBoot(NormalAttack message)
    {
        damage = NormalFormula(message.activePos.fromSO, message.activeRatio);
        Debug.Log(damage);
        //normalDamagePub.Publish(activePos.to, new NormalDamageCalcMessage(activePos));
    }

}
