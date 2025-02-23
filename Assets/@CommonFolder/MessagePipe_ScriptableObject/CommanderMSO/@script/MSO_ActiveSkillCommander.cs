using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using MessagePipe;
using SkillStruct;


//�p�����̃X�N���v�^�u���I�u�W�F�N�g�Ō����ł���̂ŁC�ut:MSO_ActiveSkillSO�v�Ō������ă��X�g�ɓ����B


[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/skill/active")]
public class MSO_ActiveSkillCommander : MessageableScriptableObject
{
    //Hashset�͗v�f�ԍ�����̒��ڌďo����������Ȃ��������߁Clist��p����B
    [SerializeField] private List<MSO_ActiveSkillSO> activeCatalog = new List<MSO_ActiveSkillSO>();

    private ISubscriber<ActiveSkillCommand> commandSubscriber;
    private System.IDisposable disposable;


    public override void MessageStart() {

#if UNITY_EDITOR
        SortEnum();
#endif

        commandSubscriber = GlobalMessagePipe.GetSubscriber<ActiveSkillCommand>();

        disposable = commandSubscriber.Subscribe(i =>{
            activeCatalog[i.activeNum].ActiveSkillBoot(i.activePos);
        });

    }

#if UNITY_EDITOR

    private void SortEnum()
    {
        //�v���C���ԊO�ɓ��������m�F�p(OnEnable�̍쓮�^�C�~���O�m�F�ɂ�����)
        //Debug.Log("enableRun");
        //OnEnable�CAwake���ɁC�C���X�y�N�^��ł���SO���J���Ă��邩�V�[����ɎQ�Ƃ����Ȃ���Α���Ȃ������B
        //@zzz�ɃA�^�b�`����SetProviderReference����Q�Ƃ�����Ă���B
        //���ۂɎg�p����Ƃ��͍ŏ��ɋN������V�[���ɎQ�Ƃ���邱�ƁB
        //��x�����΂����̂ő��̃V�[������̎Q�Ƃ͏���

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            int j = 0;
                      //Debug.Log("Unity Editor");


            //MessageableScriptableObject�̎�ނ��ƂɑΉ�����InjectList����s���B
            //activeSkillSO.MessageDependencyInjection();
            foreach (MSO_ActiveSkillSO activeSkill in activeCatalog)
            {
                activeSkill.activeKey.activeNum = j;
                j++;
            }

            activeCatalog.TrimExcess();


        }
    }

#endif
}
