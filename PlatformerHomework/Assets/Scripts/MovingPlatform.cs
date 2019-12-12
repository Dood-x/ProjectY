using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float frontBackAmplitude;
    public float frontBackTime;
    [Space(10)]
    public float upDownAmplitude;
    public float upDownTime;


    bool goDown;
    bool goBack;
    float startTimeFrontBack;
    float startTimeUpDown;
    Vector3 startPosition;

    //CharacterController playercc;


    //Vector3 previousPos;
    
    // Start is called before the first frame update
    void Start()
    {
        startTimeFrontBack = Time.time;
        startTimeUpDown = Time.time; 
         startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //previousPos = transform.position;
        //move updown
        if (upDownAmplitude > 0 && upDownTime > 0)
        {
            int sign = goDown ? -1 : 1;
            Vector3 newPos = transform.position;
            //TODO instead of time difference, measure time passed from 0
            float t = (Time.time - startTimeUpDown) / upDownTime;
            newPos.y = Mathf.SmoothStep((startPosition + Vector3.up * upDownAmplitude * sign * -1f).y, (startPosition + Vector3.up * upDownAmplitude * sign).y, t);
            transform.position = newPos;
            if (sign * (transform.position.y - startPosition.y) >= upDownAmplitude - 0.001f)
            {
                transform.position = startPosition + Vector3.up * upDownAmplitude * sign;
                goDown = !goDown;
                startTimeUpDown = Time.time;
            }
        }

        if (frontBackAmplitude > 0 && frontBackTime > 0)
        {
            int sign = goBack ? -1 : 1;
            Vector3 newPos = transform.position;
            float t = (Time.time - startTimeFrontBack) / frontBackTime;
            newPos.z = Mathf.SmoothStep((startPosition + Vector3.forward * frontBackAmplitude * sign * -1f).z, (startPosition + Vector3.forward * frontBackAmplitude * sign).z, t);
            transform.position = newPos;
            if (sign * (transform.position.z - startPosition.z) >= frontBackAmplitude - 0.001f)
            {
                transform.position = startPosition + Vector3.forward * frontBackAmplitude * sign;
                goBack = !goBack;
                startTimeFrontBack = Time.time;
            }
        }


        //Vector3 offset = transform.position - previousPos;
        //if (playercc)
        //{
        //    playercc.Move(offset);
        //}

    }

    //public void SetPlayerOnPlatform(CharacterController cc)
    //{
    //    playercc = cc;
    //}

    //public void RemovePlayerOnPlatform()
    //{
    //    playercc = null;
    //}

    //public void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        other.transform.parent = transform;
    //    }
    //}

    //public void OnCollisionExit(Collision other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        other.transform.parent = null;
    //    }
    //}
}
