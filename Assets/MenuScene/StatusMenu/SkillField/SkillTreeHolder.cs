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
    public interface ITreeHolder
    {
        public float SetSkillTree(SkillTreeList tree, Vector2 pos, DisposableBagBuilder bag);

        void IFirstSelect(bool first);
        void INextSelect(bool next);
        void IResetSelect();
    }

    //Mask内に存在
    public class SkillTreeHolder : MonoBehaviour, ITreeHolder
    {
        private int i;
        private int selectingNum;

        [SerializeField]
        private MenuSelectHolderSO holder;

        private List<ISkillHolder> holderCatalog = new List<ISkillHolder>();

        [SerializeField]
        private GameObject skillHolder;

        [SerializeField]
        private TMP_Text treeName;

        private Canvas canvas;

        //Classの名前付けミスっている。ツリー本体とその習熟レベルを保持しているClass
        private SkillTreeList tree;

        private System.IDisposable disposableReset;


        void Awake()
        {
            canvas = GetComponent<Canvas>();
            var obj = Instantiate(skillHolder, transform, false);
            holderCatalog.Add(obj.GetComponent<SkillNameHolder>());
            holderCatalog[0].ISetSelectComp(holder.firstSelector);
        }

        void OnDestroy()
        {
            disposableReset?.Dispose();
        }

        public float SetSkillTree(SkillTreeList tree, Vector2 pos, DisposableBagBuilder bag)
        {
            this.tree = tree;
            treeName.SetText(tree.skillTreeSO.treeName);

            var image = GetComponent<Image>();
            image.rectTransform.anchoredPosition = pos;




            var catalog = tree.skillTreeSO.skillCatalog;
            float height = SkillHolderSize.height;
            float sum = height;

            if (catalog.Count == 0 || tree.treeLevel == 0)
            {
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sum);
                return sum;
            }

            canvas.enabled = true;
            var resetASub = GlobalMessagePipe.GetAsyncSubscriber<ResetSkillFieldMessage>();
            resetASub.Subscribe(async (get, ct) =>
            {
                canvas.enabled = false;
                holderCatalog[i].ISetSelectComp(holder.commonSelector);
            }).AddTo(bag);

            //Debug.Log("ok");

            if(tree.treeLevel == 1)
            {
                holderCatalog[0].ISetSelectComp(holder.onlySelector);
                holderCatalog[0].ISetSkillData(catalog[0], sum);
                resetASub.Subscribe(async (get, ct) =>
                {
                    disposableReset?.Dispose();
                    holderCatalog[0].ISetSelectComp(holder.firstSelector);
                }).AddTo(bag);
                disposableReset = bag.Build();

                sum += height;
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sum);
                return sum;
            }


            //最後の一つのみ別で処理したい
            //=>tree.treeLevelから1退いている
            for (i = 0; i < tree.treeLevel -1; i++)
            {
                //Debug.Log(i + "i, count" + catalog.Count);
                if (i >= holderCatalog.Count)
                {
                    //ex0>1 =>Listに存在しないとき
                    var obj = Instantiate(skillHolder, transform, false);
                    holderCatalog.Add(obj.GetComponent<SkillNameHolder>());
                    holderCatalog[i].ISetSelectComp(holder.commonSelector);
                }
                //Debug.Log(i + ": i, count :" + holderCatalog.Count);

                holderCatalog[i].ISetSkillData(catalog[i], sum);
                

                sum += height;
                //Debug.Log(i);
            }
            //Debug.Log("after:" + i);

            if (i == holderCatalog.Count)
            {
                var obj = Instantiate(skillHolder, transform, false);
                holderCatalog.Add(obj.GetComponent<SkillNameHolder>());
            }
            holderCatalog[i].ISetSelectComp(holder.lastSelector);
            holderCatalog[i].ISetSkillData(catalog[i], sum);
            sum += height;

            //Debug.Log(sum);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sum);
            return sum;
        }


        public void IFirstSelect(bool first)
        {
            if (first)
            {
                selectingNum = 0;
            }
            else
            {
                selectingNum = i;
            }
            holderCatalog[selectingNum].IStartSelect(holder.sourceImageSO);
        }

        public void INextSelect(bool next)
        {
            IResetSelect();
            if (next)
            {
                selectingNum++;
            }
            else
            {
                selectingNum--;
            }
            holderCatalog[selectingNum].IStartSelect(holder.sourceImageSO);
        }

        public void IResetSelect()
        {
            holderCatalog[selectingNum].IResetSelect(holder.sourceImageSO);
        }

    }

}
