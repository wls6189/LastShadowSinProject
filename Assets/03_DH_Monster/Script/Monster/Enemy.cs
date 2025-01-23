using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;



public class Enemy : MonoBehaviour
{
    private Transform player;
    public float detectionRange = 10f; // Ž�� ����
    public float standoffRange = 5f; // ��ġ ����
    public float attackRange = 2f; // ���� ����
    public float hitbox = 2f; //�ǰݹ���
    public float attackCooldown; // ���� ��Ÿ��   
    public float guardDuration = 3f; // ���� ���� ���� �ð�
    private float guardStartTime; // ���� ���� ���� �ð�
    public float parryDuration = 3f;
    public float parryStartTime;
    public int successfulGuardsToParry = 3; // �и� ���·� ��ȯ�� ���� ���� Ƚ��
    private int guardSuccessCount = 0; // ���� ���� Ƚ��
    private Collider guardCollider; // ���� �ݶ��̴�
    private Collider parryCollider; // �и� �ݶ��̴�
    public Collider attackCollider;


    public float standoffMoveSpeed = 2f; // �Դٸ����ٸ� �̵� �ӵ�
    public float standoffWanderRange = 5f; // ��ġ ���¿��� �̵� ����
    private Vector3 standoffTargetPosition; // ��ġ ���¿��� �̵��� ��ǥ ��ġ
    public float standoffDecisionInterval = 2f; // ��ġ ���¿��� �ൿ�� �����ϴ� �ð� ����
    private float lastStandoffDecisionTime; // ���������� �ൿ�� ������ �ð�
    private bool isMaintainingStandoff; // ��ġ�� �������� ����
    private float lastDistanceToPlayer;

    private bool isAttacking = false;

    public AttackPattern[] attackPatterns; // ����� ���� ���� �迭
    public AttackPattern currentPattern;



    private float lastAttackTime; // ������ ���� �ð�

    private NavMeshAgent navMeshAgent; // NavMeshAgent ������Ʈ
    private bool isPlayerOnNavMesh = true; // �÷��̾ NavMesh�� �ִ��� ����

    private Animator animator; // Animator ������Ʈ


    private int currentPatternIndex = 0; // ���� ��� ���� ���� �ε���
    private int currentAttackIndex = 0;  // ���� ���� �� ���� �ε���
    public enum State { Idle, Chasing, Guard, Parry, Standoff }
    public State currentState;



    public TextMeshProUGUI indicatorText;
    public float indicatorDuration = 1f; // �ε������Ͱ� ǥ�õǴ� �ð�
    private bool isIndicatorActive = false;


    //��������϶� ��������� ����Ǯ�� ���� �����ؾ���


    private void Start()
    {

        guardCollider = transform.Find("Guard").GetComponent<Collider>();
        parryCollider = transform.Find("Parry").GetComponent<Collider>();
        
        attackCollider.gameObject.SetActive(false);
        guardCollider.gameObject.SetActive(false);
        parryCollider.gameObject.SetActive(false);
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' not found.");
        }


    }

    private void Update()
    {
        if (transform.position.x != 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }


        if (player == null) return;


        UpdatePlayerNavMeshStatus();

        switch (currentState)
        {
            case State.Idle:
                animator.SetTrigger("Idle");

                CheckForChase();
                break;

            case State.Chasing:
                if (isAttacking) return;
                navMeshAgent.isStopped = false;
                animator.SetTrigger("Run");

                if (isPlayerOnNavMesh)
                {
                    FollowPlayer();
                    CheckForGuard(); // ���� ���� ���� ���� Ȯ��
                }
                FacePlayer();

                break;

            case State.Standoff: // ��ġ ���� ���� �߰�
                HandleStandoff();
                FacePlayer();
                CheckForGuard();
                break;

            case State.Guard:
                animator.SetTrigger("Guard");
                navMeshAgent.isStopped = true;

                StartCoroutine(SwitchToChasingAfterDelay(0.3f));
                break;

            case State.Parry:
                animator.SetTrigger("Parry");
                navMeshAgent.isStopped = true;
                
                StartCoroutine(SwitchToChasingAfterDelay(0.3f));

                break;
        }

        TryAttack();
    }


    private void UpdatePlayerNavMeshStatus()
    {
        NavMeshHit hit;
        isPlayerOnNavMesh = NavMesh.SamplePosition(player.position, out hit, 1.0f, NavMesh.AllAreas);

    }
    private IEnumerator SwitchToChasingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // ������ �ð� ���� ���

        if (currentState == State.Guard || currentState == State.Parry)
        {
            currentState = State.Chasing;
            guardCollider.gameObject.SetActive(false); // ���� �ݶ��̴� ��Ȱ��ȭ
            parryCollider.gameObject.SetActive(false);
            navMeshAgent.isStopped = false; // �̵� �簳
            
        }
    }
    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void CheckForChase()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            // ��ġ ���·� ����
            if (distance > standoffRange)
            {
                currentState = State.Standoff;
                lastStandoffDecisionTime = Time.time; // �ൿ ���� �ð� �ʱ�ȭ
                SetStandoffTarget();
            }
            else
            {
                // �߰� ���·� ��ȯ
                currentState = State.Chasing;
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(player.position);
            }
        }
    }
    private void HandleStandoff()
    {
        float currentDistanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ���� �Ÿ��� ���� �Ÿ��� ���Ͽ� ���� �Ǵ�
        if (currentDistanceToPlayer > lastDistanceToPlayer)
        {
            // �÷��̾�κ��� �־��� �� (�鹫��)
            animator.SetTrigger("BackMove");
            Vector3 backwardDirection = (transform.position - player.position).normalized;
            navMeshAgent.Move(backwardDirection * standoffMoveSpeed * Time.deltaTime);
        }
        else
        {
            // �÷��̾�� ������� �� (�� �ִϸ��̼�)
            animator.SetTrigger("Run");
            navMeshAgent.SetDestination(player.position);
        }

        lastDistanceToPlayer = currentDistanceToPlayer; // ���� �Ÿ��� ����
    

        // �ൿ ���� ������ Ȯ��
        if (Time.time - lastStandoffDecisionTime > standoffDecisionInterval)
        {
            DecideStandoffAction(); // ���� �ൿ ����
            lastStandoffDecisionTime = Time.time; // ������ ���� �ð� ����
        }

        if (isMaintainingStandoff)
        {
            // ��ġ�� �����ϸ鼭 �Դ� ����
            navMeshAgent.speed = standoffMoveSpeed;

            if (Vector3.Distance(transform.position, standoffTargetPosition) < 0.1f)
            {
                SetStandoffTarget(); // �� ��ǥ ��ġ ����
            }

            navMeshAgent.SetDestination(standoffTargetPosition);
        }
        else
        {
            // �����ϱ� ���� �÷��̾�� ����
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.position);

            // �÷��̾�� �Ÿ��� ��������� ����
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                TryAttack();
            }
        }
    }
    private void DecideStandoffAction()
    {
        // 50% Ȯ���� ��ġ ���� �Ǵ� ���� ����
        if (Random.Range(0, 2) == 0)
        {
            isMaintainingStandoff = true; // ��ġ ����
            Debug.Log("Enemy decided to maintain standoff.");
        }
        else
        {
            isMaintainingStandoff = false; // ���� ����
            Debug.Log("Enemy decided to attack.");
        }
    }
    private void SetStandoffTarget()
    {
        // �÷��̾� �ֺ����� ���� ��ġ ����
        Vector3 randomOffset = new Vector3(
            Random.Range(-standoffWanderRange, standoffWanderRange),
            0,
            Random.Range(-standoffWanderRange, standoffWanderRange)
        );
        standoffTargetPosition = player.position + randomOffset;
        standoffTargetPosition.y = transform.position.y; // ���� ����
    }

    private void FollowPlayer()
    {
        if (navMeshAgent == null || navMeshAgent.isStopped) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // ���� ������ ������ ���� ��� ��� �߰�
        if (distance > attackRange)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.ResetPath(); // ���� ���� �ȿ����� �̵� ����
        }
    }
    private void CheckForGuard()
    {
       
        float distance = Vector3.Distance(transform.position, player.position);

        // �÷��̾ �����Ϸ��� �� �� ���� ���·� ��ȯ
        if (distance <= hitbox && IsPlayerAttacking())
        {
            // ���� �õ� ��, ���� Ƚ�� üũ
            guardSuccessCount++;

            if (guardSuccessCount < successfulGuardsToParry)
            {
                
                currentState = State.Guard;
                guardStartTime = Time.time; // ���� ���� �ð� ���
                guardCollider.gameObject.SetActive(true); // ���� �ݶ��̴� Ȱ��ȭ
                Debug.Log($"Guard successful! Total: {guardSuccessCount}");
            }
            else
            {
                
                currentState = State.Parry;
                parryStartTime = Time.time;
                guardSuccessCount = 0;
                parryCollider.gameObject.SetActive(true); // �и� �ݶ��̴� Ȱ��ȭ
                Debug.Log("Switching to Parry State after 4th Guard attempt.");
            }
        }
    }

    private bool IsPlayerAttacking()
    {
        return player != null && player.GetComponent<PlayerController>().IsAttacking;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null && player.IsAttacking) // �÷��̾ ���� ���� ��
        {
            if (currentState == State.Guard || currentState == State.Parry)
            {

                StartCoroutine(MoveBackwardAfterGuard());

            }
        }
    }


    private void TryAttack()
    {

        if (currentState != State.Chasing) return; // �߰� ���¿����� ����

        // ��ٿ� Ȯ��
        if (Time.time - lastAttackTime < attackCooldown) return;

        // �÷��̾ ���� ���� �ȿ� ���� ���
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            isAttacking = true;
            navMeshAgent.isStopped = true;
            ExecuteAttack(); // ���� ����
        }
    }

    private void ExecuteAttack()
    {
        if (attackPatterns.Length == 0) return;

        // ���� ���ϰ� ���� ��������
        currentPattern = attackPatterns[currentPatternIndex];
        if (currentPattern.attacks.Length == 0) return;

        Attack currentAttack = currentPattern.attacks[currentAttackIndex];
        ShowAttackIndicator(currentAttack);
        // ���� �ִϸ��̼� Ʈ���� �߰�
        animator.SetTrigger(currentAttack.attackName);  // ��: "Heavy Strike" �Ǵ� "Quick Parry" ��
        lastAttackTime = Time.time; // ���� �ð� ����
        StartCoroutine(ResetNavMeshAgentAfterAnimation());



        // ���� ����



        // ���� �������� �̵�
        //currentAttackIndex = (currentAttackIndex + 1) % currentPattern.attacks.Length;//�̰� ���������� ����
        currentAttackIndex = Random.Range(0, currentPattern.attacks.Length);//�̰� ����

        // ���� ���� ���� (���ϴ� ������)
        if (currentAttackIndex == 0)
        {
            currentPatternIndex = (currentPatternIndex + 1) % attackPatterns.Length;
        }

    }
    private IEnumerator ResetNavMeshAgentAfterAnimation()
    {
        // �ִϸ��̼��� ���� ������ Ȯ��
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = currentStateInfo.length;

        // �ִϸ��̼� ���� �ð� ���� �ݺ�
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            // �ִϸ��̼� ���� �߿� ��¦ ������ �̵�
            if (elapsedTime > 0.2f && elapsedTime < 0.8f) // ��: �ִϸ��̼��� �߰� �κп� ��¦ ������ �̵�
            {
                Vector3 forwardMovement = transform.forward * 2f * Time.deltaTime; // ��¦ ������ �̵�
                transform.position += forwardMovement; // �̵� ����
            }

            yield return null; // �� ������ ���
        }

        // �ִϸ��̼� ���� �� �̵� �簳
        navMeshAgent.isStopped = false; // �̵� �簳
        isAttacking = false;
    }
    private IEnumerator MoveBackwardAfterGuard()
    {
        // ���� �ִϸ��̼� ���� Ȯ��
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = currentStateInfo.length;

        // �ִϸ��̼� ���� �ð� ���� �ݺ�
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            // �ִϸ��̼� ���� �߿� �ڷ� �̵�
            if (elapsedTime > 0.2f && elapsedTime < 0.8f) // �ִϸ��̼��� �߰� �κп� �ڷ� �̵�
            {
                Vector3 backwardMovement = -transform.forward * 0.2f * Time.deltaTime; // �ڷ� �̵�
                transform.position += backwardMovement; // �̵� ����
            }

            yield return null; // �� ������ ���
        }

        // �ִϸ��̼� ���� ��, �߰� ���·� ��ȯ
        if (currentState != State.Parry)  // �и� ���°� �ƴ� ���� �߰� ���·� ��ȯ
        {
            currentState = State.Chasing;
        }
    }
    private void ShowAttackIndicator(Attack attack)
    {

        if (isIndicatorActive) return;

        // �ε������Ͱ� ���� Ȱ��ȭ�Ǿ����Ƿ�, ���� ����
        isIndicatorActive = true;
        // ���� ���� �ؽ�Ʈ ����
        indicatorText.text = new string('!', attack.indicatorhLevel); // ����ǥ ����
        indicatorText.color = attack.indicatorColor; // ���� ���� ����



        // ���� �ð� �� �ε������� �����
        StartCoroutine(HideAttackIndicatorAfterTime(indicatorDuration));
    }

    private IEnumerator HideAttackIndicatorAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        // �ؽ�Ʈ�� �����
        if (indicatorText != null)
        {
            indicatorText.text = ""; // �ؽ�Ʈ ����
            isIndicatorActive = false; // �ε������Ͱ� ��Ȱ��ȭ��

        }
    }
    public void EnableAttackCollider()
    {
        attackCollider.gameObject.SetActive(true); // �ݶ��̴� Ȱ��ȭ
    }

    // ���� �ִϸ��̼ǿ��� �ݶ��̴� ��Ȱ��ȭ
    public void DisableAttackCollider()
    {
        attackCollider.gameObject.SetActive(false); // �ݶ��̴� ��Ȱ��ȭ
    }



}









