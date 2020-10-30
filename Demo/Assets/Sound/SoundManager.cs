using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource Music;

    private bool _HasPlayed = false;
    private int _UpdatesCount = 0;

    void Start()
    {        
    }

    void Update()
    {
        if (!Music.isPlaying && !_HasPlayed && _UpdatesCount>1)
        {
            Music.Play();
            _HasPlayed = true;
        }

        if ((!Music.isPlaying && _HasPlayed) || Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        ++_UpdatesCount;
    }
}
