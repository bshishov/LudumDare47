using UnityEngine;

public class ReverceParticleTime : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public bool reverce;
    void Start()
    {
        ParticleSystem.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ParticleSystem && reverce)
        {
            ParticleSystem.time = ParticleSystem.time - Time.deltaTime/2;
        }
    }
}
