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

        // Update is called once per frame
        void Update()
        {
            // transform.Rotate(new Vector3(1f * (float) xRotation, 1f * (float) yRotation, 1f * (float) zRotation));
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
