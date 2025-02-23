using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using MessagePipe;

[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/chara")]
public class MSO_CharacterCommander : MessageableScriptableObject
{
    [SerializeField] private List<MSO_CharacterDataSO> charaCatalog = new List<MSO_CharacterDataSO>();

    private ISubscriber<FormCharacterBootMessage> bootFormSub;
    private System.IDisposable disposable;


    public override void MessageStart()
    {
#if UNITY_EDITOR
        SortEnum();
#endif

        bootFormSub = GlobalMessagePipe.GetSubscriber<FormCharacterBootMessage>();

        disposable = bootFormSub.Subscribe(i =>
        {
            FormCharacters(i);
        });

    }

    private void FormCharacters(FormCharacterBootMessage numInfo)
    {
        charaCatalog[numInfo.charaNum].FormingChara(numInfo.formNum);
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
            foreach (MSO_CharacterDataSO chara in charaCatalog)
            {
                chara.charaKey.charaNum = j;
                j++;
            }

            charaCatalog.TrimExcess();

        }
    }

#endif
}
