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
   

    public bool isAttacking = false;
    public bool isGuarding = false;
    public AttackPattern[] attackPatterns; // ����� ���� ���� �迭
    public AttackPattern currentPattern;

    public GameObject attackTrail;
    public GameObject spiritattackTrail;

    private float lastAttackTime; // ������ ���� �ð�

    public NavMeshAgent navMeshAgent; // NavMeshAgent ������Ʈ
    private bool isPlayerOnNavMesh = true; // �÷��̾ NavMesh�� �ִ��� ����


    private Animator animator; // Animator ������Ʈ

    public float maxChaseDistance = 15f; // ���� �÷��̾ �Ѿư� �ִ� �Ÿ�
    private Vector3 spawnPosition; // �ʱ� ��ġ ����


    private int currentPatternIndex = 0; // ���� ��� ���� ���� �ε���
    private int currentAttackIndex = 0;  // ���� ���� �� ���� �ε���
    public enum State { Idle, Chasing, Returning, Guard, Parry }
    public State currentState;
    private EnemyStats enemyStats;


    public TextMeshProUGUI indicatorText;
    public float indicatorDuration = 1f; // �ε������Ͱ� ǥ�õǴ� �ð�
    private bool isIndicatorActive = false;


    //��������϶� ��������� ����Ǯ�� ���� �����ؾ���


    private void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        attackTrail.SetActive(false);
        spiritattackTrail.SetActive(false);
        parryCollider = transform.Find("Parry").GetComponent<Collider>();
        foreach (Transform attackObject in transform)
        {
            Collider attackCollider = attackObject.GetComponent<Collider>();
            if (attackCollider != null)
            {
                attackCollider.gameObject.SetActive(false); //�����ݶ��̴� ��Ȱ��ȭ
            }
        }
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

        spawnPosition = transform.position;
    }

    private void Update()
    {
        if (isAttacking || isGuarding || enemyStats.isDead || enemyStats.isGroggy)
        {
            return;
        }
        UpdateAnimation();

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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromSpawn = Vector3.Distance(transform.position, spawnPosition);

        
        if (distanceFromSpawn > maxChaseDistance)
        {
            ReturnToSpawn();
            return;
        }

        UpdatePlayerNavMeshStatus();

        switch (currentState)
        {
            case State.Idle:
                CheckForChase();
                if (currentState == State.Idle) return;
                

                
                break;

            case State.Chasing:
                if (isAttacking) return;
                navMeshAgent.isStopped = false;
                

                if (isPlayerOnNavMesh)
                {
                    FollowPlayer();
                    CheckForGuard(); // ���� ���� ���� ���� Ȯ��
                }
   

                break;

            case State.Returning:

                navMeshAgent.SetDestination(spawnPosition);
                enemyStats.RecoverHealth(); // ü�� ȸ��
                if (distanceFromSpawn < 0.5f) // ���� �������� ��
                {
                    ResetEnemy(); // ���� �ʱ�ȭ
                }
                Vector3 directionToSpawn = spawnPosition - transform.position;
                directionToSpawn.y = 0; 
                if (directionToSpawn != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToSpawn);
                    transform.rotation = targetRotation;
                }
                break;

            case State.Guard:

                if (!isGuarding) 
                {
                    isGuarding = true;
                    
                    navMeshAgent.isStopped = true;

                    StartCoroutine(SwitchToChasingAfterGuardAnimation());
                }
                break;

            case State.Parry:
                animator.SetTrigger("Parry");
                navMeshAgent.isStopped = true;
                guardSuccessCount = 0; // ī��Ʈ �ʱ�ȭ
                StartCoroutine(SwitchToChasingAfterGuardAnimation());

                break;
        }

        TryAttack();
    }


    private void UpdatePlayerNavMeshStatus()
    {
        NavMeshHit hit;
        isPlayerOnNavMesh = NavMesh.SamplePosition(player.position, out hit, 1.0f, NavMesh.AllAreas);

    }
    private IEnumerator SwitchToChasingAfterGuardAnimation()
    {
        
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = currentStateInfo.length;


        yield return new WaitForSeconds(animationDuration); // ���� �ִϸ��̼��� ���� ������ ���

        if (currentState == State.Guard || currentState == State.Parry)
        {
            currentState = State.Chasing; // �ٽ� �߰� ���·� ��ȯ
            isGuarding = false; // ���� ����
            parryCollider.gameObject.SetActive(false);
            // **�̵� �簳**
            navMeshAgent.isStopped = false;
            if (player != null)
            {
                navMeshAgent.SetDestination(player.position); // �÷��̾ �ٽ� ����
            }

          
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
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
        if (currentState == State.Guard)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);

        // ���� ��Ÿ���� ������ �ʾҴٸ� ���带 �ϰ�, ��Ÿ���� ������ �и��� ��ȯ
        if (distance <= hitbox && IsPlayerAttacking())
        {
            if (Time.time - lastAttackTime < attackCooldown)
            {
                // ���� ��Ÿ���� ������ �ʾҴٸ� ����
                currentState = State.Guard;
                guardStartTime = Time.time;
               
               
            }
            else
            {
                // ���� ��Ÿ���� �������� �и��� ��ȯ
                currentState = State.Parry;
                parryStartTime = Time.time;
                
                parryCollider.gameObject.SetActive(true); // �и� �ݶ��̴� Ȱ��ȭ
              
            }
        }
    }

    private bool IsPlayerAttacking()
    {
        return player != null && player.GetComponent<PlayerController>().IsAttackColliderEnabled;
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


        if (attackTrail != null)
            attackTrail.SetActive(true);

        if (currentAttack.attackType == AttackType.Spirit)
        {
            if (spiritattackTrail != null)
                spiritattackTrail.SetActive(true);
        }
        // ���� �ִϸ��̼� Ʈ���� �߰�
       
        animator.SetTrigger(currentAttack.attackName);  // ��: "Heavy Strike" �Ǵ� "Quick Parry" ��
        lastAttackTime = Time.time; // ���� �ð� ����
        float attackDuration = GetAnimationLength(currentAttack.attackName);
        StartCoroutine(ManageAttackCollider(currentAttack.attackName, attackDuration, currentAttack.attackType));
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
    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length; // �ش� �ִϸ��̼��� ���� ��ȯ
            }
        }
        return 1.0f; // �⺻�� (�ִϸ��̼� ���̸� ã�� ���ϸ� 1�ʷ� ����)
    }
    private IEnumerator ManageAttackCollider(string attackTypeName, float duration, AttackType attackType)
    {
        Transform attackObject = transform.Find(attackTypeName);
        if (attackObject != null)
        {
            Collider attackCollider = attackObject.GetComponent<Collider>();
            if (attackCollider != null)
            {
                yield return new WaitForSeconds(duration *0.2f);
                attackCollider.gameObject.SetActive(true);

                // AttackBase�� attackType ����
                AttackBase attackBase = attackCollider.GetComponent<AttackBase>();
                if (attackBase != null)
                {
                    attackBase.currentAttackType = attackType;
                }

                yield return new WaitForSeconds(duration * 0.7f); // ���� �ִϸ��̼� �߰��뿡 ��Ȱ��ȭ

                attackCollider.gameObject.SetActive(false);
            }
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
        if (attackTrail != null)
            attackTrail.SetActive(false);

        if (spiritattackTrail != null)
            spiritattackTrail.SetActive(false);
        // �ִϸ��̼� ���� �� �̵� �簳
        navMeshAgent.isStopped = false; // �̵� �簳
        isAttacking = false;

    }
    private IEnumerator MoveBackwardAfterGuard()
    {
        // ���� �ִϸ��̼� ���� Ȯ��
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = currentStateInfo.length;

       
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
    

    private void ReturnToSpawn()
    {
        
        currentState = State.Returning;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(spawnPosition);
        Vector3 directionToSpawn = spawnPosition - transform.position;
        directionToSpawn.y = 0; 
        if (directionToSpawn != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToSpawn);
            transform.rotation = targetRotation;
        }
    }
    private void ResetEnemy()
    {
        
        currentState = State.Idle;
        
     
    }
    private void UpdateAnimation()
    {
        float speed = navMeshAgent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

    }
}