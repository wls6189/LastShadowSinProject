using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState // 플레이어의 현재 행동(혹은 상태)
{
    Dead,
    IdleAndMove,
    UseChaliceOfAtonement,
    Dash,
    SpiritParry,
    Penetrate,
    Parry,
    GuardHit,
    HitShortGroggy,
    HitLongGroggy,
    Guard,
    BasicHorizonSlash1,
    BasicHorizonSlash2,
    BasicVerticalSlash,
    Thrust,
    RetreatSlash,
    SpiritCleave1,
    SpiritCleave2,
    SpiritCleave3,
}

public class PlayerController : MonoBehaviour
{
    // 인스펙터에서 할당이 필요한 변수
    public NearbyMonsterCheck NearbyMonsterCheck; // 근처의 몬스터를 감지하는 클래스
    public Transform CameraFocusPosition; // 카메라의 포커스 위치

    // 움직임 관련 변수
    [HideInInspector] public float MoveActionValue; // 움직임 방향키를 누를 때 반환되는 값을 캐싱
    [HideInInspector] public float MoveSpeed; // 움직이는 속도

    // 점프 관련 변수
    float gravity = 17; // 중력값
    float verticalSpeed; // 플레이어의 수직 속도(중력에서 관리)
    float terminalSpeed = 20; // 수직 속도의 한계

    // 대쉬 관련 변수
    [HideInInspector] public float DashSpeed; // 대쉬 시 속도

    // 락온 관련 변수
    [HideInInspector] public bool IsLockOn; // 락온 여부
    GameObject targetMonster; // 락온 타겟
    int targetMonsterIndex = -1; // 락온 타겟 인덱스

    // 공격 관련 변수
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsAttackMoving;
    [HideInInspector] public bool IsAttackColliderEnabled;
    [HideInInspector] public bool CanBasicHorizonSlash2Combo;
    [HideInInspector] public bool CanSpiritCleave2Combo;
    [HideInInspector] public bool CanSpiritCleave3Combo;
    float lastBasicHorizonSlash1AttackTime;
    float lastSpiritCleave1AttackTime;
    float lastSpiritCleave2AttackTime;
    int ThrustSpiritWaveConsume = 1;
    int RetreatSpiritWaveConsume = 1;
    int SpiritCleave1SpiritWaveConsume = 1;
    int SpiritCleave2And3SpiritWaveConsume = 2;
    int SpiritPiercingSpiritWaveConsume = 2;
    int SpiritAnnihilationSpiritWaveConsume = 3;
    int SpiritSwordDanceSpiritWaveConsume = 3;
    int SpiritUnboundSpiritWaveConsume = 2;
    int SpiritParrySpiritWaveConsume = 2;

    // 막기 관련 변수
    [HideInInspector] public bool IsParring;
    [HideInInspector] public bool IsSpiritParring;
    [HideInInspector] public bool IsGuarding;
    [HideInInspector] public bool IsAttackingParring;
    [HideInInspector] public bool CanPenetrate;
    Coroutine parryEndedTermRoutine; // 패리 인정 시간 코루틴 관리하는 변수

    // 조작 관련 변수
    [HideInInspector] public CharacterController CharacterController;
    InputActionAsset inputActionAsset;
    InputAction gameMenuAction;
    InputAction moveAction;
    InputAction dashAndPenetrateAction;
    InputAction attack1Action;
    InputAction attack2Action;
    InputAction attack3Action;
    [HideInInspector] public InputAction guardAction;
    InputAction lockOnAction;
    InputAction useChaliceOfAtonementAction;
    InputAction specialAttackAction;
    InputAction spiritualAction;

    // 상태 머신 관련 변수
    [HideInInspector] public AnimatorStateInfo StateInfo;
    [HideInInspector] public PlayerState CurrentPlayerState;
    [HideInInspector] public PlayerStateMachine PlayerStateMachine;

    // 부가적인 변수
    [HideInInspector] public Animator PlayerAnimator;
    [HideInInspector] public PlayerStats PlayerStats;
    [HideInInspector] public ChaliceOfAtonement PlayerChaliceOfAtonement;
    [HideInInspector] public bool IsGrogging; // 행동 불능 상태 여부, 전투 및 비전투 여부도 이걸로 확인 // 현재 사용 : 짧은 행동 불능, 긴 행동 불능, 막기, 패리, 영혼 패리, 간파
    [HideInInspector] public bool IsLookRight; // 오른쪽을 보고 있는지 여부
    [HideInInspector] public bool IsDoSomething; // 다른 행위를 할 수 없는 특수 행동 중일 때 사용. 현재 사용 : 물약 사용 시
    [HideInInspector] public bool IsInCombat; // 전투 중(의지력 회복 불가)인지 비전투 중(의지력 회복)인지 여부
    [HideInInspector] public bool IsAttackSucceed; // 전투 중(의지력 회복 불가)인지 비전투 중(의지력 회복)인지 여부
    float lastInCombatTime;

    void Awake()
    {
        MoveSpeed = 5f;
        DashSpeed = 10f;
        IsLockOn = false;
        IsLookRight = true; // 대부분 시작 시 오른쪽을 보면서 스폰되서 true로 설정.

        PlayerAnimator = GetComponent<Animator>();
        PlayerStats = GetComponent<PlayerStats>();
        PlayerChaliceOfAtonement = GetComponent<ChaliceOfAtonement>();
        CharacterController = GetComponent<CharacterController>();
        inputActionAsset = GetComponent<PlayerInput>().actions;

        PlayerStateMachine = new PlayerStateMachine(this);
        PlayerStateMachine.Initialize(PlayerStateMachine.idleAndMoveState);
        gameMenuAction = inputActionAsset.FindAction("GameMenu");
        moveAction = inputActionAsset.FindAction("Move");
        dashAndPenetrateAction = inputActionAsset.FindAction("DashAndPenetrate");
        attack1Action = inputActionAsset.FindAction("Attack1");
        attack2Action = inputActionAsset.FindAction("Attack2");
        attack3Action = inputActionAsset.FindAction("Attack3");
        guardAction = inputActionAsset.FindAction("Guard");
        lockOnAction = inputActionAsset.FindAction("LockOn");
        useChaliceOfAtonementAction = inputActionAsset.FindAction("UseChaliceOfAtonement");
        specialAttackAction = inputActionAsset.FindAction("SpecialAttack");
        spiritualAction = inputActionAsset.FindAction("Spiritual");
    }

    void Update()
    {
        if (CurrentPlayerState == PlayerState.Dead) return;
        if (transform.position.x != 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }

        StateInfo = PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        PlayerStateMachine.Execute();

        MoveActionValue = moveAction.ReadValue<float>();

        Gravity();

        if (DialogSystem.Instance.isdialogueCanvas) return; // 다이얼로그 열리면 키 입력 및 행동 금지(추후 TimeScale로 다루는 것에 대한 여부)
        ManageRotate();
        HandleBasicHorizonSlash2Combo();
        HandleSpiritCleave2Combo();
        HandleSpiritCleave3Combo();
        ManageIsInCombat();

        GameMenuPressed();
        LockOnPressed();
        UseChaliceOfAtonementPressed();
        IdleAndMovePressed();
        DashAndPenetratePressed();
        GuardPressed();
        Attack1Pressed();
        Attack2Pressed();
    }

    void Gravity() // 중력 관리
    {
        verticalSpeed -= gravity * Time.deltaTime;

        verticalSpeed = Mathf.Clamp(verticalSpeed, -terminalSpeed, terminalSpeed);

        Vector3 verticalMove = new Vector3(0, verticalSpeed, 0);

        CollisionFlags flag = CharacterController.Move(verticalMove * Time.deltaTime);

        if ((flag & CollisionFlags.Below) != 0)
        {
            verticalSpeed = 0;
        }
    }
    void ManageRotate() // 캐릭터의 회전 관리
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash) return; // 공격 중이거나 대쉬 중이라면 회전 금지

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
    void HandleBasicHorizonSlash2Combo() // 기본 횡베기 2번째 콤보 관리
    {
        if (CanBasicHorizonSlash2Combo)
        {
            lastBasicHorizonSlash1AttackTime += Time.deltaTime;
        }
        else
        {
            lastBasicHorizonSlash1AttackTime = 0;
            return;
        }

        if (lastBasicHorizonSlash1AttackTime > 1f)
        {
            CanBasicHorizonSlash2Combo = false;
            lastBasicHorizonSlash1AttackTime = 0;
        }
    }
    void HandleSpiritCleave2Combo() // 영혼 가르기 2번째 콤보 관리
    {
        if (CanSpiritCleave2Combo)
        {
            lastSpiritCleave1AttackTime += Time.deltaTime;
        }
        else
        {
            lastSpiritCleave1AttackTime = 0;
            return;
        }

        if (lastSpiritCleave1AttackTime > 1f)
        {
            CanSpiritCleave2Combo = false;
            lastSpiritCleave1AttackTime = 0;
        }
    }
    void HandleSpiritCleave3Combo() // 영혼 가르기 3번째 콤보 관리
    {
        if (CanSpiritCleave3Combo)
        {
            lastSpiritCleave2AttackTime += Time.deltaTime;
        }
        else
        {
            lastSpiritCleave2AttackTime = 0;
            return;
        }

        if (lastSpiritCleave2AttackTime > 1f)
        {
            CanSpiritCleave3Combo = false;
            lastSpiritCleave2AttackTime = 0;
        }
    }
    void ManageIsInCombat()
    {
        if (IsAttackSucceed || IsGrogging)
        {
            lastInCombatTime = 0;
        }
        else
        {
            lastInCombatTime += Time.deltaTime;
        }

        if (lastInCombatTime > 4f)
        {
            IsInCombat = false;
        }
        else
        {
            IsInCombat = true;
        }
    }

    void GameMenuPressed() // 게임 메뉴 켜기
    {
        if (gameMenuAction.WasPressedThisFrame())
        {
            if (UIManager.Instance.IsGameMenuOpen)
            {
                UIManager.Instance.GameMenuClose();
            }
            else
            {
                UIManager.Instance.GameMenuOpen();
            }
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
            }
        }

        if (IsLockOn && !NearbyMonsterCheck.Monsters.Contains(targetMonster)) // 락온 상태인데 몬스터가 너무 멀리 떨어진다면(범위에서 벗어난다면) 락온 해제
        {
            IsLockOn = false;
            targetMonster = null;
            targetMonsterIndex = -1;
            CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
        }

        if (IsLockOn) // 락온 중엔 카메라가 플레이어와 타겟 사이를 포커싱
        {
            CameraFocusPosition.position = (transform.position + targetMonster.transform.position) / 2;
        }
    }
    void UseChaliceOfAtonementPressed() // 물약(속죄의 성배) 사용
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (useChaliceOfAtonementAction.WasPressedThisFrame() && PlayerChaliceOfAtonement.CurrentChaliceOfAtonementCount > 0)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.useChaliceOfAtonementState);
        }
    }
    void IdleAndMovePressed() // 아이들 및 움직임 관리
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (moveAction.IsPressed() && CurrentPlayerState != PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }
    void DashAndPenetratePressed() // 대쉬 및 간파 관리
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (dashAndPenetrateAction.WasPressedThisFrame())
        {
            if (CanPenetrate)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.penetrateState);
            }
            else
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.dashState);
            }
        }
    }
    void GuardPressed() // 가드 관리
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash) return;

        if (guardAction.IsPressed() && CurrentPlayerState != PlayerState.Guard) // IdleAndMove나 Guard일 때만 가드 전환
        {
            if (SpiritualPressed() && PlayerStats.CurrentSpiritWave >= SpiritParrySpiritWaveConsume) // 영혼 패리 발동 조건
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritParryState);
            }
            else
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.guardState);

                if (parryEndedTermRoutine == null)
                {
                    parryEndedTermRoutine = StartCoroutine(ParryEndedTerm());
                }
                else
                {
                    StopCoroutine(parryEndedTermRoutine);
                    parryEndedTermRoutine = StartCoroutine(ParryEndedTerm());
                }
            }
        }

        if (guardAction.WasReleasedThisFrame() && CurrentPlayerState == PlayerState.Guard) // 가드 상태일 때 키를 때면 디폴트 상태로 이동
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }
    void Attack1Pressed() // 공격 Z키 관리
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack1Action.WasPressedThisFrame())
        {
            if (SpiritualPressed() && CanSpiritCleave3Combo && PlayerStats.CurrentSpiritWave >= SpiritCleave2And3SpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritCleave3State);
            }
            else if (SpiritualPressed() && CanSpiritCleave2Combo && PlayerStats.CurrentSpiritWave >= SpiritCleave2And3SpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritCleave2State);
            }
            else if (SpiritualPressed() && PlayerStats.CurrentSpiritWave >= SpiritCleave1SpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritCleave1State);
            }
            else if (SpecialAttackPressed() && PlayerStats.CurrentSpiritWave >= ThrustSpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.thrustState);
            }
            else if (CanBasicHorizonSlash2Combo)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.basicHorizonSlash2State);
            }
            else
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.basicHorizonSlash1State);
            }
        }

    }
    void Attack2Pressed() // 공격 X키 관리
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack2Action.WasPressedThisFrame())
        {
            if (SpecialAttackPressed() && PlayerStats.CurrentSpiritWave >= RetreatSpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.retreatSlashState);
            }
            else
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.basicVerticalSlashState);
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
    bool SpecialAttackPressed() // 아랫방향키 눌린 상태 반환
    {
        return specialAttackAction.IsPressed();
    }
    bool SpiritualPressed() // Shift 눌린 상태 반환
    {
        return spiritualAction.IsPressed();
    }
    IEnumerator ParryEndedTerm() // 패리가 인정되는 시간
    {
        IsParring = true;
        yield return new WaitForSeconds(0.15f);
        IsParring = false;
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