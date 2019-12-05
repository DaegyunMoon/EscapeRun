using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("카메라기본속성")]
    private Transform myTransform = null;
    public GameObject Target = null;
    private Transform targetTransform = null;
    public bool isForward = true;
    
    public enum CameraViewPointState { FIRST, SECOND, THIRD }
    public CameraViewPointState CameraState = CameraViewPointState.THIRD;
    public CameraViewPointState LastState;
    
    [Header("3인칭 카메라")]
    public float Distance = 3.0f;
    public float Height = 1.0f;
    public float HeightDamping = 3.0f;
    public float RotateionDamping = 10.0f;
    
    [Header("2인칭 카메라")]
    public float RotateSpeed = 20.0f;
    
    [Header("1인칭 카메라")]
    public float SensitivityX = 5.0f;
    public float SensitivityY = 5.0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    public Transform FirstCameraSocket = null;
    
    // Use this for initialization
    void Start ()
    {
        //LastState = CameraState;
        myTransform = GetComponent<Transform>();
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

        float currentRotationAngle = myTransform.eulerAngles.y; //현재 카메라의 y축 각도 값.
        float currentHeight = myTransform.position.y; //현재 카메라의 높이값.
        //현재 각도에서 원하는 각도로 댐핑값을 얻게 됨.
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, RotateionDamping * Time.deltaTime);

        //현재 높이에서 원하는 높이로 댐핑값을 얻습니다.
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, HeightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);

        myTransform.position = targetTransform.position;
        myTransform.position -= currentRotation * Vector3.forward * Distance;
        myTransform.position = new Vector3(myTransform.position.x, currentHeight, myTransform.position.z);

        myTransform.LookAt(targetTransform);
    }
    
    /// <summary>
    /// 모델뷰
    /// </summary>
    void SecondView()
    {
        if(LastState == CameraViewPointState.THIRD)
        {
            myTransform.RotateAround(targetTransform.position, Vector3.up, RotateSpeed * Time.deltaTime);
            myTransform.LookAt(targetTransform);
        }
        if (LastState == CameraViewPointState.FIRST)
        {
            myTransform.RotateAround(targetTransform.position - new Vector3(-3, -3, -3), Vector3.up, RotateSpeed * Time.deltaTime);
            myTransform.LookAt(targetTransform);
        }
    }
    /// <summary>
    /// 1인칭 뷰.
    /// </summary>
    void FirstView()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX = myTransform.localEulerAngles.y + mouseX * SensitivityX;
        //마이너스 각도를 조절하기 위한 연산.
        rotationX = (rotationX > 180.0f) ? rotationX - 360.0f : rotationX;

        rotationY = rotationY + mouseY * SensitivityY;
        rotationY = (rotationY > 180.0f) ? rotationY - 360.0f : rotationY;
        
        myTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0f);
        myTransform.position = FirstCameraSocket.position;
    }
    /*
    private void Update()
    {
        if(FighterControl.stateNum == 6)
        {
            CameraState = CameraViewPointState.THIRD;
        }
        //게임오버 시 2인칭으로 변환
        if(GameControl.instance.MyGameState == GameControl.GameState.Over)
        {
            CameraState = CameraViewPointState.SECOND;
        }

        //일시정지 기능
        if (Input.GetKeyDown(KeyCode.K) && GameControl.instance.MyGameState != GameControl.GameState.Over)
        {
            if (CameraState == CameraViewPointState.SECOND)
            {
                CameraState = LastState;
            }
            else
            {
                LastState = CameraState;
                CameraState = CameraViewPointState.SECOND;
            }
        }
        //시점 변경
        if (GameControl.instance.MyGameState == GameControl.GameState.Playing && Input.GetKeyDown(KeyCode.L))
        {
            if(CameraState == CameraViewPointState.THIRD)
            {
                CameraState = CameraViewPointState.FIRST;
            }
            else if(CameraState == CameraViewPointState.FIRST)
            {
                CameraState = CameraViewPointState.THIRD;
            }
        }
    }
    */
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
            case CameraViewPointState.THIRD:
                ThirdView();
                break;
            case CameraViewPointState.SECOND:
                SecondView();
                break;
            case CameraViewPointState.FIRST:
                FirstView();
                break;
        }
	}
    
}
