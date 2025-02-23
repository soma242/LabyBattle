using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using BattleSceneMessage;

//[CreateAssetMenu(menuName = "TesterSO/MPSO/MessagePipeProviderTester")]
public class MessagePipeProviderTester : ScriptableObject
{
    // Start is called before the first frame update
    
    public void SetGlobalMessageProvider()
    {
        var builder = new BuiltinContainerBuilder();

        builder.AddMessagePipe(/* configure option */);

        // AddMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.
        builder.AddMessageBroker<BattleStartMessage>();

        // also exists AddMessageBroker<TKey, TMessage>, AddRequestHandler, AddAsyncRequestHandler

        // AddMessageHandlerFilter: Register for filter, also exists RegisterAsyncMessageHandlerFilter, Register(Async)RequestHandlerFilter
        //builder.AddMessageHandlerFilter<MyFilter<BattleStart>>();

        // create provider and set to Global(to enable diagnostics window and global fucntion)
        var provider = builder.BuildServiceProvider();
        GlobalMessagePipe.SetProvider(provider);
    }


}
