using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

//############################LifeTimeScope�Ɉˑ��֌W����͂��邱�Ƃ�Y�ꂸ��

namespace BattleSceneMessage
{
    public struct BattleStartMessage { }
    public struct BattlePrepareMessage { }
    public struct BattleFinishMessage { }

    public struct ActionSelectStartMessage
    {
        /*

        */
    }

    public struct ActionSelectEndMessage { }



    public struct TurnStartMessage { }
    public struct TurnEndMessage { }

    public struct CharaAgilitySetMessage { }

    public struct BattleOptionSelectStart { }

    public struct BattleOptionSelecting { }

    //������I�����s�킹��

    public struct BattleOptionSelected {
        public int selectedNum;

        public BattleOptionSelected(int _selectedNum) { 
            selectedNum = _selectedNum; 
        }
    }

    public struct BattleProcessStart { }


    public struct RightInput { }
    public struct LeftInput { }
    public struct UpInput { }
    public struct DownInput { }
    public struct EnterInput { }
    public struct CancelInput { }

    public struct Holdout { }

    public struct DisposeSelect { }



    //InputLayer�֌W
    //MessageableFirstCommanderSO

    public struct SelectChange { }


    public struct InputLayer
    {
        public InputLayerSO inputLayerSO { get; }

        public InputLayer(InputLayerSO inputLayerSO)
        {
            this.inputLayerSO = inputLayerSO;
        }
    }

    public struct InputLayerChanged { }


    //BuiltInContainer
    //from SelectComp,FirstSelectCommander, To SelectComp
    //�����I�ɁCInputSystem => SceneInputSystem(MessagePipe�ɕϊ�) => LayerInputSystemMSO(Layer���ɌŗL��Message�ɕϊ�)
    //�̏����ɕύX����\������B
    public struct SelectMessage
    {
        public InputLayerSO inputLayerSO { get; }
        public int selectNum;

        public SelectMessage(InputLayerSO inputLayerSO, int selectNum)
        {
            this.inputLayerSO = inputLayerSO;
            this.selectNum = selectNum;

        }
    }







}

//VContainer�ł�struct����class�ɕς���Ɠ����Ȃ��Ȃ�

public class BattleLifeTimeScope : LifetimeScope
{


    protected override void Configure(IContainerBuilder builder)
    {

        var option = builder.RegisterMessagePipe();



        //builder.RegisterMessageBroker<BattleStart>(option);
        builder.RegisterMessageBroker<BattleOptionSelectStart>(option);

        builder.RegisterMessageBroker<BattleOptionSelecting>(option);
        builder.RegisterMessageBroker<BattleOptionSelected>(option);

        builder.RegisterMessageBroker<BattleProcessStart>(option);


        //TKey�t����MessagePipe���g���ɂ�TKey������MessageBroker��p�ӂ���K�v��������ۂ�
        //TKey������Publish�𑗂��Ă��S�Ă�Tkey�t��Subscribe�͔������Ȃ�
        builder.RegisterMessageBroker<RightInput>(option);
        builder.RegisterMessageBroker<LeftInput>(option);
        builder.RegisterMessageBroker<UpInput>(option);
        builder.RegisterMessageBroker<DownInput>(option);
        builder.RegisterMessageBroker<EnterInput>(option);
        builder.RegisterMessageBroker<CancelInput>(option);

        builder.RegisterMessageBroker<Holdout>(option);


        builder.RegisterMessageBroker<InputLayerSO, RightInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, LeftInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, UpInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, DownInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, EnterInput>(option);
        builder.RegisterMessageBroker<InputLayerSO, CancelInput>(option);


        //builder.RegisterMessageBroker<InputLayerChange>(option);

        //builder.RegisterMessageBroker<DisposeSelect>(option);

        //builder.RegisterMessageBroker<SelectChange>(option);
        //builder.RegisterMessageBroker<SelectMessage, SelectChange>(option);



        builder.RegisterEntryPoint<BattleSceneCommand>(Lifetime.Singleton);



        //Tester
        //builder.RegisterEntryPoint<PureMPSOStartTester>(Lifetime.Singleton);
    }

}
