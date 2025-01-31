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
    
    private Collider parryCollider; // �и� �ݶ��̴�
    public Collider attackCollider;

    public bool isAttacking = false;

    public AttackPattern[] attackPatterns; // ����� ���� ���� �迭
    public AttackPattern currentPattern;



    private float lastAttackTime; // ������ ���� �ð�

    private NavMeshAgent navMeshAgent; // NavMeshAgent ������Ʈ
    private bool isPlayerOnNavMesh = true; // �÷��̾ NavMesh�� �ִ��� ����

    private Animator animator; // Animator ������Ʈ


    private int currentPatternIndex = 0; // ���� ��� ���� ���� �ε���
    private int currentAttackIndex = 0;  // ���� ���� �� ���� �ε���
    public enum State { Idle, Chasing, Guard, Parry }
    public State currentState;



    public TextMeshProUGUI indicatorText;
    public float indicatorDuration = 1f; // �ε������Ͱ� ǥ�õǴ� �ð�
    private bool isIndicatorActive = false;


    //��������϶� ��������� ����Ǯ�� ���� �����ؾ���


    private void Start()
    {

       
        parryCollider = transform.Find("Parry").GetComponent<Collider>();
        attackCollider.gameObject.SetActive(false);
        attackCooldown = 0f;
       
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

        Vector3 direction = player.position - transform.position;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation; 
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
   

                break;

            case State.Guard:
                animator.SetTrigger("Guard");
                navMeshAgent.isStopped = true;

                StartCoroutine(SwitchToChasingAfterDelay(0.1f));
                break;

            case State.Parry:
                animator.SetTrigger("Parry");
                navMeshAgent.isStopped = true;
                guardSuccessCount = 0; // ī��Ʈ �ʱ�ȭ
                StartCoroutine(SwitchToChasingAfterDelay(0.1f));

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
           
            parryCollider.gameObject.SetActive(false);
            navMeshAgent.isStopped = false; // �̵� �簳

        }
    }

    private void CheckForChase()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Ž�� ���� �ȿ� ������ �߰� ����
        if (distance <= detectionRange)
        {
            currentState = State.Chasing;

            // �̵� Ȱ��ȭ
            navMeshAgent.isStopped = false;
            // ������ �ʱ�ȭ
            navMeshAgent.SetDestination(player.position);

            Debug.Log("Player detected, starting chase.");
        }
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

        // ���� ��Ÿ���� ������ �ʾҴٸ� ���带 �ϰ�, ��Ÿ���� ������ �и��� ��ȯ
        if (distance <= hitbox && IsPlayerAttacking())
        {
            if (Time.time - lastAttackTime < attackCooldown)
            {
                // ���� ��Ÿ���� ������ �ʾҴٸ� ����
                currentState = State.Guard;
                guardStartTime = Time.time;
               
                Debug.Log("Guarding! Attack cooldown is still active.");
            }
            else
            {
                // ���� ��Ÿ���� �������� �и��� ��ȯ
                currentState = State.Parry;
                parryStartTime = Time.time;
                
                parryCollider.gameObject.SetActive(true); // �и� �ݶ��̴� Ȱ��ȭ
                Debug.Log("Switching to Parry State after attack cooldown.");
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
        attackCooldown = Random.Range(3.5f, 4.5f);
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
        StartCoroutine(MoveAfterAttack());



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
    private IEnumerator MoveAfterAttack()
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
        if (indicatorText == null)
        {

            return;
        }
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