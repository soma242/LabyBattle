using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;

//�Ăяo���ꂽ��摜�ύX�̗\�肪�Ȃ��̂�OnEnable�ɂ��ύX�̂�
//�퓬����HP�Ȃǂŉ摜�ύX����Ȃ�V����Sub��p�ӂ���

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
