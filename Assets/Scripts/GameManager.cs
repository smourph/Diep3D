using UnityEngine;

namespace Diep3D
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private bool m_lockCursor = true;

        void Start()
        {
            Cursor.visible = !m_lockCursor;
            Cursor.lockState = m_lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }

#if UNITY_EDITOR
        void Update()
        {
            // pressing esc toggles between hide/show
            if (Input.GetKeyDown(KeyCode.C))
            {
                m_lockCursor = !m_lockCursor;
            }
            Cursor.visible = !m_lockCursor;
            Cursor.lockState = m_lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }
#endif
    }
}
