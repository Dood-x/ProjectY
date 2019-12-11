using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBobing : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float bobbingSpeed = 10f;
    public Vector3 spinningSpeed;


    Vector3 startPositon;
    bool goDown;
    // Start is called before the first frame update
    void Start()
    {
        startPositon = transform.position; 
    }

    // Update is called once per frame
    void Update()
    {

        int sign = goDown ? -1 : 1;
        transform.position += Vector3.up * bobbingSpeed * Time.deltaTime * sign;
        if (Mathf.Abs(transform.position.y - startPositon.y) > amplitude)
        {
            transform.position = startPositon + Vector3.up * amplitude * sign;
            goDown = !goDown;
        }

        transform.Rotate(spinningSpeed * Time.deltaTime, Space.World);
    }
}
