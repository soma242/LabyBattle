using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;

public class EnemyImageController : MonoBehaviour
{
    [SerializeField] private MSO_FormationEnemySO formationSO;

    private Image image;

    private ISubscriber<sbyte, SetEnemyImage> setImageSub;
    private System.IDisposable disposable;


    //private System.IDisposable disposable;

    void Awake()
    {

        image = GetComponent<Image>();
        setImageSub = GlobalMessagePipe.GetSubscriber<sbyte, SetEnemyImage>();

        disposable = setImageSub.Subscribe(formationSO.GetFormNum(), get =>
        {
            image.sprite = formationSO.enemy.enemyImage.battleImage;

        });


    }

    void OnDestroy()
    {
        disposable.Dispose();
    }


}