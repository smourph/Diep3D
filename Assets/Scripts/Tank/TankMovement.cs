using UnityEngine;
using UnityEngine.Networking;

namespace Diep3D.Tank
{
    public class TankMovement : NetworkBehaviour
    {
        // Used to identify which tank belongs to which player
        [HideInInspector] public int m_PlayerNumber = 1;
        [HideInInspector] public Rigidbody m_Rigidbody;
        public GameObject m_Turret;
        public GameObject m_Gun;
        public float m_MovePower = 15f;
        public float m_JumpPower = 1.5f;

        private Camera m_followingCamera;
        private string m_VerticalAxis;
        private string m_HorizontalAxis;
        private string m_JumpButton;
        private Vector3 m_CameraForward;
        private float m_VerticalInput;
        private float m_HorizontalInput;
        private bool m_JumpInput;

        private Transform m_TurretTransform;
        private Transform m_GunTransform;

        // The length of the ray to check if the tank is grounded.
        private const float m_GroundCheckDistance = 0.51f;

        protected RigidbodyConstraints m_OriginalConstrains;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_TurretTransform = m_Turret.GetComponent<Transform>();
            m_GunTransform = m_Gun.GetComponent<Transform>();
        }

        private void Start()
        {
            m_VerticalAxis = "Vertical";
            m_HorizontalAxis = "Horizontal";
            m_JumpButton = "Jump";
            m_followingCamera = Camera.main;
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            // Store the value of both input axes.
            m_VerticalInput = Input.GetAxis(m_VerticalAxis);
            m_HorizontalInput = Input.GetAxis(m_HorizontalAxis);

            // Check if jump is pressed
            m_JumpInput = Input.GetButton(m_JumpButton);

            // Calculate camera direction:
            m_CameraForward = Vector3.Scale(m_followingCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;

            Move();
            Turn();
            Jump();
        }

        /// <summary>
        /// Move body
        /// </summary>
        private void Move()
        {
            Vector3 engineDirection = (m_VerticalInput * m_CameraForward + m_HorizontalInput * m_followingCamera.transform.right).normalized;
            m_Rigidbody.AddForce(engineDirection * m_MovePower);
        }

        /// <summary>
        /// Turn turret and gun
        /// </summary>
        private void Turn()
        {
            // Rotate left/right
            Quaternion cameraRotation = Quaternion.LookRotation(Vector3.Scale(m_followingCamera.transform.forward, new Vector3(1, 0, 1)));
            m_TurretTransform.rotation = cameraRotation;

            // Rotate up/down
            float cameraUpAngle = (m_followingCamera.transform.eulerAngles.x + 90) % 360;
            Debug.Log(cameraUpAngle);
            // Angles between -6.5° and 52.5° are the only allowed angles
            if (cameraUpAngle >= 37.5f || cameraUpAngle <= 96.5f) // -6.5° to 52.5°
            {
                m_GunTransform.eulerAngles = new Vector3(cameraUpAngle, m_GunTransform.eulerAngles.y, m_GunTransform.eulerAngles.z);
            }
            else if (cameraUpAngle > 96.5f) // -90° to -6.5°
            {
                m_GunTransform.eulerAngles = new Vector3(96.5f, m_GunTransform.eulerAngles.y, m_GunTransform.eulerAngles.z);
            }
            else if (cameraUpAngle < 37.5f) // 52.5° to 90°
            {
                m_GunTransform.eulerAngles = new Vector3(37.5f, m_GunTransform.eulerAngles.y, m_GunTransform.eulerAngles.z);
            }
        }

        /// <summary>
        /// Jump !
        /// </summary>
        private void Jump()
        {
            // If not going down (approximation to -0.01...) and close to the ground and jump is pressed...
            if (m_Rigidbody.velocity.y >= -0.01 && CheckGroundStatus() && m_JumpInput)
            {
                m_Rigidbody.AddForce(Vector3.up * m_JumpPower, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// This function is called to make sure each tank is set up correctly.
        /// </summary>
        public void SetDefaults()
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;

            m_VerticalInput = 0f;
            m_HorizontalInput = 0f;
        }

        /// <summary>
        /// Detect if tank is on the ground or under other collider
        /// </summary>
        bool CheckGroundStatus()
        {
            RaycastHit hitInfo;

            // 0.01f is a small offset to start the ray from inside the character because the transform position in the tank at its base
            if (Physics.Raycast(transform.position + (Vector3.up * 0.01f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                return true;
            }
            
            return false;
        }

        // Freeze rigidbody to avoid tank drifting
        void OnDisable()
        {
            m_OriginalConstrains = m_Rigidbody.constraints;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Reactivate rigidbody
        void OnEnable()
        {
            m_Rigidbody.constraints = m_OriginalConstrains;
        }
    }
}