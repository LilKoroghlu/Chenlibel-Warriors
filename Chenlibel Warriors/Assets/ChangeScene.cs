using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class ChangeScene : MonoBehaviour
{
    public double changeTime;
    public string sceneName;

    public PlayableDirector director;

    private void Start()
    {
        director.stopped += OnEnd;
    }
    private void OnDestroy()
    {
        director.stopped -= OnEnd;
    }

    private void OnEnd(PlayableDirector argDirector)
    {
        SceneManager.LoadScene(sceneName);
    }
}
