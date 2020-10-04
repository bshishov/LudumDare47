using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfNotPlaying : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    void Awake()
    {
        ParticleSystem = ParticleSystem.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ParticleSystem)
        {
            if (!ParticleSystem.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}
