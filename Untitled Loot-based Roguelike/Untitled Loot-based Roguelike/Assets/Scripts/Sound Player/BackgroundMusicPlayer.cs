using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;

    private AudioSource cr_audioSource;

    private AudioClip current;

    [SerializeField]
    private float timeline;

    // Start is called before the first frame update
    void Start()
    {
        cr_audioSource = GetComponent<AudioSource>();
        PlayRandomMusic();
    }

    private void PlayRandomMusic()
    {
        cr_audioSource.clip = getRandomClip();
        cr_audioSource.Play();
    }

    private AudioClip getRandomClip()
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        if (!cr_audioSource.isPlaying)
        {
            PlayRandomMusic();
        } else
        {
        }
    }
}
