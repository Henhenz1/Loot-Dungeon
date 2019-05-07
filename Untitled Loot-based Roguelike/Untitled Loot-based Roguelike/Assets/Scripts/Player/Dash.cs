using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dash
{
    public float dashDistance;
    
    [Tooltip("Should be less than dashDuration")]
    public float dashCooldown;

    [Tooltip("Should be more than dashCooldown")]
    public float dashDuration;

    [System.NonSerialized]
    public float dashCDTimer;

    public AnimationClip[] dashAnim;

    public AfterImage afterImagePrefab;

    public Dash()
    {
        dashCDTimer = 0;
    }
}
