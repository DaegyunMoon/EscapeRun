using UnityEngine;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
    private enum PlayerState { Idle, Run, Sprint, Jump, Fall, Exhaustion, Dive, Death }
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float turnSpeed = 200;
    [SerializeField] private float jumpForce = 6;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;

    PlayerState playerState;
    private float currentV = 0;
    private float currentH = 0;

    private readonly float interpolation = 10;
    private readonly float runScale = 1.5f;
    private readonly float backwardRunScale = 0.75f;
    private readonly float backwardWalkScale = 0.5f;

    private bool isGrounded;
    private bool wasGrounded;

    private float jumpTimeStamp = 0;
    private float minJumpInterval = 0.25f;
    private List<Collider> collisions = new List<Collider>();

    void Update()
    {
        animator.SetBool("Grounded", isGrounded);
        TankUpdate();
        wasGrounded = isGrounded;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!collisions.Contains(collision.collider)) {
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
    private void TankUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool sprint = Input.GetKey(KeyCode.LeftShift);

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
        else if (sprint)
        {
            v *= runScale;
        }

        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
        currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

        transform.position += transform.forward * currentV * moveSpeed * Time.deltaTime;
        transform.Rotate(0, currentH * turnSpeed * Time.deltaTime, 0);

        animator.SetFloat("MoveSpeed", currentV);

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - jumpTimeStamp) >= minJumpInterval;

        if (jumpCooldownOver && isGrounded && Input.GetKey(KeyCode.Space))
        {
            jumpTimeStamp = Time.time;
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");
        }

        if (!isGrounded && wasGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }

    private void CheckState()
    {

    }
}
