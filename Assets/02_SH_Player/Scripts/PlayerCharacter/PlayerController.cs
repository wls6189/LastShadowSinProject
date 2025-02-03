using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState // �÷��̾��� ���� �ൿ(Ȥ�� ����)
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
    // �ν����Ϳ��� �Ҵ��� �ʿ��� ����
    public NearbyMonsterCheck NearbyMonsterCheck; // ��ó�� ���͸� �����ϴ� Ŭ����
    public Transform CameraFocusPosition; // ī�޶��� ��Ŀ�� ��ġ
    public GameObject SpiritUnboundPrefab; // ���� �ع� �߻�ü ������

    // ������ ���� ����
    [HideInInspector] public float MoveActionValue; // ������ ����Ű�� ���� �� ��ȯ�Ǵ� ���� ĳ��
    [HideInInspector] public float MoveSpeed; // �����̴� �ӵ�

    // ���� ���� ����
    float gravity = 17; // �߷°�
    float verticalSpeed; // �÷��̾��� ���� �ӵ�(�߷¿��� ����)
    float terminalSpeed = 20; // ���� �ӵ��� �Ѱ�

    // �뽬 ���� ����
    [HideInInspector] public float DashSpeed; // �뽬 �� �ӵ�
    [HideInInspector] public bool OnDashEffect;

    // ���� ���� ����
    [HideInInspector] public bool IsLockOn; // ���� ����
    GameObject targetMonster; // ���� Ÿ��
    int targetMonsterIndex = -1; // ���� Ÿ�� �ε���

    // ���� ���� ����
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
    // ������ ��ȥ�� �ĵ� �Ҹ�
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

    // ���� ���� ����
    [HideInInspector] public bool IsParring;
    [HideInInspector] public bool IsSpiritParring;
    [HideInInspector] public bool IsClashGuard;
    [HideInInspector] public bool IsGuarding;
    [HideInInspector] public bool IsAttackingParryColliderEnabled;
    [HideInInspector] public bool CanPenetrate;
    [HideInInspector] public Action CallWhenGuarding;
    Coroutine parryEndedTermRoutine; // �и� ���� �ð� �ڷ�ƾ �����ϴ� ����

    // ���� ���� ����
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

    // ���� �ӽ� ���� ����
    [HideInInspector] public AnimatorStateInfo StateInfo;
    public PlayerState CurrentPlayerState;
    [HideInInspector] public PlayerStateMachine PlayerStateMachine;

    // �ΰ����� ����
    [HideInInspector] public Animator PlayerAnimator;
    [HideInInspector] public PlayerStats PlayerStats;
    [HideInInspector] public ChaliceOfAtonement PlayerChaliceOfAtonement;
    [HideInInspector] public ESMInventory PlayerESMInventory;
    [HideInInspector] public MarkInventory PlayerMarkInventory;
    [HideInInspector] public ManagePlayerEffect ManagePlayerEffect;
    [HideInInspector] public bool IsGrogging; // �ൿ �Ҵ� ���� ����, ���� �� ������ ���ε� �̰ɷ� Ȯ�� // ���� ��� : ª�� �ൿ �Ҵ�, �� �ൿ �Ҵ�, ����, �и�, ��ȥ �и�, ����
    [HideInInspector] public bool IsLookRight; // �������� ���� �ִ��� ����
    [HideInInspector] public bool IsDoSomething; // �ٸ� ������ �� �� ���� Ư�� �ൿ ���� �� ���. ���� ��� : ���� ��� ��
    [HideInInspector] public bool IsInCombat; // ���� ��(������ ȸ�� �Ұ�)���� ������ ��(������ ȸ��)���� ����
    [HideInInspector] public bool IsAttackSucceed; // ���� ��(������ ȸ�� �Ұ�)���� ������ ��(������ ȸ��)���� ����
    float lastInCombatTime;
    float lastPressArrowTime;
    float lastPressCtrlZXTime;
    float lastPressCTime;
    bool canPressC;
    [HideInInspector] public bool IsRadicalESMAttackPosture; // �ش����� ������ ��ȥ���� �ڼ� ����


    void Awake()
    {
        MoveSpeed = 5f;
        DashSpeed = 20f;
        IsLockOn = false;
        IsLookRight = true; // ��κ� ���� �� �������� ���鼭 �����Ǽ� true�� ����.

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
        if (DialogSystem.Instance.isdialogueCanvas) return; // ���̾�α� ������ Ű �Է� �� �ൿ ����(���� TimeScale�� �ٷ�� �Ϳ� ���� ����)

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

    void Gravity() // �߷� ����
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
    void ManageRotate() // ĳ������ ȸ�� ����
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash) return; // ���� ���̰ų� �뽬 ���̶�� ȸ�� ����

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
    void HandleBasicHorizonSlash2Combo() // �⺻ Ⱦ���� 2��° �޺� ����
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
    void HandleSpiritCleave2Combo() // ��ȥ ������ 2��° �޺� ����
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

        if (lastSpiritCleave1AttackTime > 1f) // �޺� ���� �ð�(1.5���� ����)
        {
            CanSpiritCleave2Combo = false;
            lastSpiritCleave1AttackTime = 0;
        }
    }
    void HandleSpiritCleave3Combo() // ��ȥ ������ 3��° �޺� ����
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
    void HandleSpiritNova() // ȥ���ϼ� ����
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
    void GameMenuPressed() // ���� �޴� �ѱ�
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
    void LockOnPressed() // ���� ���� ����
    {
        if (lockOnAction.WasPressedThisFrame())
        {
            if (!NearbyMonsterCheck.IsMonsterExist()) // ��ó�� ���� ���ٸ� ������ �ʱ�ȭ�ϰ� ����
            {
                IsLockOn = false;
                targetMonster = null;
                targetMonsterIndex = -1;
                CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
                return;
            }

            IsLockOn = true;

            if (targetMonsterIndex + 1 > NearbyMonsterCheck.Monsters.Count - 1) // ������ ���� ���Ͱ� ���ٸ� ������ �ʱ�ȭ�ϰ� ���� ���� ����
            {
                IsLockOn = false;
                targetMonster = null;
                targetMonsterIndex = -1;
                CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
            }
            else // ������ ���� ���Ͱ� �ִٸ� targetMonster�� ���� ���ͷ� ����
            {
                targetMonsterIndex += 1;
                targetMonster = NearbyMonsterCheck.Monsters[targetMonsterIndex];
            }
        }

        if (IsLockOn && !NearbyMonsterCheck.Monsters.Contains(targetMonster)) // ���� �����ε� ���Ͱ� �ʹ� �ָ� �������ٸ�(�������� ����ٸ�) ���� ����
        {
            IsLockOn = false;
            targetMonster = null;
            targetMonsterIndex = -1;
            CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
        }

        if (IsLockOn) // ���� �߿� ī�޶� �÷��̾�� Ÿ�� ���̸� ��Ŀ��
        {
            CameraFocusPosition.position = (transform.position + targetMonster.transform.position) / 2;
        }
    }
    void UseChaliceOfAtonementPressed() // ����(������ ����) ���
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (useChaliceOfAtonementAction.WasPressedThisFrame() && PlayerChaliceOfAtonement.CurrentChaliceOfAtonementCount > 0)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.useChaliceOfAtonementState);
        }
    }
    void IdleAndMovePressed() // ���̵� �� ������ ����
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
    void DashAndPenetratePressed() // �뽬 �� ���� ����
    {
        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (dashAndPenetrateAction.WasPressedThisFrame())
        {
            if (CanPenetrate)
            {
                if (PlayerESMInventory.EquipedESM != null && PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && IsRadicalESMAttackPosture) return; // �ش����� ������ ��ȥ���� ���� �ڼ� �ÿ� ���� ���� �Ұ�

                PlayerStateMachine.TransitionTo(PlayerStateMachine.penetrateState);
            }
            else if (PlayerStats.CurrentSpiritWave >= DashSpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.dashState);
            }
        }
    }
    void GuardPressed() // ���� ����
    {
        if (PlayerESMInventory.EquipedESM != null && PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && IsRadicalESMAttackPosture) return;

        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash) return;

        if (guardAction.IsPressed() && CurrentPlayerState != PlayerState.Guard) // IdleAndMove�� Guard�� ���� ���� ��ȯ
        {
            if (SpiritualPressed() && PlayerStats.CurrentSpiritWave >= SpiritParrySpiritWaveConsume) // ��ȥ �и� �ߵ� ����
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

        if (!guardAction.IsPressed() && CurrentPlayerState == PlayerState.Guard) // ���� ������ �� Ű�� ���� ����Ʈ ���·� �̵�
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }
    void Attack1Pressed() // ���� ZŰ ����
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
    void Attack2Pressed() // ���� XŰ ����
    {
        if (PlayerESMInventory.EquipedESM != null && PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !IsRadicalESMAttackPosture) return;

        if (IsAttacking || IsDoSomething || IsGrogging || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack2Action.WasPressedThisFrame())
        {
            if (SpiritualPressed() && SpecialAttackPressed() && PlayerStats.CurrentSpiritWave >= SpiritUnboundSpiritWaveConsume)
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritUnboundState);
            }
            else if (SpiritualPressed() && PlayerStats.CurrentSpiritWave >= SpiritPiercingSpiritWaveConsume) // ��ȥ ����. ��ȥ�� �ĵ� 2�� �̻� ��
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.spiritPiercingState);
            }
            else if (SpecialAttackPressed() && PlayerStats.CurrentSpiritWave >= RetreatSpiritWaveConsume) // ���𺣱�. ��ȥ�� �ĵ� 1�� �̻� ��
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.retreatSlashState);
            }
            else // �⺻ �������.
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.basicVerticalSlashState);
            }
        }
    }


    public void AttackMoving(float moveSpeed) // ���� �� ������ ���ݾ� �����̴� ���. ���� �� IsAttackingMoving�� true�� �Ǹ� �÷��̾ ���� �������� �����δ�.
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
    bool SpecialAttackPressed() // �Ʒ�����Ű ���� ���� ��ȯ
    {
        return specialAttackAction.IsPressed();
    }
    bool SpiritualPressed() // Shift ���� ���� ��ȯ
    {
        return spiritualAction.IsPressed();
    }
    IEnumerator ParryEndedTerm() // �и��� �����Ǵ� �ð�
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

//    if (GroundCheck.IsTouching && !IsGrounded && CurrentPlayerState == PlayerState.Airborne) // ���� �� (���� ����)
//    {
//        PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
//        Debug.Log("����?");
//    }

//    if (CurrentPlayerState == PlayerState.Airborne) // ���߿� ���� ��
//    {
//        PlayerStateMachine.Execute();
//    }

//    IsGrounded = GroundCheck.IsTouching;
//}