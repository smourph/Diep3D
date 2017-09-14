using UnityEngine;

namespace Diep3D.Tank
{
    public class BulletLauncher : MonoBehaviour
    {
        public GameObject m_ammunition;

        public float m_ejectSpeed = 100f;
        public float m_fireRate = 0.5f;

        private float m_nextFire = 0.0f;
        public float NextFire
        {
            get
            {
                return m_nextFire;
            }
            set
            {
                m_nextFire = value;
            }
        }
    }
}