using System;
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
    Grabbed,
    ClashGuard,
    Guard,
    BasicHorizonSlash1,
    BasicHorizonSlash2,
    BasicVerticalSlash,
    Thrust,
    RetreatSlash,
    SpiritCleave1,
    SpiritCleave2,
    SpiritCleave3,
    SpiritPiercing,
    SpiritSwordDance,
    SpiritUnbound,
    SpiritNova
}

public class PlayerController : MonoBehaviour
{
    // 인스펙터에서 할당이 필요한 변수
    public NearbyMonsterCheck NearbyMonsterCheck; // 근처의 몬스터를 감지하는 클래스
    public Transform CameraFocusPosition; // 카메라의 포커스 위치
    public GameObject SpiritUnboundPrefab; // 영력 해방 발사체 프리팹

    // 움직임 관련 변수
    [HideInInspector] public float MoveActionValue; // 움직임 방향키를 누를 때 반환되는 값을 캐싱
    [HideInInspector] public float MoveSpeed; // 움직이는 속도

    // 점프 관련 변수
    float gravity = 17; // 중력값
    float verticalSpeed; // 플레이어의 수직 속도(중력에서 관리)
    float terminalSpeed = 20; // 수직 속도의 한계

    // 대쉬 관련 변수
    [HideInInspector] public float DashSpeed; // 대쉬 시 속도
    [HideInInspector] public bool OnDashEffect;

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
    [HideInInspector] public bool CanSpiritNova;
    float lastBasicHorizonSlash1AttackTime;
    float lastSpiritCleave1AttackTime;
    float lastSpiritCleave2AttackTime;
    float lastSpiritCleave3AttackTime;
    [HideInInspector] public bool IsSpiritSwordDanceSecondAttack;
    [HideInInspector] public Action CallWhenDamaging;
    // 공격의 영혼의 파동 소모량
    int DashSpiritWaveConsume = 1;
    int ThrustSpiritWaveConsume = 1;
    int RetreatSpiritWaveConsume = 1;
    int SpiritCleave1SpiritWaveConsume = 1;
    int SpiritCleave2And3SpiritWaveConsume = 2;
    int SpiritPiercingSpiritWaveConsume = 2;
    int SpiritSwordDanceSpiritWaveConsume = 3;
    int SpiritUnboundSpiritWaveConsume = 2;
    int SpiritNovaSpiritWaveConsume = 2;
    int SpiritParrySpiritWaveConsume = 2;

    // 막기 관련 변수
    [HideInInspector] public bool IsParring;
    [HideInInspector] public bool IsSpiritParring;
    [HideInInspector] public bool IsClashGuard;
    [HideInInspector] public bool IsGuarding;
    [HideInInspector] public bool IsAttackingParryColliderEnabled;
    [HideInInspector] public bool CanPenetrate;
    [HideInInspector] public Action CallWhenGuarding;
    Coroutine parryEndedTermRoutine; // 패리 인정 시간 코루틴 관리하는 변수

    // 조작 관련 변수
    [HideInInspector] public CharacterController PlayerCharacterController;
    PlayerInput playerInput;
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
    [HideInInspector] public InputAction specialAttackAction;
    InputAction spiritualAction;
    [HideInInspector] public InputAction spiritMarkAbilityAction;
    [HideInInspector] public InputAction moveTabLeftAction;
    [HideInInspector] public InputAction moveTabRightAction;
    [HideInInspector] public InputAction moveSelectLeftAction;
    [HideInInspector] public InputAction moveSelectRightAction;
    [HideInInspector] public InputAction moveSelectUpAction;
    [HideInInspector] public InputAction moveSelectDownAction;
    [HideInInspector] public InputAction selectAction;

    // 상태 머신 관련 변수
    [HideInInspector] public AnimatorStateInfo StateInfo;
    public PlayerState CurrentPlayerState;
    [HideInInspector] public PlayerStateMachine PlayerStateMachine;

    // 부가적인 변수
    [HideInInspector] public Animator PlayerAnimator;
    [HideInInspector] public PlayerStats PlayerStats;
    [HideInInspector] public ChaliceOfAtonement PlayerChaliceOfAtonement;
    [HideInInspector] public ESMInventory PlayerESMInventory;
    [HideInInspector] public MarkInventory PlayerMarkInventory;
    [HideInInspector] public ManagePlayerEffect ManagePlayerEffect;
    [HideInInspector] public bool IsGrogging; // 행동 불능 상태 여부, 전투 및 비전투 여부도 이걸로 확인 // 현재 사용 : 짧은 행동 불능, 긴 행동 불능, 막기, 패리, 영혼 패리, 간파
    [HideInInspector] public bool IsLookRight; // 오른쪽을 보고 있는지 여부
    [HideInInspector] public bool IsDoSomething; // 다른 행위를 할 수 없는 특수 행동 중일 때 사용. 현재 사용 : 물약 사용 시
    [HideInInspector] public bool IsInCombat; // 전투 중(의지력 회복 불가)인지 비전투 중(의지력 회복)인지 여부
    [HideInInspector] public bool IsAttackSucceed; // 전투 중(의지력 회복 불가)인지 비전투 중(의지력 회복)인지 여부
    float lastInCombatTime;
    float lastPressArrowTime;
    float lastPressCtrlZXTime;
    float lastPressCTime;
    bool canPressC;
    [HideInInspector] public bool IsRadicalESMAttackPosture; // 극단적인 영원의 영혼낙인 자세 여부


    void Awake()
    {
        MoveSpeed = 5f;
        DashSpeed = 20f;
        IsLockOn = false;
        IsLookRight = true; // 대부분 시작 시 오른쪽을 보면서 스폰되서 true로 설정.

        PlayerAnimator = GetComponent<Animator>();
        PlayerStats = GetComponent<PlayerStats>();
        PlayerChaliceOfAtonement = GetComponent<ChaliceOfAtonement>();
        TryGetComponent(out PlayerESMInventory);
        TryGetComponent(out PlayerMarkInventory);
        TryGetComponent(out ManagePlayerEffect);
        PlayerCharacterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
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
        spiritMarkAbilityAction = inputActionAsset.FindAction("SpiritMarkAbility");
        moveTabLeftAction = inputActionAsset.FindAction("MoveTabLeft");
        moveTabRightAction = inputActionAsset.FindAction("MoveTabRight");
        moveSelectLeftAction = inputActionAsset.FindAction("MoveSelectLeft");
        moveSelectRightAction = inputActionAsset.FindAction("MoveSelectRight");
        moveSelectUpAction = inputActionAsset.FindAction("MoveSelectUp");
        moveSelectDownAction = inputActionAsset.FindAction("MoveSelectDown");
        selectAction = inputActionAsset.FindAction("Select");

        IsRadicalESMAttackPosture = true;
    }

    void Start()
    {
        SMAndESMUIManager.Instance.SetPlayerController(this);
        InGameUIManager.Instance.SetPlayerController(this);
    }
    void Update()
    {
        if (CurrentPlayerState == PlayerState.Dead) return;
        if (DialogSystem.Instance.isdialogueCanvas) return; // 다이얼로그 열리면 키 입력 및 행동 금지(추후 TimeScale로 다루는 것에 대한 여부)

        if (transform.position.x != 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }

        ManagePressArrow();
        ManagePressCtrlZX();
        ManagePressC();

        StateInfo = PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        MoveActionValue = moveAction.ReadValue<float>();

        PlayerStateMachine.Execute();

        Gravity();

        ManageRotate();
        HandleBasicHorizonSlash2Combo();
        HandleSpiritCleave2Combo();
        HandleSpiritCleave3Combo();
        HandleSpiritNova();
        ManageIsInCombat();

        GameMenuPressed();
        LockOnPressed();
        UseChaliceOfAtonementPressed();
        DashAndPenetratePressed();
        GuardPressed();
        Attack1Pressed();
        Attack2Pressed();

        IdleAndMovePressed();
    }

    void Gravity() // 중력 관리
    {
        verticalSpeed -= gravity * Time.deltaTime;

        verticalSpeed = Mathf.Clamp(verticalSpeed, -terminalSpeed, terminalSpeed);

        Vector3 verticalMove = new Vector3(0, verticalSpeed, 0);

        CollisionFlags flag = PlayerCharacterController.Move(verticalMove * Time.deltaTime);

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
            lastBasicHorizonSlash1AttackTime = Mathf.Clamp(lastBasicHorizonSlash1AttackTime, 0f, 2f);
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
            lastSpiritCleave1AttackTime = Mathf.Clamp(lastSpiritCleave2AttackTime, 0f, 2f);
        }
        else
        {
            lastSpiritCleave1AttackTime = 0;
            return;
        }

        if (lastSpiritCleave1AttackTime > 1f) // 콤보 가능 시간(1.5초인 상태)
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
            lastSpiritCleave2AttackTime = Mathf.Clamp(lastSpiritCleave2AttackTime, 0f, 2f);
        }
        else
        {
            lastSpiritCleave2AttackTime = 0;
            return;
        }

        if (lastSpiritCleave2AttackTime > 1.5f)
        {
            CanSpiritCleave3Combo = false;
            lastSpiritCleave2AttackTime = 0;
        }
    }
    void HandleSpiritNova() // 혼력일섬 관리
    {
        if (CanSpiritNova)
        {
            lastSpiritCleave3AttackTime += Time.deltaTime;
            lastSpiritCleave3AttackTime = Mathf.Clamp(lastSpiritCleave3AttackTime, 0f, 2f);
        }
        else
        {
            lastSpiritCleave3AttackTime = 0;
            return;
        }

        if (lastSpiritCleave3AttackTime > 1f)
        {
            CanSpiritNova = false;
            lastSpiritCleave3AttackTime = 0;
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
    void ManagePressArrow()
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash)
        {
            lastPressArrowTime = 0;
        }
        else
        {
            lastPressArrowTime += Time.deltaTime;
            lastPressArrowTime = Mathf.Clamp01(lastPressArrowTime);
        }

        if (lastPressArrowTime > 0.03f)
        {
            moveAction.Enable();
        }
        else
        {
            moveAction.Disable();
        }
    }
    void ManagePressCtrlZX()
    {
        if ((dashAndPenetrateAction.enabled || attack1Action.enabled || attack2Action.enabled) && (dashAndPenetrateAction.WasPressedThisFrame() || attack1Action.WasPressedThisFrame() || attack2Action.WasPressedThisFrame()))
        {
            lastPressCtrlZXTime = 0;
        }
        else
        {
            lastPressCtrlZXTime += Time.deltaTime;
        }

        if (lastPressCtrlZXTime > 0.07f)
        {
            dashAndPenetrateAction.Enable();
            attack1Action.Enable();
            attack2Action.Enable();
        }
        else
        {
            dashAndPenetrateAction.Disable();
            attack1Action.Disable();
            attack2Action.Disable();
        }
    }
    void ManagePressC()
    {
        if (guardAction.enabled && guardAction.WasCompletedThisFrame())
        {
            lastPressCTime = 0;
        }
        else
        {
            lastPressCTime += Time.deltaTime;
        }

        if (lastPressCTime > 0.08f)
        {
            canPressC = true;
        }
        else
        {
            canPressC = false;
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

        if (CurrentPlayerState == PlayerState.IdleAndMove && !StateInfo.IsName("IdleAndMove") && !PlayerAnimator.IsInTransition(0))
        {
            PlayerAnimator.SetTrigger("DoIdleAndMove");
        }
    }
    void DashAndPenetratePressed() // 대쉬 및 간파 관리
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (dashAndPenetrateAction.WasPressedThisFrame())
        {
            if (CanPenetrate)
            {
                if (PlayerESMInventory.EquipedESM != null && PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && IsRadicalESMAttackPosture) return; // 극단적인 영원의 영혼낙인 공격 자세 시엔 간파 시전 불가

                PlayerStateMachine.TransitionTo(PlayerStateMachine.penetrateState);
            }
            else if (PlayerStats.CurrentSpiritWave >= DashSpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.dashState);
            }
        }
    }
    void GuardPressed() // 가드 관리
    {
        if (PlayerESMInventory.EquipedESM != null && PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && IsRadicalESMAttackPosture) return;

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

        if (!guardAction.IsPressed() && CurrentPlayerState == PlayerState.Guard) // 가드 상태일 때 키를 때면 디폴트 상태로 이동
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }
    void Attack1Pressed() // 공격 Z키 관리
    {
        if (PlayerESMInventory.EquipedESM != null && PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !IsRadicalESMAttackPosture) return;
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack1Action.WasPressedThisFrame())
        {
            if (SpiritualPressed() && CanSpiritNova && PlayerStats.CurrentSpiritWave >= SpiritNovaSpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritNovaState);
            }
            else if (SpiritualPressed() && SpecialAttackPressed() && PlayerStats.CurrentSpiritWave >= SpiritSwordDanceSpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritSwordDanceState);
            }
            else if (SpiritualPressed() && CanSpiritCleave3Combo && PlayerStats.CurrentSpiritWave >= SpiritCleave2And3SpiritWaveConsume)
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
        if (PlayerESMInventory.EquipedESM != null && PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !IsRadicalESMAttackPosture) return;

        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack2Action.WasPressedThisFrame())
        {
            if (SpiritualPressed() && SpecialAttackPressed() && PlayerStats.CurrentSpiritWave >= SpiritUnboundSpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritUnboundState);
            }
            else if (SpiritualPressed() && PlayerStats.CurrentSpiritWave >= SpiritPiercingSpiritWaveConsume) // 영혼 관통. 영혼의 파동 2개 이상 시
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritPiercingState);
            }
            else if (SpecialAttackPressed() && PlayerStats.CurrentSpiritWave >= RetreatSpiritWaveConsume) // 후퇴베기. 영혼의 파동 1개 이상 시
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.retreatSlashState);
            }
            else // 기본 내려찍기.
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
            PlayerCharacterController.Move(moveVector * moveSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 moveVector = new Vector3(0, 0, -1);
            PlayerCharacterController.Move(moveVector * moveSpeed * Time.deltaTime);
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
        IsClashGuard = true;
        yield return new WaitForSeconds(0.15f);
        IsParring = false;
        IsClashGuard = false;
    }
    public void FireSpiritUnboundProjectile(float surgingESMPercent)
    {
        if (surgingESMPercent < 0)
        {
            GameObject go = Instantiate(SpiritUnboundPrefab);
            go.GetComponent<AttackCheck>().player = this;
            go.GetComponent<AttackCheck>().IsProjectile = true;
            go.GetComponent<AttackCheck>().ProjectilePercentage = 1.1f;

            if (IsLookRight)
            {
                go.transform.position = new Vector3(transform.position.x, 1.57f, transform.position.z + 0.7f);
                go.GetComponent<Projectile>().SetIsMoveRight(true);
            }
            else
            {
                go.transform.position = new Vector3(transform.position.x, 1.57f, transform.position.z - 0.7f);
                go.GetComponent<Projectile>().SetIsMoveRight(false);
            }
        }
        else if (surgingESMPercent >= 0)
        {
            int projectileCount = 0;

            if (surgingESMPercent >= 100) projectileCount = 10;
            else if (surgingESMPercent >= 50) projectileCount = 5;
            else if (surgingESMPercent >= 25) projectileCount = 2;

            if (projectileCount == 0) return;

            for (int i = 0; i < projectileCount; i++)
            {
                float y = UnityEngine.Random.Range(1f, 2f);
                float z = UnityEngine.Random.Range(0.2f, 1.2f);

                GameObject go = Instantiate(SpiritUnboundPrefab);
                go.GetComponent<AttackCheck>().player = this;
                go.GetComponent<AttackCheck>().IsProjectile = true;
                go.GetComponent<AttackCheck>().ProjectilePercentage = 1.1f;

                if (IsLookRight)
                {
                    go.transform.position = new Vector3(transform.position.x, y, transform.position.z + z);
                    go.GetComponent<Projectile>().SetIsMoveRight(true);
                }
                else
                {
                    go.transform.position = new Vector3(transform.position.x, y, transform.position.z - z);
                    go.GetComponent<Projectile>().SetIsMoveRight(false);
                }
            }
        }
    }
    public void FireRadicalESMProjectile()
    {
        GameObject go = Instantiate(ManagePlayerEffect.radicalESMPrefab);

        go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, transform.position.z);
        go.GetComponent<AttackCheck>().player = this;
        go.GetComponent<AttackCheck>().IsProjectile = true;
        go.GetComponent<AttackCheck>().ProjectilePercentage = 2.0f;

        Destroy(go, 0.3f);
    }
    public IEnumerator FireRagingESMProjectile()
    {
        for (int i = 0; i < PlayerStats.RagingStack; i++)
        {
            GameObject go = Instantiate(ManagePlayerEffect.radicalESMPrefab);

            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, transform.position.z);
            go.GetComponent<AttackCheck>().player = this;
            go.GetComponent<AttackCheck>().IsProjectile = true;
            go.GetComponent<AttackCheck>().ProjectilePercentage = 0.15f;
            Destroy(go, 0.3f);

            yield return new WaitForSeconds(0.25f);
        }

        PlayerStats.RagingStack = 0;
        yield return null;
    }





    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DroppedItem"))
        {
            other.GetComponent<DroppedItem>().PickUpItem(this);
        }
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