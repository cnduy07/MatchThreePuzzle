using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public float lifeTime = 1;
    public ParticleSystem[] m_AllParticles;

    // Start is called before the first frame update
    void Start()
    {
        m_AllParticles = GetComponentsInChildren<ParticleSystem>();
        Destroy(gameObject, lifeTime);
    }

    public void Play()
    {
        if (m_AllParticles != null)
        {
            foreach (ParticleSystem particle in m_AllParticles)
            {
                if (particle != null)
                {
                    particle.Stop();
                    particle.Play();
                }
            }
        }
    }
}
