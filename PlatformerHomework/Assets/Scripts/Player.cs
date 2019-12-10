using System.Collections;
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
    //the input will be remembered but the jump will be delayed

    public int maxJumpAmount = 2;

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
        animator.SetBool("Grounded", cc.isGrounded);
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
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed * Time.deltaTime);

            moveDirection.x = moveHorizontal.x;
            moveDirection.z = moveHorizontal.z;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        //switches the camera look offset depending on whether we are going forward or back
        //SwitchDirection(v); ubija me, ne treba za sad


        if (cc.isGrounded)
        {
            jumpAmount = 0;
            moveDirection.y = 0f;
            vSpeed = -0.1f;
        }
        else
        {
            vSpeed -= gravity * Time.deltaTime;
            // speed of movement * time is the amount we need to move the character
            
        }

        //jumping allowes double jump
        if (Input.GetButtonDown("Jump") && jumpAmount < maxJumpAmount)
        {
            animator.SetBool("Leap", true);
        }

        moveDirection.y = vSpeed * Time.deltaTime;

        cc.Move(Vector3.forward * 0.001f);

        // move the character!
        cc.Move(moveDirection);

    }


    void SwitchDirection(float v)
    {
        Vector3 lookOffset = camScript.Lookoffset;

        if (v > 0)
        {
            lookOffset.z = Mathf.Abs(lookOffset.z);
            camScript.Lookoffset = lookOffset;
        }
        else if (v < 0)
        {
            lookOffset.z = -Mathf.Abs(lookOffset.z);
            camScript.Lookoffset = lookOffset;
        }
    }

    public void Leap()
    {
        vSpeed = jumpSpeed;
        jumpAmount++;
        cc.Move(Vector3.up * vSpeed * Time.deltaTime);
        animator.SetBool("Leap", false);
        Debug.Log("Leap");

    }
}
