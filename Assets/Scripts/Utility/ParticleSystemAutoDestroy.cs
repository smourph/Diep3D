using System.Collections;
using UnityEngine;

namespace Diep3D.Utility
{
    public class ParticleSystemAutoDestroy : MonoBehaviour
    {
        private float m_MaxLifetime;
        private bool m_EarlyStop;

        private IEnumerator Start()
        {
            var systems = GetComponentsInChildren<ParticleSystem>();

            // find out the maximum lifetime of any particles in this effect
            foreach (var system in systems)
            {
                m_MaxLifetime = Mathf.Max(system.main.startLifetime.constant, m_MaxLifetime);
            }

            // wait for any remaining particles to expire
            yield return new WaitForSeconds(m_MaxLifetime);

            Destroy(gameObject);
        }


        public void Stop()
        {
            // stops the particle system early
            m_EarlyStop = true;
        }
    }
}
