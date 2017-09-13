using UnityEngine;

namespace Diep3D.Tank
{
    public class Gun : MonoBehaviour
    {
        public void Move(Vector3 directionEulerAngle)
        {
            if (directionEulerAngle.x <= 6.5f || directionEulerAngle.x >= 307.5f) // -6.5° to 52.5°
            {
                transform.eulerAngles = new Vector3(directionEulerAngle.x, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (directionEulerAngle.x < 270f) // -90° to -6.5°
            {
                transform.eulerAngles = new Vector3(6.5f, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (directionEulerAngle.x < 307.5f) // 53° to 90°
            {
                transform.eulerAngles = new Vector3(307.5f, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }
}
