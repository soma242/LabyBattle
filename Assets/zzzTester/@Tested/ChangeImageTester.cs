using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;


public class ChangeImageTester : MonoBehaviour
{
    [Inject] private readonly ISubscriber<RightInput> _RightInputSubscriber;
    private System.IDisposable disposable;


    [SerializeField] SelectSourceImageSO _sorceImage;
    private Image _image;

    //[Inject] private readonly IPublisher<RightInput> _RightInputPublisher;


    // Start is called before the first frame update
    void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = _sorceImage.onSelect;

        disposable = _RightInputSubscriber.Subscribe(i =>
        {
            ChangeSprite();
        });
        /*
        _RightInputSubscriber.Subscribe(i =>
        {
            ChangeSprite();
        });
        */
        //.AddTo(this);
    }

    /*
    void Start()
    {
        _RightInputPublisher.Publish(new RightInput());
    }
    */

    private void ChangeSprite()
    {
        _image.sprite = _sorceImage.offSelect;
        Debug.Log("changeSprite");
    }
}
