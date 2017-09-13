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
            // The gun follow the ascent and descent of the aim (betwen 0° and 45° up)
            if (m_followingCamera.eulerAngles.x < 270f) // camera look at bottom or behind (shall it be possible ?)
            {
                transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (m_followingCamera.eulerAngles.x <= 315f) // 45° to 90°
            {
                transform.eulerAngles = new Vector3(315f, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (m_followingCamera.eulerAngles.x > 315f) // 0 to 45°
            {
                transform.eulerAngles = new Vector3(m_followingCamera.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }
}
