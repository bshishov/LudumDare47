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
            ParticleSystem.Pause();
            if (ParticleSystem.time>0)
            {
                ParticleSystem.time = ParticleSystem.time - Time.deltaTime / 2;
            }
           
        }
    }
}
