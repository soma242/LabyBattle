using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

using MessagePipe;
using SkillStruct;
using BattleSceneMessage;

//using UnityEngine.Serialization;    //追加


[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/formation/chara")]
public class MSO_FormationCommander : MessageableScriptableObject
{

    private sbyte count;

    //[FormerlySerializedAs("formationCharaCatalog")]
    [SerializeField]
    private List<MSO_FormationCharaSO> formCharas = new List<MSO_FormationCharaSO>();

    [SerializeField]
    private List<MSO_FormationEnemySO> formEnemys = new List<MSO_FormationEnemySO>();

    /*
    private ISubscriber<NormalMoveToFront> moveToFrontSub;
    private ISubscriber<NormalMoveToBack> moveToBackSub;

    */

    private System.IDisposable disposable;

    //private IPublisher<CallBattleMessage> callPub;

    private ISubscriber<BattleStartMessage> battleStartSub;
    private System.IDisposable disposableBattle;
    private System.IDisposable disposableCheckConti;


    public override void MessageStart()
    {

#if UNITY_EDITOR
        formCharas.TrimExcess();
        formEnemys.TrimExcess();
#endif



        var bagD = DisposableBag.CreateBuilder();
        var encountSub = GlobalMessagePipe.GetAsyncSubscriber<EncountGroup>();



        encountSub.Subscribe(async (info,ct) =>
        {
            for(int i = 0; i < 3; i++)
            {
               // Debug.Log("i" + i);
                //Debug.Log(info.enemyGroup.Count);
                if(i < info.enemyGroup.Count)
                {
                    //Debug.Log("ok");
                    formEnemys[i].SetEnemy(info.enemyGroup[i]);
                }
                else
                {
                    formEnemys[i].NotSetEnemy();
                }
            }

            //Debug.Log("pub");
        }).AddTo(bagD);


        battleStartSub = GlobalMessagePipe.GetSubscriber<BattleStartMessage>();

        //バトル開始時にSub開始
         battleStartSub.Subscribe(GetBattleImage =>
        {
            //disposableBattle?.Dispose();

            var bag = DisposableBag.CreateBuilder();

            //Enemyの全滅判定
            var dropEmemySub = GlobalMessagePipe.GetSubscriber<DropEnemyMessage>();
            var dropCharaSub = GlobalMessagePipe.GetSubscriber<DropCharaMessage>();

            dropEmemySub.Subscribe(get =>
            {
                count = FormationScope.NoneEnemy();
                var checkReturnSub = GlobalMessagePipe.GetSubscriber<KnockOutChecker>();
                disposableCheckConti = checkReturnSub.Subscribe(get =>
                {
                    
                    count = get.pos;
                    //Debug.Log("count: "+count);
                });
                var checkGetPub = GlobalMessagePipe.GetPublisher<KnockOutEnemy>();
                checkGetPub.Publish(new KnockOutEnemy());
                disposableCheckConti?.Dispose();

                UniTask.NextFrame();

                if (count == FormationScope.NoneEnemy())
                {
                    //Debug.Log("count: " + count);

                    Debug.Log("all down");
                    var allDownPub = GlobalMessagePipe.GetPublisher<AllEnemyDownMessage>();
                    allDownPub.Publish(new AllEnemyDownMessage());
                }
            }).AddTo(bag);
            //Charaの全滅判定
            dropCharaSub.Subscribe(get =>
            {
                count = FormationScope.NoneChara();
                var checkReturnSub = GlobalMessagePipe.GetSubscriber<KnockOutChecker>();
                disposableCheckConti = checkReturnSub.Subscribe(get =>
                {
                    count = get.pos;
                    Debug.Log("count: " + count);
                });
                var checkGetPub = GlobalMessagePipe.GetPublisher<KnockOutChara>();
                checkGetPub.Publish(new KnockOutChara());
                disposableCheckConti?.Dispose();

                UniTask.NextFrame();
                //Debug.Log("count: " + count);

                if (count == FormationScope.NoneChara())
                {
                    Debug.Log("all down");
                    var allDownPub = GlobalMessagePipe.GetPublisher<AllCharaDownMessage>();
                    allDownPub.Publish(new AllCharaDownMessage());
                }
            }).AddTo(bag);

            var endSub = GlobalMessagePipe.GetSubscriber<BattleFinishMessage>();
            endSub.Subscribe(get =>
            {
                disposableBattle?.Dispose();
            }).AddTo(bag);

            disposableBattle = bag.Build();

        }).AddTo(bagD);

        disposable = bagD.Build();

    }

    //Chara
    public CharacterDataSO GetCharaData(int i)
    {
        return formCharas[i].setChara;
    }

    public string GetCharaName(int i)
    {
        return formCharas[i].setChara.GetCharaName();
    }
    public bool GetParticipant(int i)
    {
        return formCharas[i].participant;
    }

    public Sprite GetBattleImage(int i)
    {
        return formCharas[i].setChara.charaImages.battleImage;
    }

    public MovePosition GetMovePosition(int i)
    {
        return formCharas[i].currentPos;
    }

    public float GetCharaRatioOnHP(int i)
    {
        //Debug.Log(formCharas[i].GetRatioOnHP());
        return formCharas[i].GetRatioOnHP();
    }

    //Enemy
    public string GetEnemyName(int i)
    {
        return formEnemys[i].enemy.GetEnemyName();
    }

    public string GetSettedSkillName(int i)
    {
        return formEnemys[i].GetSettedSkillName();
    }

    public bool GetEnemyParticipant(int i)
    {
        return formEnemys[i].participant;
    }

    public float GetEnemyRatioOnHP(int i)
    {
        return formEnemys[i].GetRatioOnHP();
    }
}
