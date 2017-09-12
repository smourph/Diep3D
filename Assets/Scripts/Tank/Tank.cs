using UnityEngine;

namespace Diep3D.Tank
{
    public class Tank : MonoBehaviour
    {
        [SerializeField] private float m_MovePower = 10f;
        [SerializeField] private float m_JumpPower = 0.5f;

        private Rigidbody m_Rigidbody;
        private const float m_GroundCheckDistance = 0.51f; // The length of the ray to check if the tank is grounded.

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            // When the tank is turned on, make sure it's not kinematic.
            m_Rigidbody.isKinematic = false;
        }


        private void OnDisable()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;
        }

        public void Move(Vector3 moveDirection, bool jump)
        {
            // Add force in the move direction.
            m_Rigidbody.AddForce(moveDirection * m_MovePower);

            // If not going down (approximation to -0.01...) and close to the ground and jump is pressed...
            if (m_Rigidbody.velocity.y >= -0.01 && CheckGroundStatus() && jump)
            {
                m_Rigidbody.AddForce(Vector3.up * m_JumpPower, ForceMode.Impulse);
            }
        }

        bool CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.01f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance), Color.white, 10, false);
#endif
            // 0.01f is a small offset to start the ray from inside the character because the transform position in the tank at its base
            if (Physics.Raycast(transform.position + (Vector3.up * 0.01f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
