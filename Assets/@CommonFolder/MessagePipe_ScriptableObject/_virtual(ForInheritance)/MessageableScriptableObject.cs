
using UnityEngine;

//Interface‚Å‚ÍSerialize‚Å‚«‚È‚¢
public class MessageableScriptableObject : ScriptableObject
{
    public virtual void MessageStart() { }

    void Awake()
    {
        Debug.LogWarning("Please Regist to InjectList: " + this.name);
    }
}
