using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Diep3D.Bullet
{
    public class BulletLauncher : MonoBehaviour
    {
        [SerializeField] private Rigidbody ammunition;
        [SerializeField] private float ejectSpeed;
        [SerializeField] private float fireRate;

        private float nextFire = 0.0f;

        void Update()
        {
            if (CrossPlatformInputManager.GetButton("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;

                Rigidbody bullet = Instantiate(ammunition, transform.position, transform.rotation);
                bullet.AddForce(transform.TransformDirection(Vector3.forward * ejectSpeed));
            }
        }
    }
}