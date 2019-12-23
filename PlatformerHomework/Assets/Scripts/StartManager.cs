using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public Camera camera;

    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;

    void Update()
    {
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        float v = verticalSpeed * Input.GetAxis("Mouse Y");
        h /= 13;
        v /= 13;

        camera.transform.Rotate(v, h, 0);
    }
}
