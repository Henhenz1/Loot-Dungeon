using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource cr_audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        cr_audioSource = GetComponent<AudioSource>();    
    }


}
