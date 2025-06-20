using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using DungeonSceneMessage;


//どこに移動先のMAPと座標を保持するか
//一案としてDownStairsのインスタンスを都度作成


[CreateAssetMenu(menuName = "dungeonScene/mapComponent/UpStairs")]
public class UpStairsSO : DungeonComponentSO
{
    public DungeonMapMessage moveMapData;
    public override void SetComponent(DisposableBagBuilder bag, DungeonPos pos)
    {
#if UNITY_EDITOR
        if(moveMapData.setDirection !=1&&moveMapData.setDirection!=-1){
            Debug.LogWarning("direction is invalid");
        }
#endif

        var mapStairPub = GlobalMessagePipe.GetPublisher<UpStairsSetMessage>();
        mapStairPub.Publish(new UpStairsSetMessage(pos));

        //Debug.Log("treasure");
        var checkSub = GlobalMessagePipe.GetSubscriber<DungeonPos, ComponentCheckMessage>();
        checkSub.Subscribe(pos, get =>
        {
            //Debug.Log("treasure");
            switch (get.drawPos)
            {
                case 11:
                    Debug.Log(pos.x + "" + pos.y);
                    Debug.Log("On DownStairs");

                    var mapPub = GlobalMessagePipe.GetPublisher<DungeonMapMessage>();
                    mapPub.Publish(moveMapData);

                    break;

            }
        }).AddTo(bag);
    }

    public override bool OverLapBool()
    {


        return false;
    }
}
