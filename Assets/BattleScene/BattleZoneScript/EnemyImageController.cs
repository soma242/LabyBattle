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

        var bag = DisposableBag.CreateBuilder();

        setImageSub.Subscribe(formationSO.GetFormNum(), get =>
        {
            var canvas = GetComponent<Canvas>();
            canvas.enabled = true;
            image.sprite = formationSO.enemy.enemyImage.battleImage;

        }).AddTo(bag);

        disposable = bag.Build();
    }

    void OnDestroy()
    {
        disposable.Dispose();
    }


}