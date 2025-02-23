using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;

//呼び出された後画像変更の予定がないのでOnEnableによる変更のみ
//戦闘中にHPなどで画像変更するなら新しくSubを用意する

public class AllyImageController : MonoBehaviour
{
    [SerializeField] private MSO_FormationCharaSO formationSO;


    private Image image;
    //private ISubscriber<>


    //private System.IDisposable disposable;

    void Awake()
    {

        image = GetComponent<Image>();
        image.sprite = formationSO.setChara.charaImages.battleImage;


    }

    void OnEnable()
    {

    }


}
