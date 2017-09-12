using UnityEngine;

namespace Diep3D.Tank
{
    public class Gun : MonoBehaviour
    {
        private Transform m_followingCamera;

        private void Awake()
        {
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

        private void Update()
        {
        }


        private void FixedUpdate()
        {
            // The gun follow the ascent and descent of the aim (betwen -15° and 30° up)
            if (m_followingCamera.eulerAngles.x <= 5f || m_followingCamera.eulerAngles.x >= 285f) // -15 to 75°
			{
				transform.eulerAngles = new Vector3(m_followingCamera.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
			}
			else if (m_followingCamera.eulerAngles.x < 270f) // -90° to -15
            {
                transform.eulerAngles = new Vector3(5f, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (m_followingCamera.eulerAngles.x < 285f) // 75° to 90°
			{
                transform.eulerAngles = new Vector3(285f, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }
}
