
using UnityEngine;

//Interface�ł�Serialize�ł��Ȃ�
public class MessageableScriptableObject : ScriptableObject
{
    public virtual void MessageStart() { }

    void Awake()
    {
        Debug.LogWarning("Please Regist to InjectList: " + this.name);
    }
}
