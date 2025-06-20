using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


namespace MenuScene
{
    public interface ISkillHolder
    {
        public void ISetSkillData(MSO_SkillHolderSO skillData, float posY);
        public void ISetSelectComp(ISelectSkillNameHolder selector);
        void IStartSelect(SelectSourceImageSO sourceImageSO);
        void IResetSelect(SelectSourceImageSO sourceImageSO);

    }


    public class SkillNameHolder : MonoBehaviour, ISkillHolder
    {
        private MSO_SkillHolderSO data;
        //Menu‚Å’è‹`
        private ISelectSkillNameHolder selector;

        [SerializeField]
        private TMP_Text skillName;

        private Image image;

        public void ISetSkillData(MSO_SkillHolderSO skillData, float posY)
        {
            //TMP_Text skillName = GetComponent<TMP_Text>();
            data = skillData;
            //Debug.Log(posY);

            image = GetComponent<Image>();

            skillName.SetText(data.GetSkillName());
            image.rectTransform.anchoredPosition = new Vector2(20, -1f * posY);


        }

        public void ISetSelectComp(ISelectSkillNameHolder selector)
        {
            this.selector = selector;
        }
        public void IStartSelect(SelectSourceImageSO sourceImageSO)
        {
            selector.holder.descPub.Publish(new SkillDescriptionMessage(data.GetDescription()));
            image.sprite = sourceImageSO.onSelect;
            selector.ISetSelect();
        }
        public void IResetSelect(SelectSourceImageSO sourceImageSO)
        {            
            image.sprite = sourceImageSO.offSelect;
            selector.IResetSelect();
        }
    }

}
