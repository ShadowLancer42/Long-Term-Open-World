using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonMovement : MonoBehaviour
{
    //variables
    #region
    public GameObject characterModel;
    public CharacterController controller;
    public Transform cam;

    private float speed;
    [Space(10)]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    [Space(10)]
    public float stamina = 100;
    public float currentStamina;
    private bool healStamina = true;
    private bool exausted = false;
    [Space(10)]
    public float staminaUseSpeed = 1;
    public float staminaHealSpeed = 3;
    public float exaustedHealSpeed = 2;
    public Slider staminaDisplay;
    [Space(10)]

    [Space(20)]
    public float gravity = -9.81f;
    public float yVelocityReset = -2f;
    [Space(10)]
    public float jump = 10f;
    public float killFallDist = 30f;
    [Space(10)]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    [Space(10)]
    Vector3 velocity;
    bool isGrounded;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    string animState = "Idle";
    Animator anim;

    bool moving = false;

    bool fallMeasured = false;
    float startFall;
    float endFall;
    float fallDist;
    [Space]
    public bool playerIsControllable = true;
    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();

        //lock da' cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //stamina setup
        staminaDisplay.maxValue = stamina;
        currentStamina = stamina;

    }


    // Update is called once per frame
    void Update()
    {


        //reset all animations to false
        #region
        anim.SetBool("IsWalking", false);
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsJumping", false);
        anim.SetBool("Idle", false);
        moving = false;
        #endregion

        //ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //fall damage check
        #region
        //checks the y position the player is at when they fall
        if (!isGrounded && !fallMeasured)
        {
            fallMeasured = true;
            startFall = groundCheck.position.y;
        }
        //called once the player lands again
        if (isGrounded && fallMeasured)
        {
            fallMeasured = false;
            endFall = groundCheck.position.y;
            //fall equation
            fallDist = Mathf.Abs(Mathf.Abs(startFall) - Mathf.Abs(endFall));
            //if the fall distance was enough to kill, kill.
            if (fallDist > killFallDist)
            {
                die();
            }

        }
        #endregion


        //set velocity to a small value when the player is grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = yVelocityReset;
        }


        //jump thing
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += jump;
            moving = true;
            bool jumping = true;
            anim.SetBool("IsJumping", jumping);
        }
        //jump anim backup
        if (!isGrounded)
        {
            bool jumping = true;
            anim.SetBool("IsJumping", jumping);
        }



        //Create a vector3 named 'direction' based on your movement inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //set heal stamina true
        healStamina = true;

        //change speed (and animation!) based on if the player is pressing down sprint key
        if (horizontal > 0.01f || horizontal < -0.1f || vertical > 0.01f || vertical < -0.1f && isGrounded)
        {
            moving = true;
            //sprint if you have stamina and press sprint
            if (Input.GetButton("Sprint") && !exausted)
            {
                speed = runSpeed;
                bool running = true;
                anim.SetBool("IsRunning", running);
                healStamina = false;    //don't heal stamina whilest running    //this gets reset to true every loop of the update function
                currentStamina -= staminaUseSpeed * Time.deltaTime;
            }
            //if you don't have stamina or aren't pressing sprint, just walk
            else
            {
                speed = walkSpeed;
                bool walking = true;
                anim.SetBool("IsWalking", walking);
            }
        }
        if (currentStamina <= 0)
        {
            exausted = true;
        }

        //heal stamina
        if (healStamina == true && currentStamina < stamina)
        {
            //regular heal speed
            if (!exausted)
            {
                currentStamina += staminaHealSpeed * Time.deltaTime;
            }
            //exausted heal speed
            else
            {
                currentStamina += exaustedHealSpeed * Time.deltaTime;
            }
            //set exausted to false if you heal up all the way
            if (currentStamina >= stamina)
            {
                exausted = false;
            }
        }
        staminaDisplay.value = currentStamina;


        //idle
        if (!moving)
        {
            bool Idle = true;
            anim.SetBool("Idle", Idle);
        }

        //if the movement is noticable, do movement calculations
        if (direction.magnitude >= 0.1f && playerIsControllable)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }



        //gravity thing
        velocity.y += gravity * Time.deltaTime * Time.deltaTime;

        //move the player if she's controllable
        if (playerIsControllable)
        {
            controller.Move(velocity);
        }

    }

    void die()
    {
        anim.SetTrigger("Die");
        playerIsControllable = false;
        Debug.Log("you died");
    }

}
