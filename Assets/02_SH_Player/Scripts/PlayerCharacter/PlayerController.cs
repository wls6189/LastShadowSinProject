using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState // �÷��̾��� ���� �ൿ(Ȥ�� ����)
{
    IdleAndMove,
    Dash,
    Groggy,
    Guard,
    Penetrate,
    BasicHorizonSlash1,
    BasicHorizonSlash2,
    BasicVerticalSlash,
    Thrust,
    RetreatSlash,
    PowerfulThrust,
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
    float verticalSpeed; // �÷��̾��� ���� �ӵ�(�߷¿��� ����)
    float gravity = 17; // �߷�
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
    [HideInInspector] public bool CanBasicHorizonSlashCombo;
    [HideInInspector] public bool IsAttackColliderEnabled;
    float lastBasicHorizonSlash1AttackTime;
    int specialAttackCount;

    // ���� ���� ����
    [HideInInspector] public bool IsParring;
    [HideInInspector] public bool IsGuarding;

    // ���� ���� ����
    [HideInInspector] public CharacterController CharacterController;
    InputActionAsset inputActionAsset;
    InputAction gameMenuAction;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAndPenetrateAction;
    InputAction attack1Action;
    InputAction attack2Action;
    [HideInInspector] public InputAction attack3Action;
    [HideInInspector] public InputAction guardAction;
    InputAction lockOnAction;

    // �ΰ����� ����
    [HideInInspector] public Animator Animator;
    [HideInInspector] public AnimatorStateInfo StateInfo;
    [HideInInspector] public PlayerState CurrentPlayerState;
    [HideInInspector] public PlayerStateMachine PlayerStateMachine;
    [HideInInspector] public bool IsLookRight; // �������� ���� �ִ��� ����
    Coroutine parryEndedTermRoutine;

    void Awake()
    {
        MoveSpeed = 5f;
        DashSpeed = 10f;
        IsLockOn = false;

        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();
        inputActionAsset = GetComponent<PlayerInput>().actions;

        PlayerStateMachine = new PlayerStateMachine(this);
        PlayerStateMachine.Initialize(PlayerStateMachine.idleAndMoveState);

        gameMenuAction = inputActionAsset.FindAction("GameMenu");
        moveAction = inputActionAsset.FindAction("Move");
        jumpAction = inputActionAsset.FindAction("Jump");
        dashAndPenetrateAction = inputActionAsset.FindAction("DashAndPenetrate");
        attack1Action = inputActionAsset.FindAction("Attack1");
        attack2Action = inputActionAsset.FindAction("Attack2");
        attack3Action = inputActionAsset.FindAction("Attack3");
        guardAction = inputActionAsset.FindAction("Guard");
        lockOnAction = inputActionAsset.FindAction("LockOn");
    }

    void Update()
    {
        if (transform.position.x != 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }

        HandleBasicHorizonSlashCombo();

        StateInfo = Animator.GetCurrentAnimatorStateInfo(0);

        PlayerStateMachine.Execute();

        MoveActionValue = moveAction.ReadValue<float>();

        Gravity();


        if (DialogSystem.Instance.isdialogueCanvas) return; // ���̾�α� ������ �ൿ ����
        ManageRotate();

        GameMenuPressed();
        LockOnPressed();
        IdleAndMovePressed();
        DashPressed();
        GuardPressed();
        BasicHorizonSlashPressed();
        BasicVerticalSlashPressed();
        ThrustPressed();
    }

    void Gravity()
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
    void ManageRotate()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Groggy) return; // ���� ���̰ų� �뽬 ���̶�� ȸ�� ����

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

    void HandleBasicHorizonSlashCombo()
    {
        if (CanBasicHorizonSlashCombo)
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
            CanBasicHorizonSlashCombo = false;
            lastBasicHorizonSlash1AttackTime = 0;
        }
    }

    void GameMenuPressed()
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
    void IdleAndMovePressed() // ���̵� �� ������
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard || CurrentPlayerState == PlayerState.Groggy) return;

        if (moveAction.IsPressed() && CurrentPlayerState != PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }
    void DashPressed() // �뽬
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard || CurrentPlayerState == PlayerState.Groggy) return;

        if (dashAndPenetrateAction.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.dashState);
        }
    }
    void GuardPressed() // ����
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Groggy) return;

        if (guardAction.WasPressedThisFrame() && CurrentPlayerState == PlayerState.IdleAndMove) // IdleAndMove�� Guard�� ���� ���� ��ȯ
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

        if (guardAction.WasReleasedThisFrame() && CurrentPlayerState == PlayerState.Guard)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }
    void BasicHorizonSlashPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard || CurrentPlayerState == PlayerState.Groggy) return;

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
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard || CurrentPlayerState == PlayerState.Groggy) return;

        if (attack2Action.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicVerticalSlashState);
        }
    }
    void ThrustPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard || CurrentPlayerState == PlayerState.Groggy) return;

        if (attack3Action.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.thrustState);
        }
    }
    void RetreatSlashPressed()
    {

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