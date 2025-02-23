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
        //プレイ時間外に働いたか確認用(OnEnableの作動タイミング確認につかった)
        //Debug.Log("enableRun");
        //OnEnable，Awake共に，インスペクタ上でこのSOを開いているかシーン上に参照を作らなければ走らなかった。
        //@zzzにアタッチしたSetProviderReferenceから参照を作っている。
        //実際に使用するときは最初に起動するシーンに参照を作ること。
        //一度動けばいいので他のシーンからの参照は消す

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            j = 0;


            //MessageableScriptableObjectの種類ごとに対応したInjectListから行う。
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
