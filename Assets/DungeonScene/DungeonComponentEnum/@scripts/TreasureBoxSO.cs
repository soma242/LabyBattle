using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using DungeonSceneMessage;

[CreateAssetMenu(menuName = "dungeonScene/mapComponent/TreasureBox")]
public class TreasureBoxSO : DungeonComponentSO
{
    public override void SetComponent(DisposableBagBuilder bag, DungeonPos pos)
    {
        //Debug.Log("treasure");
        var checkSub = GlobalMessagePipe.GetSubscriber<DungeonPos, ComponentCheckMessage>();
        checkSub.Subscribe(pos ,get =>
        {
            //Debug.Log("treasure");
            switch (get.drawPos)
            {
                case 11:
                    Debug.Log(pos.x +""+ pos.y);
                    var invalidPub = GlobalMessagePipe.GetPublisher<DungeonPos, ComponentOverlapMessage>();
                    invalidPub.Publish(pos, new ComponentOverlapMessage());
                    Debug.Log("treasure get");

                    var drawPub = GlobalMessagePipe.GetPublisher<TestMessage>();
                    drawPub.Publish( new TestMessage());

                    break;
                case 4:
                    Debug.Log("treasure right");
                    break;
            }
        }).AddTo(bag);
    }

    public override bool OverLapBool()
    {


        return true;
    }
}
