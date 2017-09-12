using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Diep3D.Tank
{
    public class Turret : MonoBehaviour
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
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, m_followingCamera.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
