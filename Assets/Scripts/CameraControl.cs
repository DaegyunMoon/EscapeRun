using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("카메라기본속성")]
    public GameObject Target = null;
    private Transform targetTransform = null;
    public bool isForward = true;
    
    public enum CameraViewPointState { SKYVIEW, SECOND, MAINVIEW }
    public CameraViewPointState CameraState = CameraViewPointState.MAINVIEW;
    public CameraViewPointState LastState;
    
    [Header("3인칭 카메라")]
    public float Distance = 3.0f;
    public float Height = 1.0f;
    public float HeightDamping = 3.0f;
    public float RotateionDamping = 10.0f;
    
    [Header("2인칭 카메라")]
    public float RotateSpeed = 20.0f;
    
    // Use this for initialization
    void Start ()
    {
        if(Target != null)
        {
            targetTransform = Target.transform;
        }
	}
    private void Update()
    {
        if (Target == null)
        {
            return;
        }
        if (targetTransform == null)
        {
            targetTransform = Target.transform;
        }
    }
    void ThirdView()
    {
        float wantedRotationAngle = targetTransform.eulerAngles.y; //현재 타겟의 y축 각도 값.
        float wantedHeight = targetTransform.position.y + Height; //현재 타겟의 높이 + 우리가 추가로 높이고 싶은 높이.

        float currentRotationAngle = transform.eulerAngles.y; //현재 카메라의 y축 각도 값.
        float currentHeight = transform.position.y; //현재 카메라의 높이값.
        //현재 각도에서 원하는 각도로 댐핑값을 얻게 됨.
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, RotateionDamping * Time.deltaTime);

        //현재 높이에서 원하는 높이로 댐핑값을 얻습니다.
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, HeightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);

        transform.position = targetTransform.position;
        transform.position -= currentRotation * Vector3.forward * Distance;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        transform.LookAt(targetTransform);
    }
    
    /// <summary>
    /// 모델뷰
    /// </summary>
    void SecondView()
    {
        if(LastState == CameraViewPointState.MAINVIEW)
        {
            transform.RotateAround(targetTransform.position, Vector3.up, RotateSpeed * Time.deltaTime);
            transform.LookAt(targetTransform);
        }
        if (LastState == CameraViewPointState.SKYVIEW)
        {
            transform.RotateAround(targetTransform.position - new Vector3(-3, -3, -3), Vector3.up, RotateSpeed * Time.deltaTime);
            transform.LookAt(targetTransform);
        }
    }
    /// <summary>
    /// 1인칭 뷰.
    /// </summary>
    void SkyView()
    {
        transform.position = targetTransform.position + new Vector3(0.0f, 30.0f, 0.0f);
    }
    /// <summary>
    /// update함수 후에 호출되는 업데이트.
    /// </summary>
    private void LateUpdate ()
    {
		if(Target == null)
        {
            return;
        }
        if(targetTransform == null)
        {
            targetTransform = Target.transform;
        }
        switch (CameraState)
        {
            case CameraViewPointState.MAINVIEW:
                ThirdView();
                break;
            case CameraViewPointState.SECOND:
                SecondView();
                break;
            case CameraViewPointState.SKYVIEW:
                SkyView();
                break;
        }
	}
    
}
