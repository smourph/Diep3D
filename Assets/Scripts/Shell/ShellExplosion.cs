using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Diep3D.Shell
{
    public class ShellExplosion : NetworkBehaviour
    {
        public ParticleSystem m_ExplosionParticles;
        public float m_MaxDamage = 100f;
        public float m_ExplosionForce = 1000f;
        public float m_MaxLifeTime = 2f;
        public float m_ExplosionRadius = 5f;

        private int m_TankMask;

        private void Start()
        {
            if (isServer)
            {
                // If it isn't destroyed by then, destroy the shell after it's lifetime.
                Destroy(gameObject, m_MaxLifeTime);
                GetComponent<Collider>().enabled = false;
                StartCoroutine(EnableCollision());
            }

            // Set the value of the layer mask based solely on the Players layer.
            m_TankMask = LayerMask.GetMask("Players");
        }

        //allow to delay a bit the activation of the collider so that it don't collide when spawn close to the canon
        IEnumerator EnableCollision()
        {
            yield return new WaitForSeconds(0.1f);
            GetComponent<Collider>().enabled = true;
        }

        //Trigger are handled only on the server, as it have authority
        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                // ... and find their rigidbody.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                // If they don't have a rigidbody, go on to the next collider.
                if (!targetRigidbody)
                    continue;

                // Find the TankHealth script associated with the rigidbody.
                Tank.TankHealth targetHealth = targetRigidbody.GetComponent<Tank.TankHealth>();

                // If there is no TankHealth script attached to the gameobject, go on to the next collider.
                if (!targetHealth)
                    continue;

                // Create a vector from the shell to the target.
                Vector3 explosionToTarget = targetRigidbody.position - transform.position;

                // Calculate the distance from the shell to the target.
                float explosionDistance = explosionToTarget.magnitude;

                // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
                float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

                // Calculate damage as this proportion of the maximum possible damage.
                float damage = relativeDistance * m_MaxDamage;

                // Make sure that the minimum damage is always 0.
                damage = Mathf.Max(0f, damage);

                // Deal this damage to the tank.
                targetHealth.Damage(damage);
            }

            if (!NetworkClient.active)//if we are ALSO client (so hosting), this will be done by the Destroy so Skip
                PhysicForces();

            // Destroy the shell on clients.
            NetworkServer.Destroy(gameObject);
        }

        //called on client when the Network destroy that object (it was destroyed on server)
        public override void OnNetworkDestroy()
        {

            //we spawn the explosion particle
            ExplodeShell();
            //set the particle to be destroyed at the end of their lifetime
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration);
            base.OnNetworkDestroy();
        }

        void ExplodeShell()
        {
            // Unparent the particles from the shell.
            m_ExplosionParticles.transform.parent = null;

            // Play the particle system.
            m_ExplosionParticles.Play();

            PhysicForces();
        }


        //This apply force on object. Do that on all clients & server as each must apply force to object they own
        void PhysicForces()
        {
            // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                // ... and find their rigidbody.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                // If they don't have a rigidbody or we don't own that object, go on to the next collider.
                if (!targetRigidbody || !targetRigidbody.GetComponent<NetworkIdentity>().hasAuthority)
                    continue;

                // Add an explosion force with no vertical bias.
                targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
            }
        }
    }
}