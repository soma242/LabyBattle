using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using StartSceneMessage;

public struct StartSceneDisableMessage { }
public struct StartSceneEnableMessage { }

public struct CallBattleMessage { }
public struct EndBattleSceneMessage { }

public struct SaveCallMessage { }

public struct NewGameMessage { }
public struct GameContinueMessage { }

public struct NewGameboolSaveTrigger { }

public struct DungeonToMenuMessage { }
public struct MenuToDungeonMessage { }

public struct ToTitleMessage { }

public struct InputSystemSwitch
{
    public SceneName scene;
    public InputSystemSwitch(SceneName scene)
    {
        this.scene = scene;
    }
}
public enum SceneName
{
    start,
    dungeon,
    battle,
    menu
}

namespace StartSceneMessage
{
    /*
    public struct UpInput { }
    public struct DownInput { }
    public struct EnterInput { }

    public struct Holdout { }
    */

    public struct SelectComp { }

    public struct StartSelectMessage
    {
        public bool played;
        public StartSelectMessage(bool played) { this.played = played; }
    }
    public struct NoCheckMessage { }
    public struct YesCheckMessage { }
}


/*
public class StartLifeTimeScope : LifetimeScope
{


    protected override void Configure(IContainerBuilder builder)
    {

        var option = builder.RegisterMessagePipe();






        //TKey付きのMessagePipeを使うにはTKey無しのMessageBrokerを用意する必要があるっぽい
        //TKey無しのPublishを送っても全てのTkey付きSubscribeは反応しない

        builder.RegisterMessageBroker<UpInput>(option);
        builder.RegisterMessageBroker<DownInput>(option);
        builder.RegisterMessageBroker<EnterInput>(option);

        builder.RegisterMessageBroker<Holdout>(option);

        builder.RegisterMessageBroker<SelectComp>(option);

        builder.RegisterMessageBroker<sbyte, SelectComp>(option);







        //builder.RegisterMessageBroker<InputLayerChange>(option);




        //builder.RegisterMessageBroker<SelectChange>(option);
        //builder.RegisterMessageBroker<SelectMessage, SelectChange>(option);


        //builder.RegisterMessageBroker<DungeonMapData>(option);




        //Tester
        //builder.RegisterEntryPoint<PureMPSOStartTester>(Lifetime.Singleton);
    }

}
*/