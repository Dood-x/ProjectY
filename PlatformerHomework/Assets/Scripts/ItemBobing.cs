using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBobing : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float bobbingTime = 1f;
    public Vector3 spinningSpeed;


    Vector3 startPosition;
    bool goDown;
    Vector3 smoothSpeed;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        int sign = goDown ? -1 : 1;
        //float velocity = bobbingSpeed * Time.deltaTime;
        //transform.position += Vector3.up * bobbingSpeed * Time.deltaTime * sign;
        //transform.position = Vector3.SmoothDamp(transform.position, startPosition + Vector3.up * amplitude * sign, ref smoothSpeed, bobbingSpeed);
        Vector3 newPos = transform.position;
        float t = (Time.time - startTime) / bobbingTime;
        newPos.y = Mathf.SmoothStep((startPosition + Vector3.up * amplitude * sign * -1f).y, (startPosition + Vector3.up * amplitude * sign).y, t);
        transform.position = newPos;
        if (sign*(transform.position.y - startPosition.y) >= amplitude - 0.001f)
        {
            transform.position = startPosition + Vector3.up * amplitude * sign;
            goDown = !goDown;
            startTime = Time.time;
        }

        transform.Rotate(spinningSpeed * Time.deltaTime, Space.World);
        Debug.Log(goDown);
    }
}
