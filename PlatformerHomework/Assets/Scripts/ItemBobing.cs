using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBobing : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float bobbingTime = 1f;
    public Vector3 spinningSpeed;

    AudioSource crystalSource;

    Vector3 startPosition;
    bool goDown;
    Vector3 smoothSpeed;
    float startTime;


    Syphon syphonScript;

    bool stopBobbing = false;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopBobbing)
        {
            return;
        }
        int sign = goDown ? -1 : 1;
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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && syphonScript == null)
        {
            // add the syphon script to this item if they entered the players trigger 
            syphonScript = gameObject.AddComponent<Syphon>();
            syphonScript.player = other.transform;
            Debug.Log("syphoncreated");
            stopBobbing = true;
        }
    }
}
