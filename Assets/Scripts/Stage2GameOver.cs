using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Stage2GameOver : MonoBehaviour
{
    public PostProcessVolume bloom;
    public void OnGameClear()
    {
        Bloom b;
        if(bloom.profile.TryGetSettings<Bloom>(out b))
        {
            b.active = false;
        }

    }
}
