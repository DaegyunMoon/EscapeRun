using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject target = null;
    private Transform targetTransform = null;
    public bool isForward = true;
    
    public enum CameraViewPointState { SKYVIEW, ROTATEVIEW, DEFAULTVIEW }
    public CameraViewPointState cameraState = CameraViewPointState.DEFAULTVIEW;
    public CameraViewPointState lastState;
    
    [Header("DefaultView")]
    public float distance = 3.0f;
    public float height = 1.0f;
    public float heightDamping = 3.0f;
    public float rotateionDamping = 10.0f;
    
    [Header("RotateView")]
    public float rotateSpeed = 20.0f;
    
    // Use this for initialization
    void Start ()
    {
        if(target != null)
        {
            targetTransform = target.transform;
        }
	}
    private void Update()
    {
        if (target == null)
        {
            return;
        }
        if (targetTransform == null)
        {
            targetTransform = target.transform;
        }
    }
    void DefaultView()
    {
        float wantedRotationAngle = targetTransform.eulerAngles.y;
        float wantedHeight = targetTransform.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotateionDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);

        transform.position = targetTransform.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        transform.LookAt(targetTransform);
    }
    void RotateView()
    {
        if(lastState == CameraViewPointState.DEFAULTVIEW)
        {
            transform.RotateAround(targetTransform.position, Vector3.up, rotateSpeed * Time.deltaTime);
            transform.LookAt(targetTransform);
        }
        if (lastState == CameraViewPointState.SKYVIEW)
        {
            transform.RotateAround(targetTransform.position - new Vector3(-3, -3, -3), Vector3.up, rotateSpeed * Time.deltaTime);
            transform.LookAt(targetTransform);
        }
    }
    void SkyView()
    {
        Quaternion currentRotation = Quaternion.Euler(90.0f, targetTransform.eulerAngles.y, 0f);
        transform.rotation = currentRotation;
        transform.position = targetTransform.position + new Vector3(0.0f, 30.0f, 0.0f);
    }
    private void LateUpdate ()
    {
		if(target == null)
        {
            return;
        }
        if(targetTransform == null)
        {
            targetTransform = target.transform;
        }
        switch (cameraState)
        {
            case CameraViewPointState.DEFAULTVIEW:
                DefaultView();
                break;
            case CameraViewPointState.ROTATEVIEW:
                RotateView();
                break;
            case CameraViewPointState.SKYVIEW:
                SkyView();
                break;
        }
	}
    
    public void SetDistance(float distance)
    {
        this.distance = distance;
    }
}
