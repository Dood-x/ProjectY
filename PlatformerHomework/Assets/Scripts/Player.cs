﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlatformerHomework;

public class Player : MonoBehaviour
{

    private CharacterController cc;
    public PlatformerCamera camScript;
    private Animator animator;

    private Camera cam;

    public float walkSpeed = 1.5f;
    public float runSpeed = 6f;
    public float rotSpeed = 20f;
    public float jumpSpeed = 20f;
    public float gravity = 9.81f;

    float vSpeed = 0f;
    int jumpAmount = 0;

    Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
		animator = GetComponent<Animator> ();

        if (camScript != null)
        {
            cam = camScript.gameObject.GetComponent<Camera>();
        }
        else
        {
            cam = Camera.main;
        }

        moveDirection = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Horizontal");
        float h = 0;

        float animSpeed = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));

        animator.SetFloat("Speed", animSpeed);

        //Vector3 moveDirection = Vector3.zero;


        if (animSpeed > 0.1f)
        {
            Vector3 moveHorizontal = new Vector3(h, 0, v);
            // direction on the XZ plane determined by the inputs
            // make the direction value lie on a unit circle
            moveHorizontal.Normalize();
            // the strength of the movement is as big as the highest input
            moveHorizontal *= Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));

            // buffer, input should exceed 0.1 for the character to move
            if (animSpeed < 0.1)
            {
                moveHorizontal = Vector3.zero;
            }


            // rotate the direction by the cameras local y rotation
            //moveHorizontal = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * moveDirection;

            // we linearly interpolate between the walk and run speeds using the animation factor thats in the 0-1 interval
            float movementSpeed = Mathf.Lerp(walkSpeed, runSpeed, animSpeed);

            moveHorizontal *= Time.deltaTime * movementSpeed;

            // move the character
            //cc.Move(moveHorizontal);

            //turn the movement direction into a rotation
            Quaternion rotation = Quaternion.LookRotation(moveHorizontal.normalized);
            // rotate the characetr in the direction of movement
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed*Time.deltaTime);

            moveDirection.x = moveHorizontal.x;
            moveDirection.z = moveHorizontal.z;
        }

        //Vector3 crossForward = Vector3.Cross(this.transform.forward, Vector3.forward);
        ////if(crossForward)
        //if (crossForward.y > 0.0f)
        //{
        //    camScript.lookoffset.z = Mathf.Abs(camScript.lookoffset.z);
        //}
        //else if (crossForward.y < 0.0f)
        //{
        //    camScript.lookoffset.z = -Mathf.Abs(camScript.lookoffset.z);

        //}
        //Debug.Log(crossForward);

        if (Input.GetButtonDown("Jump") && jumpAmount < 2)
        {
            Debug.Log("Jumped");
            vSpeed = jumpSpeed;
            jumpAmount++;
        }

        if (cc.isGrounded)
        {
            jumpAmount = 0;
        }

        vSpeed -= gravity * Time.deltaTime;
        moveDirection.y = vSpeed * Time.deltaTime;
        Debug.Log(moveDirection.y);
        cc.Move(moveDirection);

    }
}
