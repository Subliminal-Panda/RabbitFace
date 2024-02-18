using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TM
{
    public class RotateObject : MonoBehaviour
    {
        public float xRotation = 0;
        public float yRotation = 0;
        public float zRotation = 0;

        void Update()
        {
            transform.localRotation *= Quaternion.Euler(xRotation, yRotation, zRotation);
        }

        public void ChangeRotation(Vector3 transform)
        {
            xRotation = transform.x;
            yRotation = transform.y;
            zRotation = transform.z;
        }
    }
}
