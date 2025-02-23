using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

[CreateAssetMenu(menuName = "MessageableSO/Component/CharaData/normal")]
public class MSO_NormalCharaDataSO : MSO_CharacterDataSO
{


    //基底クラスにアップキャストで渡しているのでnewでは基底クラスの関数が呼び出される。
    /*
    public override int GetAttack()
    {
        return 0;
    }
    */


    //このキャラクターデータの参照を指定のformationに送り付ける。


}
