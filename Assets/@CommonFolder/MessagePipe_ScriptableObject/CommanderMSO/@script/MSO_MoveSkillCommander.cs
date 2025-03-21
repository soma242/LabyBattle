using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/skill/move")]
public class MSO_MoveSkillCommander : MessageableScriptableObject
{
    //Hashset�͗v�f�ԍ�����̒��ڌďo����������Ȃ��������߁Clist��p����B
    [SerializeField] private List<MSO_MoveSkillSO> moveCatalog = new List<MSO_MoveSkillSO>();

    //private ISubscriber<MoveSkillCommand> commandSub;
    private ISubscriber<MoveSkillCommand>moveCommandSub;
    private System.IDisposable disposable;


    public override void MessageStart()
    {

#if UNITY_EDITOR
        SortEnum();
#endif
        var bag = DisposableBag.CreateBuilder();

        //commandSub = GlobalMessagePipe.GetSubscriber<MoveSkillCommand>();
        moveCommandSub = GlobalMessagePipe.GetSubscriber<MoveSkillCommand>();

        ///commandSub.Subscribe(i => {
        //    moveCatalog[i.moveNum].MoveSkillBoot(i.formNum);
        //}).AddTo(bag);

        moveCommandSub.Subscribe(i => {
            //Debug.Log(i);
            moveCatalog[i.moveKey].MoveSkillBoot(i.movePos);
        }).AddTo(bag);

        disposable = bag.Build();

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
            foreach (MSO_MoveSkillSO moveSkill in moveCatalog)
            {
                moveSkill.moveKey = j;
                j++;
            }

            moveCatalog.TrimExcess();


        }
    }

#endif
}
