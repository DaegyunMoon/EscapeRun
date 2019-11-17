using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class PlayerAgent : Agent
{
    public PlayerControl playerControl;
    public TargetControl targetControl;
    public Rigidbody unitBody;
    public Transform terrainTransform;
    public GameObject target;
    public TargetSpawner targetSpawner;

    private int thresholdTime;
    private float beforeDistanceToTarget;

    // Start is called before the first frame update
    void Start()
    {
        targetSpawner = FindObjectOfType<TargetSpawner>();
    }

    public override void AgentReset()
    {
        this.transform.position = new Vector3(42.5f, 6.0f, 32.5f);
        unitBody.velocity = Vector3.zero;
    }
    public override void CollectObservations()
    {
        float temp = 500.0f;
        Vector3 closestDistance = Vector3.zero;
        Vector3 relativeDistance;
        
        foreach (GameObject target in targetSpawner.targetList)
        {
            Vector3 distanceToTarget = target.transform.position - this.transform.position;
            if (temp > distanceToTarget.magnitude)
            {
                temp = distanceToTarget.magnitude;
                closestDistance = distanceToTarget;
                this.target = target;
            }
        }

        TargetControl[] targetControls = FindObjectsOfType<TargetControl>();
        foreach (TargetControl targetControl in targetControls)
        {
            if (targetControl.gameObject == target)
            {
                this.targetControl = targetControl;
            }
        }

        AddVectorObs(Mathf.Clamp(closestDistance.x / 5.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(closestDistance.z / 5.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(closestDistance.y / 5.0f, -1.0f, 1.0f));

        relativeDistance = terrainTransform.position - this.transform.position;

        AddVectorObs(Mathf.Clamp(relativeDistance.x / 5.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(relativeDistance.z / 5.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(relativeDistance.y / 5.0f, -1.0f, 1.0f));

        AddVectorObs(Mathf.Clamp(playerControl.moveAmount.x / 10.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(playerControl.moveAmount.z / 10.0f, -1.0f, 1.0f));
        AddVectorObs(Mathf.Clamp(unitBody.velocity.y / 10.0f, -1.0f, 1.0f));
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.001f);
        float v = 1.0f;
        float h = vectorAction[1];
        float s = vectorAction[2];
        float j = vectorAction[3];

        playerControl.Move(v, h, s, j);

        if (playerControl.GetHP() <= 0.0f && playerControl.GetHP() > -99.0f)
        {
            Debug.Log("보상 : 탈진");
            AddReward(-1.0f);
        }

        if (playerControl.playerState == PlayerControl.PlayerState.Death)
        {
            Debug.Log("보상 : 사망");
            AddReward(-1.0f);
            Done();
        }
        if (playerControl.playerState == PlayerControl.PlayerState.Dive)
        {
            if (targetControl)
            {
                if (targetControl.playerState == TargetControl.PlayerState.Dive)
                {
                    Debug.Log("보상 : 타겟을 따라 물에 빠짐");
                    AddReward(0.01f);
                }
                else
                {
                    Debug.Log("보상 : 물에 빠짐");
                    AddReward(-0.01f);
                }
            }
        }
        if ((int)Timer.time >= thresholdTime)
        {
            float nowDistanceToTarget = (target.transform.position - this.transform.position).magnitude;
            thresholdTime += 2;
            if (beforeDistanceToTarget > nowDistanceToTarget)
            {
                Debug.Log("보상 : 타겟과 가까워짐");
                AddReward(0.2f);
            }
            else
            {
                Debug.Log("보상 : 타겟과 멀어짐");
                AddReward(-0.2f);
                if (s > 0.5f && playerControl.playerState != PlayerControl.PlayerState.Exhaust)
                {
                    AddReward(-0.5f);
                }
            }
            beforeDistanceToTarget = (target.transform.position - this.transform.position).magnitude;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("보상 : 타겟을 잡음");
            AddReward(2.0f);
            targetSpawner.RemoveTarget(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log("보상 : 아이템 획득");
            AddReward(0.25f);
            if (playerControl.GetHP() < 50.0f)
            {
                Debug.Log("보상 : 피로도 회복");
                AddReward(0.75f);
            }
        }
    }

    public void CheckOnLanding(float fallAmount, float heightBefore)
    {
        float heightAfter = this.transform.position.y;
        float heightDiffBefore = Mathf.Abs(heightBefore - target.transform.position.y);
        float heightDiffAfter = Mathf.Abs(heightAfter - target.transform.position.y);
        Debug.Log(Math.Round(fallAmount, 2));
        Debug.Log(Math.Round(heightBefore, 1));
        Debug.Log(Math.Round(heightAfter, 1));
        if (Math.Round(fallAmount, 2) == 1.77 && Math.Round(heightBefore, 1) == Math.Round(heightAfter, 1))
        {
            Debug.Log("보상 : 의미없는 점프");
            AddReward(-1.0f);
        }
        else
        {
            Debug.Log("보상 : 점프 성공");
            AddReward(0.2f);
        }
        if (Math.Round(heightDiffBefore, 2) > Math.Round(heightDiffAfter, 2))
        {
            Debug.Log("보상 : 타겟과 높이가 가까워짐");
            AddReward(0.2f);
        }
    }

    public void CheckSprint(float sprintStart, float sprintEnd)
    {
        float sprintTime = sprintEnd - sprintStart;
        if (sprintTime > 2.0f)
        {
            if (playerControl.GetHP() > 5.0f)
            {
                Debug.Log("보상 : Sprint");
                AddReward(0.001f);
            }
            else
            {
                Debug.Log("보상 : 탈진 위험");
                AddReward(-0.01f);
            }
        }
        else
        {
            Debug.Log("보상 : 불필요한 Sprint");
            AddReward(-1.0f);
        }
    }
}
