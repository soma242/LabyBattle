using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CreateAssetMenu(menuName = "TesterSO/MPSO/MessagePipeSOListTester")]
public class MessagePipeSOListTester : ScriptableObject
{
    [SerializeField] private MessagePipeProviderTester providerTester;
    [SerializeField] private List<MessagePipeSOTester> listTester = new List<MessagePipeSOTester>();

    //�R�����g�A�E�g���O����MPSOTestPublisher��Enable�ɂ���Ɠ���
    /*
    public void OnEnable()
    {
        //�v���C���ԊO�ɓ��������m�F�p(OnEnable�̍쓮�^�C�~���O�m�F�ɂ�����)
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
