using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

//[CreateAssetMenu(menuName = "TesterSO/MPSO/MessagePipeSOTester")]
public class MessagePipeSOTester : ScriptableObject
{


    private ISubscriber<BattleStartMessage> testSubscriber;

    private System.IDisposable disposable;

    public void IMessageStart()
    {
        testSubscriber = GlobalMessagePipe.GetSubscriber<BattleStartMessage>();
        disposable = testSubscriber.Subscribe(i =>
        {
            Debug.Log("test is OK");
        });
    }
    

    // Start is called before the first frame update
    //public void Awake()
    /*
    public void OnEnable()
    {
        //プレイ時間外に働いたか確認用
        //Debug.Log("enableRun");

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("Started");


            testSubscriber = GlobalMessagePipe.GetSubscriber<BattleStart>();

            testSubscriber.Subscribe(i =>
            {
                Debug.Log("test is OK");
            });
        }
       

    }
    */
}

