using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using DungeonSceneMessage;

//############################LifeTimeScope�Ɉˑ��֌W����͂��邱�Ƃ�Y�ꂸ��

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
        //+�Ȃ�true
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

    //��ʂ̍ĕ`�ʂ��s��
    public struct DungeonDrawMessage { }
    //��ʂ̍ĕ`�ʓ��ł̕ǂ̕`�ʂ��s��
    //drawPos��TKey
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


    //�X�N���[���𖳌����ifalse����true)����
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

    //screen�CComponent�̃t�@�C���f�[�^���X�V���邩
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

//VContainer�ł�struct����class�ɕς���Ɠ����Ȃ��Ȃ�
/*
public class DungeonLifeTimeScope : LifetimeScope
{


    protected override void Configure(IContainerBuilder builder)
    {

        var option = builder.RegisterMessagePipe();






        //TKey�t����MessagePipe���g���ɂ�TKey������MessageBroker��p�ӂ���K�v��������ۂ�
        //TKey������Publish�𑗂��Ă��S�Ă�Tkey�t��Subscribe�͔������Ȃ�
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