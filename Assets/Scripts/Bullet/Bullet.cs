using UnityEngine;

namespace Diep3D.Bullet
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject particleEffect;

        void OnCollisionEnter(Collision collision)
        {
#if UNITY_EDITOR
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white, 10, false);
            }
#endif

            if (collision.gameObject.tag != "Player")
            {
                ContactPoint contact = collision.contacts[0];
                SpawnParticleEffect(contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
                Destroy(gameObject);
            }
        }

        void SpawnParticleEffect(Vector3 position, Quaternion rotation)
        {
            Instantiate(particleEffect, position, rotation);
        }
    }
}