using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour
{
    public enum PlayerState { Idle, Run, Sprint, Jump, Fall, Exhaust, Dive, Death }
    public enum TargetState { Stand, Run }

    [SerializeField] private float hp = 100;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float turnSpeed = 200;
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;

    public PlayerState playerState;
    public TargetState targetState;
    private Vector3 firstPosition;
    private float currentV = 0;
    private float currentH = 0;

    private readonly float interpolation = 10;
    private readonly float runScale = 1.5f;
    private readonly float backwardRunScale = 0.75f;
    private readonly float backwardWalkScale = 0.5f;

    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;
    private bool isRecovering;

    private float jumpThreshold = 0.0f;
    //private float turnThreshold = 0.0f;
    //private float turnTime = 0.0f;
    private float jumpTimeStamp = 0;
    private float minJumpInterval = 0.25f;
    public float maximumHeight = 0f;
    public Vector3 moveAmount = Vector3.zero;
    private List<Collider> collisions = new List<Collider>();
    void Start()
    {
        firstPosition = this.transform.position;
        targetState = TargetState.Stand;
    }
    void Update()
    {
        animator.SetBool("Grounded", isGrounded);
        if (targetState == TargetState.Run && playerState != PlayerState.Death)
        {
            MoveUpdate();
        }
        CheckState();
        wasGrounded = isGrounded;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!collisions.Contains(collision.collider))
                {
                    collisions.Add(collision.collider);
                }
                isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true;
                break;
            }
        }

        if (validSurfaceNormal)
        {
            isGrounded = true;
            if (!collisions.Contains(collision.collider))
            {
                collisions.Add(collision.collider);
            }
        }
        else
        {
            if (collisions.Contains(collision.collider))
            {
                collisions.Remove(collision.collider);
            }
            if (collisions.Count == 0)
            {
                isGrounded = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisions.Contains(collision.collider))
        {
            collisions.Remove(collision.collider);
        }
        if (collisions.Count == 0)
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && playerState != PlayerState.Exhaust)
        {
            playerState = PlayerState.Dive;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && playerState != PlayerState.Exhaust)
        {
            playerState = PlayerState.Dive;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && playerState != PlayerState.Exhaust)
        {
            playerState = PlayerState.Fall;
        }
    }
    private void MoveUpdate()
    {
        float v = 1.0f;
        float h = 0.2f;
        /*
        if(Time.time > turnThreshold)
        {
            h = -0.1f;
            turnTime += 0.1f;
            if(turnTime >= 10.0f)
            {
                turnThreshold += 10.0f;
                turnTime = 0.0f;
            }
        }
        else
        {
            h = 0.0f;
        }
        */

        bool sprint = Input.GetKey(KeyCode.LeftShift) && playerState != PlayerState.Exhaust;
        bool CanChangeState()
        {
            switch (playerState)
            {
                case PlayerState.Jump:
                case PlayerState.Dive:
                case PlayerState.Exhaust:
                    return false;
                default:
                    return true;
            }
        }

        if (v < 0)
        {
            if (sprint)
            {
                v *= backwardRunScale;
            }
            else
            {
                v *= backwardWalkScale;
            }
        }
        else if (sprint & Mathf.Abs(v) >= 0.1f)
        {
            v *= runScale;
            hp -= 0.2f;
        }
        if (playerState == PlayerState.Dive || playerState == PlayerState.Exhaust)
        {
            v *= 0.5f;
        }
        if (isGrounded && CanChangeState())
        {
            if (Mathf.Abs(v) < 0.1)
            {
                playerState = PlayerState.Idle;
            }
            else
            {
                if (sprint)
                {
                    playerState = PlayerState.Sprint;
                }
                else
                {
                    playerState = PlayerState.Run;
                }
            }
        }

        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
        currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

        transform.position += transform.forward * currentV * moveSpeed * Time.deltaTime;
        moveAmount = transform.forward * currentV * moveSpeed;
        transform.Rotate(0, currentH * turnSpeed * Time.deltaTime, 0);

        animator.SetFloat("MoveSpeed", currentV);

        JumpingAndLanding();
    }
    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - jumpTimeStamp) >= minJumpInterval;
        bool jumpInput;
        if(Time.time > jumpThreshold)
        {
            jumpThreshold += 5.0f;
            jumpInput = true;
        }
        else
        {
            jumpInput = false;
        }
        if (jumpCooldownOver && isGrounded && jumpInput)
        {
            if (playerState == PlayerState.Exhaust)
            {
                jumpForce = 6f;
            }
            else
            {
                jumpForce = 12f;
            }
            jumpTimeStamp = Time.time;
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerState = PlayerState.Jump;
            isJumping = true;
        }
        if (!wasGrounded && isGrounded)
        {
            float fallAmount = maximumHeight - this.transform.position.y;
            if (fallAmount > 6.0f || hp < 0.0f)
            {
                hp = -100;
                playerState = PlayerState.Exhaust;
                if (!isRecovering)
                {
                    Invoke("Recover", 10.0f);
                    isRecovering = true;
                }
            }
            animator.SetTrigger("Land");
            maximumHeight = this.transform.position.y;
        }
        if (!isGrounded && wasGrounded)
        {
            animator.SetTrigger("Jump");
            if (isJumping)
            {
                playerState = PlayerState.Jump;
            }
            else
            {
                playerState = PlayerState.Fall;
            }
        }
    }

    private void CheckState()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                maximumHeight = this.transform.position.y;
                if (hp < 100.0f)
                {
                    hp += 0.5f;
                }
                break;
            case PlayerState.Run:
                maximumHeight = this.transform.position.y;
                isJumping = false;
                break;
            case PlayerState.Sprint:
                maximumHeight = this.transform.position.y;
                isJumping = false;
                break;
            case PlayerState.Jump:
                isJumping = true;
                if (maximumHeight <= this.transform.position.y)
                {
                    maximumHeight = this.transform.position.y;
                }
                else
                {
                    playerState = PlayerState.Fall;
                    isJumping = false;
                }
                break;
            case PlayerState.Fall:
                break;
            case PlayerState.Exhaust:
                maximumHeight = this.transform.position.y;
                break;
            case PlayerState.Dive:
                maximumHeight = this.transform.position.y;
                break;
            case PlayerState.Death:
                this.transform.position = firstPosition;
                this.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
                playerState = PlayerState.Fall;
                break;
        }

        if (hp > 100.0f)
        {
            hp = 100.0f;
        }
        if (hp <= 0f && hp > -100f && isGrounded)
        {
            hp = -100f;
            playerState = PlayerState.Exhaust;
            if (!isRecovering)
            {
                Invoke("Recover", 10.0f);
                isRecovering = true;
            }
        }

        if (this.transform.position.y < -15.0f)
        {
            playerState = PlayerState.Death;
        }
    }
    private void Recover()
    {
        hp = 0.1f;
        playerState = PlayerState.Idle;
        isRecovering = false;
    }
}
