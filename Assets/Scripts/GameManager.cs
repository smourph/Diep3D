using UnityEngine;

namespace Diep3D
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private bool lockCursor = true;

        void Start()
        {
            Cursor.visible = !lockCursor;
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }

#if UNITY_EDITOR
        void Update()
        {
            // pressing esc toggles between hide/show
            if (Input.GetKeyDown(KeyCode.C))
            {
                lockCursor = !lockCursor;
            }
            Cursor.visible = !lockCursor;
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }
#endif
    }
}
