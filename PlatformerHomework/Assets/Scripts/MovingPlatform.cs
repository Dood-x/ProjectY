using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float frontBackAmplitude;
    public float frontBackTime;
    public bool startForward = true;
    [Space(10)]
    public float upDownAmplitude;
    public float upDownTime;
    public bool startUp = true;



    bool goDown;
    bool goBack;
    float startTimeFrontBack;
    float startTimeUpDown;
    Vector3 startPosition;

    Vector3 movementDelta;

    Rigidbody rb;

    //CharacterController playercc;


    Vector3 previousPos;
    
    // Start is called before the first frame update
    void Start()
    {
        startTimeFrontBack = Time.time;
        startTimeUpDown = Time.time; 
        startPosition = transform.position;
        previousPos = startPosition;
        rb = GetComponent<Rigidbody>();

        if (!startForward)
            goBack = true;

        if (!startUp)
            goDown = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (rb)
        {
            movementDelta = transform.position - previousPos;
            previousPos = transform.position;
            return;
        }


        movementDelta = Vector3.zero;
        previousPos = transform.position;
        //previousPos = transform.position;
        //move updown
        if (upDownAmplitude > 0 && upDownTime > 0)
        {
            int sign = goDown ? -1 : 1;
            Vector3 newPos = transform.position;
            //TODO instead of time difference, measure time passed from 0
            float t = (Time.time - startTimeUpDown) / upDownTime;
            newPos.y = Mathf.SmoothStep((startPosition + Vector3.up * upDownAmplitude * sign * -1f).y, (startPosition + Vector3.up * upDownAmplitude * sign).y, t);
            transform.Translate(newPos - transform.position);
            //transform.position = newPos;
            if (sign * (transform.position.y - startPosition.y) >= upDownAmplitude - 0.001f)
            {
                Vector3 endPos = startPosition + Vector3.up * upDownAmplitude * sign;
                transform.Translate(endPos - transform.position);
                //transform.position = startPosition + Vector3.up * upDownAmplitude * sign;
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
            transform.Translate(newPos - transform.position);
            //transform.position = newPos;
            if (sign * (newPos.z - startPosition.z) >= frontBackAmplitude - 0.001f)
            {
                Vector3 endPos = startPosition + Vector3.forward * frontBackAmplitude * sign;
                transform.Translate(endPos - transform.position);

                //transform.position = startPosition + Vector3.forward * frontBackAmplitude * sign;
                goBack = !goBack;
                startTimeFrontBack = Time.time;
            }
        }

        movementDelta = transform.position - previousPos;
        //if (playercc)
        //{
        //    playercc.Move(movementDelta);
        //}

    }

    //void FixedUpdate()
    //{
    //    RBUpdate();
    //}

    //void RBUpdate()
    //{
    //    if (!rb)
    //        return;

    //    //previousPos = transform.position;

    //    if (upDownAmplitude > 0 && upDownTime > 0)
    //    {
    //        int sign = goDown ? -1 : 1;
    //        Vector3 newPos = transform.position;
    //        //TODO instead of time difference, measure time passed from 0
    //        float t = (Time.time - startTimeUpDown) / upDownTime;
    //        newPos.y = Mathf.SmoothStep((startPosition + Vector3.up * upDownAmplitude * sign * -1f).y, (startPosition + Vector3.up * upDownAmplitude * sign).y, t);
    //        rb.MovePosition(newPos);
    //        if (sign * (transform.position.y - startPosition.y) >= upDownAmplitude - 0.001f)
    //        {
    //            rb.MovePosition(startPosition + Vector3.up * upDownAmplitude * sign);
    //            goDown = !goDown;
    //            startTimeUpDown = Time.time;
    //        }
    //    }

    //    if (frontBackAmplitude > 0 && frontBackTime > 0)
    //    {
    //        int sign = goBack ? -1 : 1;
    //        Vector3 newPos = transform.position;
    //        float t = (Time.time - startTimeFrontBack) / frontBackTime;
    //        newPos.z = Mathf.SmoothStep((startPosition + Vector3.forward * frontBackAmplitude * sign * -1f).z, (startPosition + Vector3.forward * frontBackAmplitude * sign).z, t);
    //        //transform.position = newPos;
    //        rb.MovePosition(newPos);
    //        if (sign * (newPos.z - startPosition.z) >= frontBackAmplitude - 0.001f)
    //        {
    //            Vector3 endPos = startPosition + Vector3.forward * frontBackAmplitude * sign;
    //            rb.MovePosition(startPosition + Vector3.forward * frontBackAmplitude * sign);
    //            goBack = !goBack;
    //            startTimeFrontBack = Time.time;
    //        }
    //    }

    //    //movementDelta = rb.velocity * Time.fixedDeltaTime;
    //}

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

    public Vector3 GetMovementDelta()
    {
        return movementDelta;
    }
    public void ClearnMovementDelta()
    {
        movementDelta = Vector3.zero;
    }
}
