using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;



[CreateAssetMenu(menuName = "MessageableSO/(ÅñComponentàÍéÌóﬁñà)MessageableInjectListSO")]
public class MessageableInjectListSO : MessageableInjectScriptableObject
{
    
    [SerializeField]
    //[UnityEngine.Serialization.FormerlySerializedAs("activeSkillList")]
    private List<MessageableScriptableObject> messageableSOList = new List<MessageableScriptableObject>();
    

    public override void MessageDependencyInjection()
    {
        //EditorUtility.SetDirty(this);

#if UNITY_EDITOR
            //Debug.Log("MessageDependencyInjection");
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                    messageableSOList.TrimExcess();

#endif
        foreach (MessageableScriptableObject messageable in messageableSOList)
        {
            messageable.MessageStart();
        }
    }
}
