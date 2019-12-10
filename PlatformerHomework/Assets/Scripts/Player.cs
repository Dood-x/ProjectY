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
    }

    // Update is called once per frame
    void Update()
    {
        //float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Horizontal");
        float h = 0;

        float animSpeed = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));

        animator.SetFloat("Speed", animSpeed);

        if (animSpeed > 0.1f)
        {
            // direction on the XZ plane determined by the inputs
            Vector3 moveDirection = new Vector3(h, 0f, v);
            // make the direction value lie on a unit circle
            moveDirection.Normalize();
            // the strength of the movement is as big as the highest input
            moveDirection *= Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));

            // buffer, input should exceed 0.1 for the character to move
            if (animSpeed < 0.1)
            {
                moveDirection = Vector3.zero;
            }


            // rotate the direction by the cameras local y rotation
            //moveDirection = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * moveDirection;

            // we linearly interpolate between the walk and run speeds using the animation factor thats in the 0-1 interval
            float movementSpeed = Mathf.Lerp(walkSpeed, runSpeed, animSpeed);

            moveDirection *= Time.deltaTime * movementSpeed;

            // move the character
            cc.Move(moveDirection);

            //turn the movement direction into a rotation
            Quaternion rotation = Quaternion.LookRotation(moveDirection.normalized);
            // rotate the characetr in the direction of movement
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, rotSpeed*Time.deltaTime);
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
    }
}
