using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private float baseIntensity;
    [SerializeField] private int baseWidth;
    [SerializeField] private int baseHeight;

    // Start is called before the first frame update
    void Start()
    {
        float baseRatio = (float)baseWidth / baseHeight;
        if (volume.profile.TryGetSettings(out ChromaticAberration ca))
        {
            ca.intensity.value = ((float)baseRatio / ((float)Screen.width / Screen.height)) * baseIntensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
