using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using DungeonSceneMessage;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

//Pub‚ÌMessage‚É‚àŽg—p
[System.Serializable]
public class EncountGroup
{
    public List<EnemyData> enemyGroup = new List<EnemyData>();
    public int rate;
}

public interface IEncounterEnemy
{
    public void ISetSum();
    public void ISetEncount(DisposableBagBuilder bag);

}

[CreateAssetMenu(menuName = "dungeonScene/encount")]
public class EncounterEnemySO : ScriptableObject, IEncounterEnemy
{
    public List<EncountGroup> encountList = new List<EncountGroup>();

    public int encountRate;
    private int sumRate;

    public void ISetSum()
    {
        sumRate = 0;
        foreach (EncountGroup group in encountList)
        {
            sumRate += group.rate;
        }
    }

    public void ISetEncount(DisposableBagBuilder bag)
    {

        var posSub = GlobalMessagePipe.GetSubscriber<DungeonSceneMessage.PosChangeMessage>();
        posSub.Subscribe(async get =>
        {
            int test = Random.Range(0, 100);
            //Debug.Log(test);
            if (test < encountRate)
            {
                int value = Random.Range(0, sumRate);
                for(int i = 0; i<encountList.Count; i++)
                {
                    value -= encountList[i].rate;
                    //Debug.Log("value:" + value);
                    if(value < 0)
                    {
                        var encountPub = GlobalMessagePipe.GetAsyncPublisher<EncountGroup>();
                        await encountPub.PublishAsync(encountList[i]);

                        var callPub = GlobalMessagePipe.GetPublisher<CallBattleMessage>();
                        callPub.Publish(new CallBattleMessage());

                        var closePub = GlobalMessagePipe.GetPublisher<MapCloseMessage>();
                        closePub.Publish(new MapCloseMessage());

                        break;
                    }
                }

            }
        }).AddTo(bag);
    }
}
