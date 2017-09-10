using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Diep3D.Tank
{
    public class BodyMovement : MonoBehaviour
	{
        private Tank m_tank;
        private Vector3 m_moveVector;
        private Transform m_followingCamera;
        private bool m_jump;

		private void Awake()
		{
			m_tank = GetComponent<Tank>();

			// Get the transform of the main camera
			if (Camera.main != null)
			{
				m_followingCamera = Camera.main.transform;
			}
			else
			{
				Debug.LogWarning("Warning: no main camera found. Tank needs a Camera tagged \"MainCamera\", for camera-relative controls.");
			}
		}

		private void OnEnable()
		{
            // Reset any pre-existent movement
            m_moveVector = Vector3.zero;
		}

		private void Update()
		{
			// Get the axis
			float h = CrossPlatformInputManager.GetAxis("Horizontal");
			float v = CrossPlatformInputManager.GetAxis("Vertical");

            // Check if jump is pressed
			m_jump = CrossPlatformInputManager.GetButton("Jump");

			if (m_followingCamera != null)
			{
				// Calculate camera relative direction to move:
				Vector3 camForward = Vector3.Scale(m_followingCamera.forward, new Vector3(1, 0, 1)).normalized;
				m_moveVector = (v * camForward + h * m_followingCamera.right).normalized;
			}
			else
			{
				// We use world-relative directions in the case of no main camera
				m_moveVector = (v * Vector3.forward + h * Vector3.right).normalized;
			}
		}


		private void FixedUpdate()
		{
			m_tank.Move(m_moveVector, m_jump);
		}
    }
}
