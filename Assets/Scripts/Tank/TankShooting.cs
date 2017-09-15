using UnityEngine;
using UnityEngine.Networking;

namespace Diep3D.Tank
{
    public class TankShooting : NetworkBehaviour
    {
        [HideInInspector] public int m_PlayerNumber = 1;
        [HideInInspector] public Color m_PlayerColor;
        public Rigidbody m_Shell;
        public Transform m_BulletLauncher;
        public float m_EjectSpeed = 100f;
        public float m_FireRate = 0.5f;

        private string m_FireButton;
        private Rigidbody m_Rigidbody;
        [SyncVar] private float m_CurrentLaunchForce;
        private float m_NextFire = 0.0f;

        private void Awake()
        {
            // Set up the references.
            m_Rigidbody = GetComponent<Rigidbody>();
        }


        private void Start()
        {
            m_FireButton = "Fire1";
            m_CurrentLaunchForce = m_EjectSpeed;
        }

        [ClientCallback]
        private void Update()
        {
            if (!isLocalPlayer)
                return;

            // If the fire button has just started being pressed...
            if (Input.GetButton(m_FireButton))
            {
                Fire();
            }
        }

        private void Fire()
        {
            if (Time.time > m_NextFire)
            {
                m_NextFire = Time.time + m_FireRate;
                CmdFire(m_Rigidbody.velocity, m_CurrentLaunchForce, m_BulletLauncher.forward, m_BulletLauncher.position, m_BulletLauncher.rotation);
            }
        }

        [Command]
        private void CmdFire(Vector3 rigidbodyVelocity, float launchForce, Vector3 forward, Vector3 position, Quaternion rotation)
        {
            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance = Instantiate(m_Shell, position, rotation) as Rigidbody;

            // Set shell color
            shellInstance.GetComponent<MeshRenderer>().material.color = m_PlayerColor;

            // Create a velocity that is the tank's velocity and the launch force in the fire position's forward direction.
            Vector3 velocity = rigidbodyVelocity + launchForce * forward;

            // Set the shell's velocity to this velocity.
            shellInstance.velocity = velocity;

            NetworkServer.Spawn(shellInstance.gameObject);
        }

        // This is used by the game manager to reset the tank.
        public void SetDefaults()
        {
        }
    }
}