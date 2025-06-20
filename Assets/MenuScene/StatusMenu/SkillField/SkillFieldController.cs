using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using MessagePipe;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

namespace MenuScene
{
    /// <summary>
    /// SkillTreeHolder��List��ێ�
    /// SkillNameHolder��List��SkillTreeHolder�ɓ���
    /// 
    /// SkillMask
    ///     SkillTreeHolder.cs��ێ�
    ///     TreeHolder��TMP�̎Q�Ƃ�ێ�
    /// 
    /// TreeHolder�̈���
    ///     Mask�̎q�Ƃ��Đ���
    ///     SkillTree�̃��x������TreeHolder��Mask�T�C�Y���m��
    ///     Mask�T�C�Y���玟��TreeHolder�̈ʒu���m��
    /// 
    /// NameHolder�̈���
    ///     TreeHolder����SetText
    ///     =>TreeHolder��SkillTree�Ƃ��̃��x��(SkillTreeList: class)��n���ĕێ�
    ///     =>�����ł�FormationCommander����Character�������Ƃ�΂���
    ///     
    /// Select�̈���
    ///     TreeHolder��MenuSelectHolder
    ///     Tree���m�ł̑I���ړ���FieldController�����(����Tree�ւ̈ړ����iKey�����́jSub��treeCatalog���珀�����s��
    ///         Count��p���Ĕ͈͓��Ɏ��߂�
    ///         Dispose��interface��ʂ��čs��
    ///     TreeHolder����interface������ėp��Pub�p�̃N���X��n���CInput�ɑ΂�����s��ω�������B
    ///     �Ŋ��̈�ƍŏ��̈�̂݁C�O�̃c���[�̍Ō�ɓ����C���̃c���[�̍ŏ��ɓ���class�𐶐�����
    ///         �O�̃c���[�̍Ō��int��class�̃t�B�[���h�ɂ��邱�Ƃŕێ�����
    ///         �I�����ꂽ�^�C�~���O��Sub���s��
    ///     TreeHolder�őΉ�����Sub���s���C�I�𒆂̔z��ԍ���ۑ�����int��class��field�ɗp��
    ///     
    /// </summary>
    public class SkillFieldController : MonoBehaviour
    {
        private int selectingNum;
        private Vector2 fieldPos;

        //[SerializeField]
        //private MenuSelectHolderSO holder;

        [SerializeField]
        private MSO_FormationCommander formations;

        //[SerializeField]
        //private Canvas fieldCanvas;

        [SerializeField]
        private TMP_Text desc;

        [SerializeField]
        private Image skillField;
        
        [SerializeField]
        private GameObject skillMask;

        private int treeNum;
        private List<ITreeHolder> treeCatalog = new List<ITreeHolder>();

        private IAsyncPublisher<ResetSkillFieldMessage> resetAPub;

        private System.IDisposable disposableOnDestroy;
        private System.IDisposable disposableReset;


        void Awake()
        {
            var bag = DisposableBag.CreateBuilder();

            resetAPub = GlobalMessagePipe.GetAsyncPublisher<ResetSkillFieldMessage>();

            var changeSub = GlobalMessagePipe.GetSubscriber<StatusMemberChangeMessage>();
            changeSub.Subscribe(get =>
            {
                fieldPos = Vector2.zero;
                skillField.rectTransform.anchoredPosition = fieldPos;

                SetTreeHolder(get.id);
            }).AddTo(bag);

            var skillSub = GlobalMessagePipe.GetSubscriber<StatusToSkillMessage>();
            skillSub.Subscribe(get =>
            {
                selectingNum = 0;
                treeCatalog[selectingNum].IFirstSelect(true);
            }).AddTo(bag);

            var statusSub = GlobalMessagePipe.GetSubscriber<SkillToStatusMessage>();
            statusSub.Subscribe(get =>
            {
                treeCatalog[selectingNum].IResetSelect();
                desc.SetText(CommonVariable.NoneString);
            }).AddTo(bag);

            var descSub = GlobalMessagePipe.GetSubscriber<SkillDescriptionMessage>();
            descSub.Subscribe(get =>
            {
                desc.SetText(get.desc);
            }).AddTo(bag);

            var nextNameSub = GlobalMessagePipe.GetSubscriber<SelectNextNameHolder>();
            var preNameSub = GlobalMessagePipe.GetSubscriber<SelectPreNameHolder>();
            var nextTreeSub = GlobalMessagePipe.GetSubscriber<SelectNextTreeHolder>();
            var preTreeSub = GlobalMessagePipe.GetSubscriber<SelectPreTreeHolder>();

            nextNameSub.Subscribe(get =>
            {
                fieldPos.y += SkillHolderSize.height;
                skillField.rectTransform.anchoredPosition = fieldPos;

                treeCatalog[selectingNum].INextSelect(true);
            }).AddTo(bag);
            preNameSub.Subscribe(get =>
            {
                fieldPos.y -= SkillHolderSize.height;
                skillField.rectTransform.anchoredPosition = fieldPos;

                treeCatalog[selectingNum].INextSelect(false);
            }).AddTo(bag);

            nextTreeSub.Subscribe(get =>
            {
                if(selectingNum == treeNum)
                {
                    return;
                }
                fieldPos.y += SkillHolderSize.height*2;
                skillField.rectTransform.anchoredPosition = fieldPos;

                treeCatalog[selectingNum].IResetSelect();
                selectingNum++;
                //true=>0����, false=>last����
                treeCatalog[selectingNum].IFirstSelect(true);

            }).AddTo(bag);

            preTreeSub.Subscribe(get =>
            {
                if(selectingNum == 0)
                {
                    return;
                }
                fieldPos.y -= SkillHolderSize.height*2;
                skillField.rectTransform.anchoredPosition = fieldPos;

                treeCatalog[selectingNum].IResetSelect();
                selectingNum--;
                treeCatalog[selectingNum].IFirstSelect(false);
            }).AddTo(bag);


            disposableOnDestroy = bag.Build();
            //var 
        }
        void OnDestroy()
        {
            disposableOnDestroy?.Dispose();
            disposableReset?.Dispose();
        }

        private async void SetTreeHolder(int id)
        {
            selectingNum = 0;

            var bag = DisposableBag.CreateBuilder();
            await resetAPub.PublishAsync(new ResetSkillFieldMessage());
            disposableReset?.Dispose();

            var trees = formations.GetCharaData(id).GetTreeList();
            treeNum = 0;

            Vector2 pos = new Vector2(0f, 0f);

            foreach(var skillTree in trees)
            {
                //Debug.Log(treeNum + "treeNum, count" + treeCatalog.Count);
                if (treeNum >= treeCatalog.Count)
                {
                    //ex0>1 =>List�ɑ��݂��Ȃ��Ƃ�
                    var obj = Instantiate(skillMask, skillField.transform, false);
                    treeCatalog.Add(obj.GetComponent<SkillTreeHolder>());

                }
                pos.y -= treeCatalog[treeNum].SetSkillTree(skillTree, pos, bag);

                treeNum++;
            }
            treeNum--;
            //�S�Ă�Disposable��Z�߂�
            disposableReset = bag.Build();

        }

    }

}

