using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;


public class SceneChangeTester : MonoBehaviour
{
    [Inject] private readonly ISubscriber<EnterInput> _EnterInputSubscriber;

    public void Start()
    {
        var bag = DisposableBag.CreateBuilder();

        _EnterInputSubscriber.Subscribe(i => {
            ChangeScene();
        }).AddTo(bag);

    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("NewScene", LoadSceneMode.Additive);
    }
}
