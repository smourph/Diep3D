using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace Diep3D
{
    public class PlayerController : NetworkBehaviour
    {
        private Tank.Tank m_tank;
        private Camera m_followingCamera;
        private Vector3 m_engineDirection;
        private bool m_jump;
        private bool m_fire;

        private void OnEnable()
        {
            // Reset any pre-existent movement
            m_engineDirection = Vector3.zero;

            // Load player tank
            m_tank = GetComponentInChildren<Tank.Tank>();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            // Apply "Player" tag
            gameObject.tag = "LocalPlayer";

            m_tank.tag = "Player";

            if (Camera.main != null)
            {
                // Get the transform of the main camera
                m_followingCamera = Camera.main;
            }
            else
            {
                Debug.LogWarning("Warning: no main camera found. Tank needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            }

            // Apply player material on tank meshes 
            m_tank.m_hull.GetComponent<MeshRenderer>().material = Resources.Load("Player", typeof(Material)) as Material;
            m_tank.m_turret.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Player", typeof(Material)) as Material;
            m_tank.m_gun.GetComponentInChildren<MeshRenderer>().material = Resources.Load("Player", typeof(Material)) as Material;
        }

        private void Update()
        {
            // Only for the local player
            if (!isLocalPlayer)
            {
                return;
            }

            // Get the axis
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            // Check if jump is pressed
            m_jump = CrossPlatformInputManager.GetButton("Jump");

            // Check if fire is pressed
            m_fire = CrossPlatformInputManager.GetButton("Fire1");

            if (m_followingCamera != null)
            {
                // Calculate camera relative direction to move:
                Vector3 camForward = Vector3.Scale(m_followingCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
                m_engineDirection = (v * camForward + h * m_followingCamera.transform.right).normalized;
            }
            else
            {
                // We use world-relative directions in the case of no main camera
                m_engineDirection = (v * Vector3.forward + h * Vector3.right).normalized;
            }
        }


        private void FixedUpdate()
        {
            // Only for the local player
            if (!isLocalPlayer)
            {
                return;
            }

            // If tank is defined
            if (m_tank)
            {
                // Move the engine
                m_tank.Move(m_engineDirection, m_jump);

                // Turn the turret
                m_tank.m_turret.GetComponent<Tank.Turret>().Move(m_followingCamera.transform.forward);

                // Turn the barrel
                m_tank.m_gun.GetComponent<Tank.Gun>().Move(m_followingCamera.transform.eulerAngles);

                // Shoot
                if (m_fire)
                {
                    CmdFire();
                }
            }
        }

        // This [Command] code is called on the Client but it is run on the Server!
        [Command]
        public void CmdFire()
        {
            Tank.BulletLauncher launcher = m_tank.m_bulletLauncher.GetComponent<Tank.BulletLauncher>();

            if (Time.time > launcher.NextFire)
            {
                launcher.NextFire = Time.time + launcher.m_fireRate;

                GameObject bullet = (GameObject)Instantiate(launcher.m_ammunition,
                                                            m_tank.m_bulletLauncher.transform.position,
                                                            m_tank.m_bulletLauncher.transform.rotation);

                if (isLocalPlayer)
                {
                    bullet.GetComponent<MeshRenderer>().material = Resources.Load("PlayerBullet", typeof(Material)) as Material;
                }

                // Spawn the bullet on the Clients
                NetworkServer.Spawn(bullet);

                bullet.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(m_tank.m_bulletLauncher.transform.forward * launcher.m_ejectSpeed));
            }
        }
    }
}