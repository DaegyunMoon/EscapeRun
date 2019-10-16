using Holoville.HOTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterControl : MonoBehaviour
{

    [Header("이동관련속성")] //유니티에 나타남
    [Tooltip("기본이동속도")] //MoveSpeed에 마우스를 올리면 tooltip이 나타남
    public float MoveSpeed = 3.0f;//이동속도
    public float RunSpeed = 4.5f;//달리기속도
    public float DirectionRotateSpeed = 100.0f; // 이동방향을 변경하기 위한 속도.
    public float BodyRotateSpeed = 3.0f; // 몸체 회전 속도
    [Range(0.01f, 5.0f)] // 유니티에서 스크롤로 0.01 부터 5.0까지 나타냄
    public float VelocityChangeSpeed = 0.1f;// 무브에서 달리는 속도로 변경되는 속도
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 MoveDirection = Vector3.zero;
    private CharacterController mycharacterController = null;
    private CollisionFlags collisionFlags = CollisionFlags.None;
    private float gravity = 9.8f;//중력값.
    private float verticalSpeed = 0.0f; // 수직속도.
    private bool CannotMove = false; // 이동 불가 플래그.

    [Header("애니메이션관련 속성")]
    public AnimationClip IdleAnimClip = null;
    public AnimationClip WalkAnimClip = null;
    public AnimationClip RunAnimClip = null;
    public AnimationClip Attack1AnimClip = null;
    public AnimationClip Attack2AnimClip = null;
    public AnimationClip Attack3AnimClip = null;
    public AnimationClip Attack4AnimClip = null;
    public AnimationClip SkillAnimClip = null;
    public AnimationClip DeathAnimClip = null;

    private Transform myTransform = null;
    private Animation myAnimation = null;

    //캐릭터 상태 목록.
    public enum FighterState { None, Idle ,Walk, Run, Attack, Skill, Death }
    [Header("캐릭터상태")]
    public FighterState myState = FighterState.None;
    public static int stateNum;

    public enum FighterAttackState { Attack1, Attack2, Attack3, Attack4 }
    public FighterAttackState AttackState;

    //다음 공격 활성화 여부를 확인하는 플래그.
    public bool NextAttack;
    public static bool SkillAvailable = true;
    public static bool SkillMade = false;

    [Header("전투 관련")]
    public double HP = 100;
    public GameObject SkillEffect = null;
    public GameObject SkillImage;
    private Tweener effectTweener = null;
    private SkinnedMeshRenderer skinMeshRenderer = null;
    public TrailRenderer AttackTrailRenderer = null;
    public CapsuleCollider AttackCapsuleCollider = null;

    // Use this for initialization
    void Start()
    {
        CancelInvoke();
        mycharacterController = GetComponent<CharacterController>();

        myTransform = GetComponent<Transform>();
        myAnimation = GetComponent <Animation>();
        myAnimation.playAutomatically = false;
        myAnimation.Stop();

        myState = FighterState.Idle;
        myAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;//대기 애니메이션은 반복
        myAnimation[WalkAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[RunAnimClip.name].wrapMode = WrapMode.Loop;
        myAnimation[Attack1AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[Attack2AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[Attack3AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[Attack4AnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[SkillAnimClip.name].wrapMode = WrapMode.Once;
        myAnimation[DeathAnimClip.name].wrapMode = WrapMode.Once;

        AddAnimationEvent(Attack1AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(Attack2AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(Attack3AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(Attack4AnimClip, "OnAttackAnimFinished");
        AddAnimationEvent(SkillAnimClip, "OnSkillAnimFinished");
        AddAnimationEvent(DeathAnimClip, "OnDeathAnimFinished");

        skinMeshRenderer = myTransform.Find("body").GetComponent<SkinnedMeshRenderer>();
        NextAttack = false;
        //AttackState = FighterAttackState.Attack1;

        //피로누적 적용
        InvokeRepeating("GetTiredness", 0.0f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {   
        if(GameControl.instance.MyGameState == GameControl.GameState.Playing)
        {
            //이동
            Move();
            //몸통의 방향을 이동 방향으로 돌려줍니다.
            BodyDirectionChange();
            //상태에 맞추어 애니메이션을 재생시켜줍니다.
            AnimationControl();
            //조건에 맞추어 캐릭터 상태를 변경시켜줍니다.
            CheckState();
            //공격을 제어합니다.
            //InputControl();
            //중력적용
            ApplyGravity();
            //공격 관련 컴포넌트 제어.
            AttackComponentControl();
        }
    }
    /// <summary>
    /// 이동 관련 함수
    /// </summary>
    void Move()
    {  
        if(CannotMove == true)
        {
            return;
        }
        //Maincamera 게임오브젝트의 트랜스폼 컴포넌트.
        Transform CameraTransform = Camera.main.transform;
        //카메라가 바라보는 방향의 월드상에서 어떤 방향인지 얻어옴.
        Vector3 forward = CameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        float vertical = Input.GetAxis("Vertical");//키보드의 위,아래,w,s -1~1
        float horizontal = Input.GetAxis("Horizontal");//키보드의 좌,우,a,d -1 ~1
        Vector3 targetDirection = horizontal * right + vertical * forward;// 우리가 이동하고자 하는 방향.
        // 현재 이동하는 방향에서 원하는 방향으로 조금씩 회전을하게 된다.
        MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection, DirectionRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        MoveDirection = MoveDirection.normalized;//방향이기때문에 크기는 없애고 방향만 가져옴.
        float speed = MoveSpeed; //이동속도
        if(myState == FighterState.Run)
        {
            speed = RunSpeed;
        }
        //중력 벡터.
        Vector3 gravityVector = new Vector3(0.0f, verticalSpeed, 0.0f);
        Vector3 moveAmount = (MoveDirection * speed * Time.deltaTime) + gravityVector; //이번 프레임에 움직일 양.
        collisionFlags = mycharacterController.Move(moveAmount); //실제 이동.
    }

    /*
    private void OnGUI()
    {
        //충돌 정보.
        GUILayout.Label("충돌:" + collisionFlags.ToString());
        GUILayout.Label("현재 속도:" + GetvelocitySpeed().ToString());
        //캐릭터컨트롤러 컴포넌트를 찾았고, 현재 내 캐릭터의 이동속도가 0이 아니라면.
        if(mycharacterController != null && mycharacterController.velocity != Vector3.zero)
        {   //현재 내 캐릭터가 이동하는 방향(+크기)
            GUILayout.Label("current Velocity Vector : " + mycharacterController.velocity.ToString());
            //현재 내 속도.
            GUILayout.Label("current Velocity Magnitude : " + mycharacterController.velocity.magnitude.ToString());
        }
    }
    */

    /// <summary>
    /// 현재 내 캐릭터의 이동속도를 얻어옵니다.
    /// </summary>
    float GetvelocitySpeed()
    {   //멈춰있다면.
        if(mycharacterController.velocity == Vector3.zero)
        {   //현재 속도를 0으로
            currentVelocity = Vector3.zero;
        }else
        {
            Vector3 goalVelocity = mycharacterController.velocity;
            goalVelocity.y = 0.0f;
            currentVelocity = Vector3.Lerp(currentVelocity, goalVelocity, VelocityChangeSpeed * Time.fixedDeltaTime);
        }
        //currentVelocity의 크기를 리턴합니다.
        return currentVelocity.magnitude * 10;
    }
    /// <summary>
    /// 몸통의 방향을 이동방향으로 돌려줍니다.
    /// </summary>
    void BodyDirectionChange()
    {
        if(GetvelocitySpeed() > 0.0f)
        {
            Vector3 newForward = mycharacterController.velocity;
            newForward.y = 0.0f;
            transform.forward = Vector3.Lerp(transform.forward, newForward, BodyRotateSpeed * Time.deltaTime);
        }
    }
    /// <summary>
    /// 애니메이션을 재생시키는 함수.
    /// </summary>
    /// <param name="clip"></param>
    void AnimationPlay(AnimationClip clip)
    {
        myAnimation.clip = clip;
        myAnimation.CrossFade(clip.name);
    }

    void AnimationControl()
    {
        switch(myState)
        {
            case FighterState.Idle:
                AnimationPlay(IdleAnimClip);
                break;
            case FighterState.Walk:
                AnimationPlay(WalkAnimClip);
                break;
            case FighterState.Run:
                AnimationPlay(RunAnimClip);
                break;
            case FighterState.Attack:
                //공격상태에 맞춘 애니메이션을 재생시켜줍니다.
                AttackAnimationContol();
                break;
            case FighterState.Skill:
                AnimationPlay(SkillAnimClip);
                break;
            case FighterState.Death:
                AnimationPlay(DeathAnimClip);
                break;
        }
    }
    /// <summary>
    /// 상태를 변경해주는 함수.
    /// </summary>
    void CheckState()
    {
        stateNum = (int)myState;
        float currentSpeed = GetvelocitySpeed();

        switch (myState)
        {
            case FighterState.Idle:
                SoundManager.instance.StopSound(0);
                SoundManager.instance.StopSound(1);
                if (currentSpeed > 0.0f)
                {
                    myState = FighterState.Walk;
                }   
                break;
            case FighterState.Walk:
                SoundManager.instance.PlaySound(0, true); // Walking 재생
                SoundManager.instance.StopSound(1);
                if (currentSpeed > 0.3f)
                {
                    myState = FighterState.Run;
                }
                else if (currentSpeed < 0.01f)
                {
                    myState = FighterState.Idle;
                }
                break;
            case FighterState.Run:
                SoundManager.instance.StopSound(0);
                SoundManager.instance.PlaySound(1, true); // Running 재생
                if (currentSpeed < 0.3f)
                {
                    myState = FighterState.Walk;
                }
                if(currentSpeed < 0.01f)
                {
                    myState = FighterState.Idle;
                }
                break;
            case FighterState.Attack:
            case FighterState.Skill:
            case FighterState.Death:
                SoundManager.instance.StopSound(0);
                SoundManager.instance.StopSound(1);
                CannotMove = true;
                break;
        }
        if(GameControl.instance.MyGameState != GameControl.GameState.Playing)
        {
            SoundManager.instance.StopSound(0);
            SoundManager.instance.StopSound(1);
        }
    }
    /// <summary>
    /// 마우스 왼쪽 버튼으로 공격을 합니다.
    /// </summary>
    void InputControl()
    {   // 0 : 마우스 왼쪽버튼
        if(Input.GetMouseButtonDown(0)== true)
        {   
            //내가 공격중이 아니라면 공격을 시작하게 되고.
            if (myState != FighterState.Attack && myState != FighterState.Death)
            {
                SoundManager.instance.PlaySound(2, false); // Swing1 재생
                myState = FighterState.Attack;
                AttackState = FighterAttackState.Attack1;
            }
            else
            {   // 공격 중이라면 애니메이션이 일정 이상 재생이 됐다면 다음 공격을 활성화.
                switch (AttackState)
                {
                    case FighterAttackState.Attack1:
                        if (myAnimation[Attack1AnimClip.name].normalizedTime > 0.1f)
                        {
                            SoundManager.instance.PlaySound(3, false); // Swing2 재생
                            NextAttack = true;
                        }
                        break;
                    case FighterAttackState.Attack2:
                        if (myAnimation[Attack2AnimClip.name].normalizedTime > 0.1f)
                        {
                            SoundManager.instance.PlaySound(4, false); // Swing3 재생
                            NextAttack = true;
                        }
                        break;
                    case FighterAttackState.Attack3:
                        if (myAnimation[Attack3AnimClip.name].normalizedTime > 0.1f)
                        {
                            SoundManager.instance.PlaySound(5, false); // Swing4 재생
                            NextAttack = true;
                        }
                        break;
                    case FighterAttackState.Attack4:
                        if (myAnimation[Attack4AnimClip.name].normalizedTime > 0.1f)
                        {
                            SoundManager.instance.PlaySound(2, false); // Swing1 재생
                            NextAttack = true;
                        }
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (myState != FighterState.Death && SkillAvailable == true && TimeManager.instance.MyPlayTime > 0.5)
            {
                if (myState == FighterState.Attack)
                {
                    AttackState = FighterAttackState.Attack1;
                    NextAttack = false;
                }
                SkillMade = true;
                SoundManager.instance.PlaySound(6, false); // ActiveSkill 재생
                myState = FighterState.Skill;
            }
        }
    }
    /// <summary>
    /// 공격 애니메이션 재생이 끝나면 호출되는 애니메이션 이벤트 함수.
    /// </summary>
    void OnAttackAnimFinished()
    {
        if(NextAttack == true)
        {
            NextAttack = false;
            switch (AttackState)
            {
                case FighterAttackState.Attack1:
                    AttackState = FighterAttackState.Attack2;
                    break;
                case FighterAttackState.Attack2:
                    AttackState = FighterAttackState.Attack3;
                    break;
                case FighterAttackState.Attack3:
                    AttackState = FighterAttackState.Attack4;
                    break;
                case FighterAttackState.Attack4:
                    AttackState = FighterAttackState.Attack1;
                    break;
            }
        }
        else
        {
            CannotMove = false;
            myState = FighterState.Idle;
            AttackState = FighterAttackState.Attack1;
        }
    }
    //스킬 애니메이션 재생이 끝났으면.
    void OnSkillAnimFinished()
    {
        myState = FighterState.Idle;
        Vector3 position = transform.position;
        position += transform.forward * 2.0f;
        Instantiate(SkillEffect, position, Quaternion.identity);
        CannotMove = false;
        SkillMade = false;
        SkillAvailable = false;
        SkillImage.SetActive(false);
        Invoke("RefreshSkill", 10f);
    }
    void RefreshSkill()
    {
        SkillImage.SetActive(true);
        SkillAvailable = true;
    }
    void OnDeathAnimFinished()
    {
        SoundManager.instance.PlaySound(15, false); // OverSound 재생
        GameControl.instance.SetGameState(3);
    }

    /// <summary>
    /// 애니메이션 클립 재생이 끝날때쯤 애니메이션 이벤트 함수를 호출 시켜주도로 추가합니다.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="FuncName"></param>
    void AddAnimationEvent(AnimationClip clip, string FuncName)
    {
        AnimationEvent newEvent = new AnimationEvent();
        newEvent.functionName = FuncName;
        newEvent.time = clip.length - 0.1f;
        clip.AddEvent(newEvent);
    }
    /// <summary>
    /// 공격 애니메이션을 재생시켜줍니다.
    /// </summary>
    void AttackAnimationContol()
    {
        switch(AttackState)
        {
            case FighterAttackState.Attack1:
                AnimationPlay(Attack1AnimClip);
                break;
            case FighterAttackState.Attack2:
                AnimationPlay(Attack2AnimClip);
                break;
            case FighterAttackState.Attack3:
                AnimationPlay(Attack3AnimClip);
                break;
            case FighterAttackState.Attack4:
                AnimationPlay(Attack4AnimClip);
                break;
        }
    }
    /// <summary>
    /// 중력적용.
    /// </summary>
    void ApplyGravity()
    {   
        //CollidedBelow가 세팅되었다면. == 바닥에 붙었다면.
        if((collisionFlags & CollisionFlags.CollidedBelow) != 0)
        {
            verticalSpeed = 0.0f;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    void AttackComponentControl()
    {
        switch (myState)
        {
            //공격중일때만 트레일 컴포넌트와 충돌 컴포넌트 활성화.
            case FighterState.Attack:
            case FighterState.Skill:
                AttackTrailRenderer.enabled = true;
                AttackCapsuleCollider.enabled = true;
                break;
            default:
                AttackTrailRenderer.enabled = false;
                AttackCapsuleCollider.enabled = false;
                break;
        }
    }

    void GetTiredness()
    {
        if(GameControl.instance.MyGameState == GameControl.GameState.Playing && myState != FighterState.Death)
        {
            HP -= Score.tiredness;
        }
        CheckHealthPoint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (myState != FighterState.Death && other.gameObject.CompareTag("EnemyAttack") == true)
        {
            HP -= Score.enemyDamage;
            if (HP > 0)
            {
                DamageTweenEffect(); //피격 트위닝 이팩트 재생
                SoundManager.instance.PlaySound(8, false); // BeHit 재생

            }
        }
        if (other.gameObject.CompareTag("Item") == true)
        {
            HP += 20;
            Score.instance.MyScore += Score.itemScore;
            SoundManager.instance.PlaySound(7, false); // GetItem 재생
        }
        CheckHealthPoint();
    }
    void DamageTweenEffect()
    {
        if (effectTweener != null && effectTweener.isComplete == false)
        {
            return;
        }
        Color colorTo = Color.red;
        effectTweener = HOTween.To(skinMeshRenderer.material, 0.2f, new TweenParms()
            .Prop("color", colorTo)
            .Loops(1, LoopType.Yoyo)
            .OnStepComplete(OnDamageTweenFinished));
    }

    void OnDamageTweenFinished()
    {
        skinMeshRenderer.material.color = Color.white;
    }

    void CheckHealthPoint()
    {
        
        if (HP < 0)
        {
            HP = 0;
            SoundManager.instance.PlaySound(9, false); // DeathVoice 재생
            myState = FighterState.Death;
        }
        else if(HP > 0 && HP <= 35)
        {
            SoundManager.instance.PlaySound(18, false); // Warning 재생
        }
        else if (HP > 100)
        {
            HP = 100;
        }
    }
}
