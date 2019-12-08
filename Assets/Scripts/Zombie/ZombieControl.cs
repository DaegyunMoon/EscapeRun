using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieControl : MonoBehaviour
{
    public enum ZombieState { Idle, Run, Jump, Fall, Exhaust, Dive, Death }
    [SerializeField] private float hp = 100;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float turnSpeed = 200;
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;

    public Transform terrainTransform;
    public ZombieAgent playerAgent;
    public ZombieState zombieState;
    private float currentV = 0;
    private float currentH = 0;

    private readonly float interpolation = 10;

    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;
    private bool isRecovering;

    private float jumpTimeStamp = 0.0f;
    private float minJumpInterval = 0.25f;
    private float heightBefore = 0.0f;
    private float maximumHeight = 0.0f;
    public Vector3 moveAmount = Vector3.zero;
    private List<Collider> collisions = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        heightBefore = this.transform.position.y;
        wasGrounded = isGrounded;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", isGrounded);
        CheckState();
    }
    public float GetHP()
    {
        return hp;
    }
    public bool GetIsGrounded()
    {
        return isGrounded;
    }
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
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
        if (other.gameObject.CompareTag("Water") && zombieState != ZombieState.Exhaust)
        {
            zombieState = ZombieState.Dive;
            if (playerAgent)
            {
                playerAgent.AddReward(-0.1f);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && zombieState != ZombieState.Exhaust)
        {
            zombieState = ZombieState.Dive;
            if (playerAgent)
            {
                playerAgent.AddReward(-0.1f);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water") && zombieState != ZombieState.Exhaust)
        {
            zombieState = ZombieState.Fall;
            if (playerAgent)
            {
                playerAgent.AddReward(0.1f);
            }
        }
    }
    public void Move(float v, float h, float j)
    {
        float GetDiscreteValue(float n)
        {
            if (n > 0.1f)
            {
                n = 1.0f;
            }
            else if (n < -0.1f)
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
            switch (zombieState)
            {
                case ZombieState.Jump:
                case ZombieState.Dive:
                case ZombieState.Exhaust:
                    return false;
                default:
                    return true;
            }
        }
        bool jump = j > 0.5f;

        v = GetDiscreteValue(v);
        h = GetDiscreteValue(h);

        if (zombieState == ZombieState.Dive || zombieState == ZombieState.Exhaust)
        {
            v *= 0.5f;
        }
        if (isGrounded && CanChangeState())
        {
            if (Mathf.Abs(v) < 0.1)
            {
                zombieState = ZombieState.Idle;
            }
            else
            {
                zombieState = ZombieState.Run;
            }
        }

        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
        currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

        transform.position += transform.forward * currentV * moveSpeed * Time.deltaTime;
        moveAmount = transform.forward * currentV * moveSpeed;
        transform.Rotate(0, currentH * turnSpeed * Time.deltaTime, 0);

        animator.SetFloat("MoveSpeed", currentV);
        Jump(jump);
        wasGrounded = isGrounded;
    }

    private void Jump(bool jump)
    {
        bool jumpCooldownOver = (Time.time - jumpTimeStamp) >= minJumpInterval;

        if (jumpCooldownOver && isGrounded && jump)
        {
            if (zombieState == ZombieState.Exhaust)
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
            zombieState = ZombieState.Jump;
            isJumping = true;
        }
        if (!wasGrounded && isGrounded)
        {
            float fallAmount = maximumHeight - this.transform.position.y;
            playerAgent.CheckOnLanding(fallAmount, heightBefore);
            if (fallAmount > 6.0f || hp < 0.0f)
            {
                hp = -100;
                zombieState = ZombieState.Exhaust;
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
                zombieState = ZombieState.Jump;
            }
            else
            {
                heightBefore = this.transform.position.y;
                zombieState = ZombieState.Fall;
            }
        }
    }

    private void CheckState()
    {
        switch (zombieState)
        {
            case ZombieState.Idle:
                maximumHeight = this.transform.position.y;

                if (hp < 100.0f)
                {
                    hp += 0.5f;
                }
                break;
            case ZombieState.Run:
                maximumHeight = this.transform.position.y;
                isJumping = false;
                break;
            case ZombieState.Jump:
                isJumping = true;
                if (maximumHeight <= this.transform.position.y)
                {
                    maximumHeight = this.transform.position.y;
                }
                else
                {
                    zombieState = ZombieState.Fall;
                    isJumping = false;
                }
                break;
            case ZombieState.Fall:
                break;
            case ZombieState.Exhaust:
                maximumHeight = this.transform.position.y;
                break;
            case ZombieState.Dive:
                maximumHeight = this.transform.position.y;
                break;
        }

        if (hp > 100.0f)
        {
            hp = 100.0f;
        }
        if (hp <= 0f && hp > -100f && isGrounded)
        {
            hp = -100f;
            zombieState = ZombieState.Exhaust;
            if (!isRecovering)
            {
                Invoke("Recover", 10.0f);
                isRecovering = true;
            }
        }

        if (this.transform.position.y < -25.0f)
        {
            zombieState = ZombieState.Death;
        }
    }
    private void Recover()
    {
        if (isRecovering)
        {
            hp = 0.1f;
            zombieState = ZombieState.Idle;
            isRecovering = false;
        }
    }

    public void FullHP()
    {
        hp = 100.0f;
        if (zombieState == ZombieState.Exhaust)
        {
            zombieState = ZombieState.Idle;
            isRecovering = false;
        }
    }
}
