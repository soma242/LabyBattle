using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

//[CreateAssetMenu(menuName = "TesterSO/SortedSO/SortListSO")]
public class SortListTesterSO : ScriptableObject
{
    [SerializeField] private List<SortedEnumTesterSO> sortList = new List<SortedEnumTesterSO>();
    private int j;

    /*
    public void OnEnable()
    {
        //�v���C���ԊO�ɓ��������m�F�p(OnEnable�̍쓮�^�C�~���O�m�F�ɂ�����)
        //Debug.Log("enableRun");
        //OnEnable�CAwake���ɁC�C���X�y�N�^��ł���SO���J���Ă��邩�V�[����ɎQ�Ƃ����Ȃ���Α���Ȃ������B
        //@zzz�ɃA�^�b�`����SetProviderReference����Q�Ƃ�����Ă���B
        //���ۂɎg�p����Ƃ��͍ŏ��ɋN������V�[���ɎQ�Ƃ���邱�ƁB
        //��x�����΂����̂ő��̃V�[������̎Q�Ƃ͏���

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            j = 0;


            //MessageableScriptableObject�̎�ނ��ƂɑΉ�����InjectList����s���B
            //activeSkillSO.MessageDependencyInjection();
            foreach (SortedEnumTesterSO sortEnum in sortList)
            {
                sortEnum.i = j;
                j++;
            }

        }
    }
    */
}
