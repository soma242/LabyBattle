using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CreateAssetMenu(menuName = "TesterSO/MPSO/MessagePipeSOListTester")]
public class MessagePipeSOListTester : ScriptableObject
{
    [SerializeField] private MessagePipeProviderTester providerTester;
    [SerializeField] private List<MessagePipeSOTester> listTester = new List<MessagePipeSOTester>();

    //コメントアウトを外してMPSOTestPublisherをEnableにすると働く
    /*
    public void OnEnable()
    {
        //プレイ時間外に働いたか確認用(OnEnableの作動タイミング確認につかった)
        //Debug.Log("enableRun");

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("Started");

            providerTester.SetGlobalMessageProvider();

            foreach(MessagePipeSOTester SOTester in listTester)
            {
                SOTester.IMessageStart();
            }

         }
    }
    */
}
