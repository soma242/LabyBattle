using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/calc/MSO_DamageCalcSO")]
public class MSO_DamageCalcSO : MessageableScriptableObject
{
    //private int tempAttack;
    //private float damage;

    //MessagePipe
    //Publisher
    //  NormalAttackの算出したDamageを通知 => 編成番号をTKey
    private IPublisher<sbyte, NormalDamageCalcMessage> normalDamagePub;
    private IPublisher<sbyte, NormalMagicDamageCalcMessage> normalMagicDamagePub;

    //Subscriber
    private ISubscriber<NormalAttack> normalAttackSub;
    private ISubscriber<NormalMagic> normalMagicSub;
    private System.IDisposable disposable;





    public override void MessageStart() {

        //Normal
        //pub
        normalDamagePub = GlobalMessagePipe.GetPublisher<sbyte, NormalDamageCalcMessage>();
        normalMagicDamagePub = GlobalMessagePipe.GetPublisher<sbyte, NormalMagicDamageCalcMessage>();

        //sub
        normalAttackSub = GlobalMessagePipe.GetSubscriber<NormalAttack>();
        normalMagicSub = GlobalMessagePipe.GetSubscriber<NormalMagic>();

        var bag = DisposableBag.CreateBuilder();

        normalAttackSub.Subscribe(i =>
        {
            float damage = NormalPhysicalFormula(i.activePos.userSO, i.activeRatio);
            //Debug.Log(damage);

            normalDamagePub.Publish(i.activePos.target, new NormalDamageCalcMessage(damage, i.activePos));
        }).AddTo(bag);

        normalMagicSub.Subscribe(i =>
        {
            float damage = NormalMagicFormula(i.activePos.userSO, i.activeRatio);
            //Debug.Log(name);
            normalMagicDamagePub.Publish(i.activePos.target, new NormalMagicDamageCalcMessage(damage, i.activePos));
        }).AddTo(bag);
    }




    //ダメージ計算用の関数。
    private float NormalPhysicalFormula(IGetFormationInfo info, float activeRatio)
    {
        //int damageCalc = Mathf.FloorToInt(info.GetActualAttack() * activeRatio);

        float damageCalc = info.GetActualAttack() * activeRatio;

        /*
#if UNITY_EDITOR
        Debug.Log("actualAttack: "+ info.GetActualAttack());
        Debug.Log("activeRatio: "+ activeRatio);
#endif
      */  


        return damageCalc;
    }
    
    private float NormalMagicFormula(IGetFormationInfo info, float activeRatio)
    {
        //int damageCalc = Mathf.FloorToInt(info.GetActualMagic() * activeRatio);
        float damageCalc = info.GetActualMagic() * activeRatio;

        /*
#if UNITY_EDITOR
        Debug.Log("actualMagic: "+ info.GetActualMagic());
        Debug.Log("activeRatio: "+ activeRatio);
#endif
        */


        return damageCalc;
    }

}
