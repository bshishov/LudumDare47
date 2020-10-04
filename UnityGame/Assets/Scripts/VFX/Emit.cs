using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emit : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    void Start()
    {
        ParticleSystem = ParticleSystem.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ParticleSystem)
        {
            ParticleSystem.Emit(1);
        }
    }
}
