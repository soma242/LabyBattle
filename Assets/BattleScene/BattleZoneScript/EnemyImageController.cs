using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class EnemyImageController : MonoBehaviour
{
    [SerializeField] private MSO_FormationEnemySO formationSO;

    private Image image;


    //private System.IDisposable disposable;

    void Awake()
    {

        image = GetComponent<Image>();


    }

    void OnEnable()
    {
        image.sprite = formationSO.enemy.enemyImage.battleImage;

    }
}