using UnityEngine;

namespace Diep3D.Tank
{
    public class Turret : MonoBehaviour
    {
        public void Move(Vector3 forwardDirection)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.Scale(forwardDirection, new Vector3(1, 0, 1)));
        }
    }
}
