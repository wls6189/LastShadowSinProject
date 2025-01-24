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
    // �ν����Ϳ��� �Ҵ��� �ʿ��� ����
    public NearbyMonsterCheck NearbyMonsterCheck; // ��ó�� ���͸� �����ϴ� Ŭ����
    public Transform CameraFocusPosition; // ī�޶��� ��Ŀ�� ��ġ

    // ������ ���� ����
    [HideInInspector] public float MoveActionValue; // ������ ����Ű�� ���� �� ��ȯ�Ǵ� ���� ĳ��
    [HideInInspector] public float MoveSpeed; // �����̴� �ӵ�

    // ���� ���� ����
    float gravity = 17; // �߷°�
    float verticalSpeed; // �÷��̾��� ���� �ӵ�(�߷¿��� ����)
    float terminalSpeed = 20; // ���� �ӵ��� �Ѱ�

    // �뽬 ���� ����
    [HideInInspector] public float DashSpeed; // �뽬 �� �ӵ�

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

    // ���� ���� ����
    [HideInInspector] public bool IsParring;
    [HideInInspector] public bool IsSpiritParring;
    [HideInInspector] public bool IsGuarding;
    [HideInInspector] public bool IsAttackingParring;
    [HideInInspector] public bool CanPenetrate;
    Coroutine parryEndedTermRoutine; // �и� ���� �ð� �ڷ�ƾ �����ϴ� ����

    // ���� ���� ����
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

    // ���� �ӽ� ���� ����
    [HideInInspector] public AnimatorStateInfo StateInfo;
    [HideInInspector] public PlayerState CurrentPlayerState;
    [HideInInspector] public PlayerStateMachine PlayerStateMachine;

    // �ΰ����� ����
    [HideInInspector] public Animator PlayerAnimator;
    [HideInInspector] public PlayerStats PlayerStats;
    [HideInInspector] public ChaliceOfAtonement PlayerChaliceOfAtonement;
    [HideInInspector] public bool IsGrogging; // �ൿ �Ҵ� ���� ����, ���� �� ������ ���ε� �̰ɷ� Ȯ�� // ���� ��� : ª�� �ൿ �Ҵ�, �� �ൿ �Ҵ�, ����, �и�, ��ȥ �и�, ����
    [HideInInspector] public bool IsLookRight; // �������� ���� �ִ��� ����
    [HideInInspector] public bool IsDoSomething; // �ٸ� ������ �� �� ���� Ư�� �ൿ ���� �� ���. ���� ��� : ���� ��� ��
    [HideInInspector] public bool IsInCombat; // ���� ��(������ ȸ�� �Ұ�)���� ������ ��(������ ȸ��)���� ����
    [HideInInspector] public bool IsAttackSucceed; // ���� ��(������ ȸ�� �Ұ�)���� ������ ��(������ ȸ��)���� ����
    float lastInCombatTime;

    void Awake()
    {
        MoveSpeed = 5f;
        DashSpeed = 10f;
        IsLockOn = false;
        IsLookRight = true; // ��κ� ���� �� �������� ���鼭 �����Ǽ� true�� ����.

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

        if (DialogSystem.Instance.isdialogueCanvas) return; // ���̾�α� ������ Ű �Է� �� �ൿ ����(���� TimeScale�� �ٷ�� �Ϳ� ���� ����)
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

    void Gravity() // �߷� ����
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
    void HandleSpiritCleave3Combo() // ��ȥ ������ 3��° �޺� ����
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
    }
    void DashAndPenetratePressed() // �뽬 �� ���� ����
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
    void GuardPressed() // ���� ����
    {
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

        if (guardAction.WasReleasedThisFrame() && CurrentPlayerState == PlayerState.Guard) // ���� ������ �� Ű�� ���� ����Ʈ ���·� �̵�
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }
    void Attack1Pressed() // ���� ZŰ ����
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
    void Attack2Pressed() // ���� XŰ ����
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


    public void AttackMoving(float moveSpeed) // ���� �� ������ ���ݾ� �����̴� ���. ���� �� IsAttackingMoving�� true�� �Ǹ� �÷��̾ ���� �������� �����δ�.
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