using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class PlayerControl : MonoBehaviour {
    public enum PlayerState { Idle, Run, Sprint, Jump, Fall, Exhaust, Dive, Death }
    [SerializeField] private float hp = 100;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float turnSpeed = 200;
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;

    public PlayerAgent playerAgent;
    public PlayerState playerState;
    private float currentV = 0;
    private float currentH = 0;

    private readonly float interpolation = 10;
    private readonly float runScale = 1.5f;
    private readonly float backwardRunScale = 0.75f;
    private readonly float backwardWalkScale = 0.5f;

    private bool isSprint;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;
    private bool isRecovering;

    private float jumpTimeStamp = 0.0f;
    private float minJumpInterval = 0.25f;
    private float heightBefore = 0.0f;
    private float maximumHeight = 0.0f;
    private float sprintStart = 0.0f;
    public Vector3 moveAmount = Vector3.zero;
    private List<Collider> collisions = new List<Collider>();

    //UI
    //public Slider hpbar;
    public GameObject gameOverPanel;

    void Update()
    {
        animator.SetBool("Grounded", isGrounded);
        if(playerState != PlayerState.Death)
        {
            MoveUpdate();
        }
        CheckState();
        wasGrounded = isGrounded;
    }

    public float GetHP()
    {
        return hp;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!collisions.Contains(collision.collider) && !collision.gameObject.CompareTag("Item"))
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

        if(validSurfaceNormal)
        {
            isGrounded = true;
            if (!collisions.Contains(collision.collider))
            {
                collisions.Add(collision.collider);
            }
        } else
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
        if(other.gameObject.CompareTag("Water") && playerState != PlayerState.Exhaust)
        {
            playerState = PlayerState.Dive;
            if(playerAgent)
            {
                playerAgent.AddReward(-0.1f);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && playerState != PlayerState.Exhaust)
        {
            playerState = PlayerState.Dive;
            if (playerAgent)
            {
                playerAgent.AddReward(-0.1f);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && playerState != PlayerState.Exhaust)
        {
            playerState = PlayerState.Fall;
            if (playerAgent)
            {
                playerAgent.AddReward(0.1f);
            }
        }
    }
    private void MoveUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool sprint = Input.GetKey(KeyCode.LeftShift) && playerState != PlayerState.Exhaust;
        bool CanChangeState()
        {
            switch(playerState)
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

        if (jumpCooldownOver && isGrounded && Input.GetKey(KeyCode.Space))
        {
            if(playerState == PlayerState.Exhaust)
            {
                jumpForce = 6f;
            }
            else
            {
                jumpForce = 12f;
            }
            heightBefore = this.transform.position.y;
            jumpTimeStamp = Time.time;
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerState = PlayerState.Jump;
            isJumping = true;
        }
        if (!wasGrounded && isGrounded)
        {
            float fallAmount = maximumHeight - this.transform.position.y;
            if(fallAmount > 6.0f || hp < 0.0f)
            {
                hp = -100;
                playerState = PlayerState.Exhaust;
                if(!isRecovering)
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
                heightBefore = this.transform.position.y;
                playerState = PlayerState.Fall;
            }
        }
    }

    private void CheckState()
    {
        switch(playerState)
        {
            case PlayerState.Idle:
                maximumHeight = this.transform.position.y;
                if(hp < 100.0f)
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
                //gameOverPanel.SetActive(true);
                this.transform.position = new Vector3(42.5f, 6.0f, 32.5f);
                this.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
                playerState = PlayerState.Fall;
                break;
        }

        //hpbar.value = (Mathf.Round(hp) > 0.0f) ? Mathf.Round(hp) : 0.0f;

        if(hp > 100.0f)
        {
            hp = 100.0f;
        }
        if(hp <= 0f && hp > -100f && isGrounded)
        {
            hp = -100f;
            playerState = PlayerState.Exhaust;
            if(!isRecovering)
            {
                Invoke("Recover", 10.0f);
                isRecovering = true;
            }
        }

        if(this.transform.position.y < -15.0f)
        {
            playerState = PlayerState.Death;
        }
    }
    private void Recover()
    {
        if(isRecovering)
        {
            hp = 0.1f;
            playerState = PlayerState.Idle;
            isRecovering = false;
        }
    }

    // 강화학습 함수
    public void Move(float v, float h, float s, float j)
    {
        float GetDiscreteValue(float n)
        {
            if(n > 0.1f)
            {
                n = 1.0f;
            }
            else if(n < -0.1f)
            {
                n = -1.0f;
            }
            else
            {
                n = 0.0f;
            }
            return n;
        }
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
        bool sprint = s > 0.25f && playerState != PlayerState.Exhaust;
        bool jump = j > 0.5f;

        v = GetDiscreteValue(v);
        h = GetDiscreteValue(h);

        if (sprint && !isSprint)
        {
            sprintStart = Time.time;
            isSprint = true;
        }

        if(!sprint && isSprint)
        {
            playerAgent.CheckSprint(sprintStart, Time.time);
            isSprint = false;
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
        Jump(jump);
    }

    private void Jump(bool jump)
    {
        bool jumpCooldownOver = (Time.time - jumpTimeStamp) >= minJumpInterval;

        if (jumpCooldownOver && isGrounded && jump)
        {
            if (playerState == PlayerState.Exhaust)
            {
                jumpForce = 6f;
            }
            else
            {
                jumpForce = 12f;
            }
            heightBefore = this.transform.position.y;
            jumpTimeStamp = Time.time;
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerState = PlayerState.Jump;
            isJumping = true;
        }
        if (!wasGrounded && isGrounded)
        {
            float fallAmount = maximumHeight - this.transform.position.y;
            playerAgent.CheckOnLanding(fallAmount, heightBefore);
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
                heightBefore = this.transform.position.y;
                playerState = PlayerState.Fall;
            }
        }
    }

    public void FullHP()
    {
        hp = 100.0f;
        if(playerState == PlayerState.Exhaust)
        {
            playerState = PlayerState.Idle;
            isRecovering = false;
        }
    }

}
