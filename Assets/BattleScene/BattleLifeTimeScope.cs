using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

//############################LifeTimeScopeに依存関係を入力することを忘れずに

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

    //複数回選択を行わせる

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



    //InputLayer関係
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
    //将来的に，InputSystem => SceneInputSystem(MessagePipeに変換) => LayerInputSystemMSO(Layer毎に固有のMessageに変換)
    //の処理に変更する可能性あり。
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

//VContainerではstructからclassに変えると動かなくなる

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


        //TKey付きのMessagePipeを使うにはTKey無しのMessageBrokerを用意する必要があるっぽい
        //TKey無しのPublishを送っても全てのTkey付きSubscribeは反応しない
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
