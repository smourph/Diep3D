using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Diep3D.Tank
{
    public class BulletLauncher : MonoBehaviour
    {
        public Rigidbody m_ammunition;

        [SerializeField] private float m_ejectSpeed = 100f;
        [SerializeField] private float m_fireRate = 0.5f;

        private float nextFire = 0.0f;

        public void Fire()
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + m_fireRate;

                Rigidbody bullet = Instantiate(m_ammunition, transform.position, transform.rotation);
                bullet.AddForce(transform.TransformDirection(Vector3.forward * m_ejectSpeed));
            }
        }
    }
}