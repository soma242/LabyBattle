using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using DungeonSceneMessage;

//############################LifeTimeScopeに依存関係を入力することを忘れずに

namespace DungeonSceneMessage
{




    /*

    public struct RightInput { }
    public struct LeftInput { }
    public struct UpInput { }
    public struct DownInput { }
    public struct EnterInput { }
    public struct CancelInput { }
    public struct MapInput { }

    public class ScrollInput
    {
        public Vector2 value;
        //+ならtrue
        public bool sign;
        public ScrollInput(float value)
        {
            if (value < 0)
            {
                this.value = new Vector2(0, 55);
                sign = true;
            }
            else
            {
                this.value = new Vector2(0, -55);
                sign = false;
            }
        }
    }

    public struct Holdout { }
    */

    public struct MiniMapResetMessage { }
    public struct MiniMapUpdateMessage { }

    //画面の再描写を行う
    public struct DungeonDrawMessage { }
    //画面の再描写内での壁の描写を行う
    //drawPosでTKey
    public struct WallDrawMessage
    {
        public bool wall;
        public int index;
        public WallDrawMessage(bool wall, int index)
        {
            this.wall = !wall;
            this.index = index;
        }
    }



    //
    public struct ResetMapMessage { }
    public struct MapRoadMessage
    {
        public DungeonPos pos;
        public MapRoadMessage(DungeonPos pos)
        {
            this.pos = pos;
        }
    }

    public struct PosChangeMessage { }
    public struct DireChangeMessage { }

    public struct RotateDirectionMessage
    {
        public bool right;
        public RotateDirectionMessage(bool right)
        {
            this.right = right;
        }
    }


    public struct ComponentCheckMessage
    {
        public int drawPos;
        public ComponentCheckMessage(int drawPos)
        {
            this.drawPos = drawPos;
        }
    }
    public struct ComponentOverlapMessage { }

    public struct MapExpandMessage { }
    public struct MapCloseMessage { }

    public struct BigMapResetMessage { }


    //スクリーンを無効化（falseからtrue)する
    public struct ScreenTrueMessage
    {
        public DungeonPos pos;
        public ScreenTrueMessage(DungeonPos pos)
        {
            this.pos = pos;
        }
    }
    public struct ScreenDrawMessage
    {
        public DungeonPos pos;
        public ScreenDrawMessage(DungeonPos pos)
        {
            this.pos = pos;
        }
    }

    //screen，Componentのファイルデータを更新するか
    public struct ScreenValidMessage { }
    public struct ComponentValidMessage { }


    public struct MapChangeMessage { }


    public struct DownStairsSetMessage 
    {
        public DungeonPos pos;
        public DownStairsSetMessage(DungeonPos pos)
        {
            this.pos = pos;
        }
    }
    
    public struct UpStairsSetMessage
    {
        public DungeonPos pos;
        public UpStairsSetMessage(DungeonPos pos)
        {
            this.pos = pos;
        }
    }


}

//VContainerではstructからclassに変えると動かなくなる
/*
public class DungeonLifeTimeScope : LifetimeScope
{


    protected override void Configure(IContainerBuilder builder)
    {

        var option = builder.RegisterMessagePipe();






        //TKey付きのMessagePipeを使うにはTKey無しのMessageBrokerを用意する必要があるっぽい
        //TKey無しのPublishを送っても全てのTkey付きSubscribeは反応しない
        builder.RegisterMessageBroker<RightInput>(option);
        builder.RegisterMessageBroker<LeftInput>(option);
        builder.RegisterMessageBroker<UpInput>(option);
        builder.RegisterMessageBroker<DownInput>(option);
        builder.RegisterMessageBroker<EnterInput>(option);
        builder.RegisterMessageBroker<CancelInput>(option);
        builder.RegisterMessageBroker<MapInput>(option);
        builder.RegisterMessageBroker<ScrollInput>(option);

        builder.RegisterMessageBroker<Holdout>(option);

        
        builder.RegisterMessageBroker<InputLayerSO, RightInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, LeftInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, UpInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, DownInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, EnterInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, CancelInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, MapInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, ScrollInput>(option);


        //builder.RegisterMessageBroker<InputLayerChange>(option);




        //builder.RegisterMessageBroker<SelectChange>(option);
        //builder.RegisterMessageBroker<SelectMessage, SelectChange>(option);


        //builder.RegisterMessageBroker<DungeonMapData>(option);




        //Tester
        //builder.RegisterEntryPoint<PureMPSOStartTester>(Lifetime.Singleton);
    }

}

*/