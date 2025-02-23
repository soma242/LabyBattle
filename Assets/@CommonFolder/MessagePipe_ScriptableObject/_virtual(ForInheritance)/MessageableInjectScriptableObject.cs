using UnityEngine;

//Messagepipeの注入を行うための関数の存在を明示するための継承元クラス
//abstractでは，外からこのクラス指定で関数を呼び出せない
public class MessageableInjectScriptableObject : ScriptableObject
{
    public virtual void MessageDependencyInjection() { }
}
