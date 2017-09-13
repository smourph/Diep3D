using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Diep3D
{
    public class PlayerController : MonoBehaviour
    {
        private Tank.Tank m_tank;
        private Camera m_followingCamera;
        private Vector3 m_engineDirection;
        private Vector3 m_gunDirection;
        private bool m_jump;
        private bool m_fire;

        private void Awake()
        {
            m_tank = GetComponentInChildren<Tank.Tank>();

            // Get the transform of the main camera
            if (Camera.main != null)
            {
                m_followingCamera = Camera.main;
            }
            else
            {
                Debug.LogWarning("Warning: no main camera found. Tank needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            }
        }

        private void OnEnable()
        {
            // Reset any pre-existent movement
            m_engineDirection = Vector3.zero;
        }

        private void Update()
        {
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
            // Move the engine
            m_tank.Move(m_engineDirection, m_jump);

            // Turn the turret
            m_tank.m_turret.GetComponent<Tank.Turret>().Move(m_followingCamera.transform.forward);

            // Turn the barrel
            m_tank.m_gun.GetComponent<Tank.Gun>().Move(m_followingCamera.transform.eulerAngles);

            // Shoot
            if (m_fire)
            {
                m_tank.m_bulletLauncher.GetComponent<Tank.BulletLauncher>().Fire();
            }
        }
    }
}
