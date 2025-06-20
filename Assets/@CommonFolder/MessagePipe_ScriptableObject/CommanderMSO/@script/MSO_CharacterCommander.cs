using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using MessagePipe;

[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/chara")]
public class MSO_CharacterCommander : MessageableScriptableObject
{
    [SerializeField] private List<CharacterDataSO> charaCatalog = new List<CharacterDataSO>();

    //private ISubscriber<FormCharacterBootMessage> bootFormSub;
    private IPublisher<sbyte, FormCharacterMessage> formCharaPub;
    private System.IDisposable disposable;


    public override void MessageStart()
    {
#if UNITY_EDITOR
        SortEnum();
#endif

        var bootFormSub = GlobalMessagePipe.GetSubscriber<FormCharacterBootMessage>();

        formCharaPub = GlobalMessagePipe.GetPublisher<sbyte, FormCharacterMessage>();

        disposable = bootFormSub.Subscribe(i =>
        {
            //charaCatalog[i.charaNum].FormingChara(i.formNum);
            formCharaPub.Publish(i.formNum, new FormCharacterMessage(charaCatalog[i.charaNum]));

        });

    }



#if UNITY_EDITOR

    private void SortEnum()
    {
        //プレイ時間外に働いたか確認用(OnEnableの作動タイミング確認につかった)
        //Debug.Log("enableRun");
        //OnEnable，Awake共に，インスペクタ上でこのSOを開いているかシーン上に参照を作らなければ走らなかった。
        //@zzzにアタッチしたSetProviderReferenceから参照を作っている。
        //実際に使用するときは最初に起動するシーンに参照を作ること。
        //一度動けばいいので他のシーンからの参照は消す

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            int j = 0;
                      //Debug.Log("Unity Editor");


            //MessageableScriptableObjectの種類ごとに対応したInjectListから行う。
            //activeSkillSO.MessageDependencyInjection();
            foreach (CharacterDataSO chara in charaCatalog)
            {
                chara.charaKey.charaNum = j;
                j++;
            }

            charaCatalog.TrimExcess();

        }
    }

#endif
}
