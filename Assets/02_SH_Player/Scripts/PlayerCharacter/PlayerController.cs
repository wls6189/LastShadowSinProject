using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState // 플레이어의 현재 행동(혹은 상태)
{
    IdleAndMove,
    Dash,
    Airborne,
    ShortGroggy,
    LongGroggy,
    Guard,
    GuardMove,
    Penetrate,
    BasicHorizonSlash1,
    BasicHorizonSlash2,
    BasicVerticalSlash,
    Thrust,
    CounterPosture,
    RetreatSlash,
    PowerfulThrust,
}

public class PlayerController : MonoBehaviour
{
    // 인스펙터에서 할당이 필요한 변수
    public GroundCheck GroundCheck; // 플레이어의 발이 땅에 붙어있는지 체크하는 클래스
    public Collider WeaponCollider; // 무기의 콜라이더
    public NearbyMonsterCheck NearbyMonsterCheck; // 근처의 몬스터를 감지하는 클래스
    public Transform CameraFocusPosition; // 카메라의 포커스 위치

    // 움직임 관련 변수
    [HideInInspector] public float MoveActionValue; // 움직임 방향키를 누를 때 반환되는 값을 캐싱
    [HideInInspector] public float MoveSpeed; // 움직이는 속도

    // 점프 관련 변수
    [HideInInspector] public float VerticalSpeed; // 플레이어의 수직 속도(중력에서 관리)
    float gravity = 17; // 중력
    float terminalSpeed = 20; // 수직 속도의 한계
    [HideInInspector] public bool IsGrounded;

    // 대쉬 관련 변수
    [HideInInspector] public float DashSpeed; // 대쉬 시 속도

    // 공격 관련 변수
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsAttackMoving;
    [HideInInspector] public bool CanBasicHorizonSlashCombo;
    [HideInInspector] public bool IsAttackColliderEnabled;

    // 막기 관련 변수
    public bool IsParring;
    public bool IsGuarding;
    public bool IsParrySucceed;

    // 조작 관련 변수
    [HideInInspector] public CharacterController CharacterController;
    InputActionAsset inputActionAsset;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAndPenetrateAction;
    InputAction attack1Action;
    InputAction attack2Action;
    InputAction attack3Action;
    InputAction guardAction;
    InputAction lockOnAction;
    InputAction interactAction;

    // 부가적인 변수
    [HideInInspector] public Animator Animator;
    [HideInInspector] public AnimatorStateInfo StateInfo;
    [HideInInspector] public PlayerAnimationSetter AnimationSetter;
    [HideInInspector] public PlayerState CurrentPlayerState;
    [HideInInspector] public PlayerStateMachine PlayerStateMachine;
    [HideInInspector] public bool IsLockOn;
    [HideInInspector] public bool IsLookRight;
    GameObject targetMonster;
    int targetMonsterIndex = -1;
    Coroutine endParryCoroutine;
    Coroutine waitMotionCoroutine;
    Coroutine parrySucceedDurationCoroutine;

    void Awake()
    {
        MoveSpeed = 5f;
        DashSpeed = 10f;
        IsLockOn = false;
        IsGrounded = true;

        Animator = GetComponent<Animator>();
        AnimationSetter = GetComponent<PlayerAnimationSetter>();
        CharacterController = GetComponent<CharacterController>();
        inputActionAsset = GetComponent<PlayerInput>().actions;

        PlayerStateMachine = new PlayerStateMachine(this);
        PlayerStateMachine.Initialize(PlayerStateMachine.idleAndMoveState);

        moveAction = inputActionAsset.FindAction("Move");
        jumpAction = inputActionAsset.FindAction("Jump");
        dashAndPenetrateAction = inputActionAsset.FindAction("DashAndPenetrate");
        attack1Action = inputActionAsset.FindAction("Attack1");
        attack2Action = inputActionAsset.FindAction("Attack2");
        attack3Action = inputActionAsset.FindAction("Attack3");
        guardAction = inputActionAsset.FindAction("Guard");
        lockOnAction = inputActionAsset.FindAction("LockOn");
        interactAction = inputActionAsset.FindAction("Interact");
    }

    void Update()
    {
        if (transform.position.x != 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }

        StateInfo = Animator.GetCurrentAnimatorStateInfo(0);

        PlayerStateMachine.Execute();

        MoveActionValue = moveAction.ReadValue<float>();


        Gravity();
        ManageRotate();
        LockOnPressed();

        DashPressed();
        GuardPressed();
        BasicHorizonSlashPressed();
        BasicVerticalSlashPressed();
        ThrustPressed();
        IdleAndMovePressed();
    }

    void Gravity()
    {
        VerticalSpeed -= gravity * Time.deltaTime;

        VerticalSpeed = Mathf.Clamp(VerticalSpeed, -terminalSpeed, terminalSpeed);

        Vector3 verticalMove = new Vector3(0, VerticalSpeed, 0);

        CollisionFlags flag = CharacterController.Move(verticalMove * Time.deltaTime);

        if ((flag & CollisionFlags.Below) != 0)
        {
            VerticalSpeed = 0;
        }
    }
    void ManageRotate()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash) return; // 공격 중이거나 대쉬 중이라면 회전 금지

        if (IsLockOn)
        {
            if (targetMonster.transform.position.z >= transform.position.z)
            {
                transform.eulerAngles = Vector3.zero;
                IsLookRight = true;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                IsLookRight = false;
            }
        }
        else
        {
            if (MoveActionValue > 0)
            {
                transform.eulerAngles = Vector3.zero;
                IsLookRight = true;
            }
            else if (MoveActionValue < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                IsLookRight = false;
            }
        }
    }
    public void AttackMoving(float moveSpeed) // 공격 시 앞으로 조금씩 움직이는 기능. 공격 시 IsAttackingMoving이 true가 되면 플레이어가 공격 방향으로 움직인다.
    {
        if (IsLookRight)
        {
            Vector3 moveVector = new Vector3(0, 0, 1);
            CharacterController.Move(moveVector * moveSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 moveVector = new Vector3(0, 0, -1);
            CharacterController.Move(moveVector * moveSpeed * Time.deltaTime);
        }
    }
    void LockOnPressed() // 락온 상태 관리
    {
        if (lockOnAction.WasPressedThisFrame())
        {
            if (!NearbyMonsterCheck.IsMonsterExist()) // 근처에 적이 없다면 변수를 초기화하고 리턴
            {
                IsLockOn = false;
                targetMonster = null;
                targetMonsterIndex = -1;
                CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
                return;
            }

            IsLockOn = true;

            if (targetMonsterIndex + 1 > NearbyMonsterCheck.Monsters.Count - 1) // 락온할 다음 몬스터가 없다면 변수를 초기화하고 락온 상태 종료
            {
                IsLockOn = false;
                targetMonster = null;
                targetMonsterIndex = -1;
                CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
            }
            else // 락온할 다음 몬스터가 있다면 targetMonster를 다음 몬스터로 변경
            {
                targetMonsterIndex += 1;
                targetMonster = NearbyMonsterCheck.Monsters[targetMonsterIndex];
                CameraFocusPosition.position = (transform.position + targetMonster.transform.position) / 2;
            }
        }

        if (IsLockOn && !NearbyMonsterCheck.Monsters.Contains(targetMonster)) // 락온 상태인데 몬스터가 너무 멀리 떨어진다면(범위에서 벗어난다면) 락온 해제
        {
            IsLockOn = false;
            targetMonster = null;
            targetMonsterIndex = -1;
            CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
        }
    }
    void ManageParrySucceedDuration()
    {
        if (IsParrySucceed)
        {
            if (parrySucceedDurationCoroutine == null)
            {
                parrySucceedDurationCoroutine = StartCoroutine(ParrySucceedDuration());
            }
            else
            {
                StopCoroutine(parrySucceedDurationCoroutine);
                parrySucceedDurationCoroutine = StartCoroutine(ParrySucceedDuration());
            }
        }
    }
    void IdleAndMovePressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (moveAction.IsPressed() && CurrentPlayerState != PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }

    void DashPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (dashAndPenetrateAction.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.dashState);
        }
    }

    void BasicHorizonSlashPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack1Action.WasPressedThisFrame() && CanBasicHorizonSlashCombo)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicHorizonSlash2State);
        }
        else if (attack1Action.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicHorizonSlash1State);
        }
    }

    void BasicVerticalSlashPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack2Action.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicVerticalSlashState);
        }
    }

    void GuardPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash) return;

        if (guardAction.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.guardState);
            StartCoroutine(EndParry());
        }

        if (guardAction.WasReleasedThisFrame())
        {
            if (StateInfo.IsName("GuardHit") || StateInfo.IsName("Parry")) // 가드나 패리 시 막기 키를 바로 때도 모션이 끊기지 않도록 방지하는 코드
            {
                StartCoroutine(WaitGuardHitAndParryMotion(StateInfo.length * (1 - StateInfo.normalizedTime)));
            }
            else
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
            }
        }
    }

    void ThrustPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack3Action.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.thrustState);
        }
    }












    IEnumerator CanBasicHorizonSlashTerm()
    {
        yield return new WaitForSeconds(1f);
        CanBasicHorizonSlashCombo = false;
    }
    IEnumerator EndParry() // 패리가 인정되는 시간
    {
        yield return new WaitForSeconds(0.15f);
        IsParring = false;
    }
    IEnumerator WaitGuardHitAndParryMotion(float remainDuration) // 가드 및 패리 시 모션이 유지되는 시간
    {
        yield return new WaitForSeconds(remainDuration);
        PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
    }
    IEnumerator ParrySucceedDuration()
    {
        yield return new WaitForSeconds(0.5f);
        IsParrySucceed = false;
    }
}

























//legacy
// [HideInInspector] public float JumpSpeed;

//void OnJump()
//{
//    if (jumpAction.WasPressedThisFrame() && IsGrounded && GroundCheck.IsTouching && CurrentPlayerState == PlayerState.IdleAndMove)
//    {
//        PlayerStateMachine.TransitionTo(PlayerStateMachine.airborneState);

//        VerticalSpeed = JumpSpeed;
//        IsGrounded = false;
//    }
//    else if (!GroundCheck.IsTouching && IsGrounded && CurrentPlayerState == PlayerState.IdleAndMove)
//    {
//        PlayerStateMachine.TransitionTo(PlayerStateMachine.airborneState);
//    }

//    if (GroundCheck.IsTouching && !IsGrounded && CurrentPlayerState == PlayerState.Airborne) // 착지 시 (에러 있음)
//    {
//        PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
//        Debug.Log("범인?");
//    }

//    if (CurrentPlayerState == PlayerState.Airborne) // 공중에 있을 때
//    {
//        PlayerStateMachine.Execute();
//    }

//    IsGrounded = GroundCheck.IsTouching;
//}