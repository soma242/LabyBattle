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
    /// SkillTreeHolderのListを保持
    /// SkillNameHolderのListはSkillTreeHolderに内臓
    /// 
    /// SkillMask
    ///     SkillTreeHolder.csを保持
    ///     TreeHolderのTMPの参照を保持
    /// 
    /// TreeHolderの扱い
    ///     Maskの子として生成
    ///     SkillTreeのレベルからTreeHolderのMaskサイズを確定
    ///     Maskサイズから次のTreeHolderの位置を確定
    /// 
    /// NameHolderの扱い
    ///     TreeHolderからSetText
    ///     =>TreeHolderにSkillTreeとそのレベル(SkillTreeList: class)を渡して保持
    ///     =>ここではFormationCommanderからCharacterをうけとればいい
    ///     
    /// Selectの扱い
    ///     TreeHolderにMenuSelectHolder
    ///     Tree同士での選択移動にFieldControllerを介する(次のTreeへの移動を（Key無しの）SubしtreeCatalogから準備を行う
    ///         Countを用いて範囲内に収める
    ///         Disposeをinterfaceを通して行う
    ///     TreeHolderからinterfaceを介した汎用のPub用のクラスを渡し，Inputに対する実行を変化させる。
    ///     最期の一つと最初の一つのみ，前のツリーの最後に動く，次のツリーの最初に動くclassを生成する
    ///         前のツリーの最後はintをclassのフィールドにすることで保持する
    ///         選択されたタイミングでSubを行う
    ///     TreeHolderで対応するSubを行い，選択中の配列番号を保存するintをclassのfieldに用意
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
                //true=>0から, false=>lastから
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
                    //ex0>1 =>Listに存在しないとき
                    var obj = Instantiate(skillMask, skillField.transform, false);
                    treeCatalog.Add(obj.GetComponent<SkillTreeHolder>());

                }
                pos.y -= treeCatalog[treeNum].SetSkillTree(skillTree, pos, bag);

                treeNum++;
            }
            treeNum--;
            //全てのDisposableを纏める
            disposableReset = bag.Build();

        }

    }

}

