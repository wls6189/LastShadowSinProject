using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;



public class Enemy : MonoBehaviour
{
    private Transform player;
    public float detectionRange = 10f; // 탐지 범위
    public float standoffRange = 5f; // 대치 범위
    public float attackRange = 2f; // 공격 범위
    public float hitbox = 2f; //피격범위
    public float attackCooldown; // 공격 쿨타임   
    public float guardDuration = 3f; // 가드 상태 유지 시간
    private float guardStartTime; // 가드 상태 시작 시간
    public float parryDuration = 3f;
    public float parryStartTime;
    public int successfulGuardsToParry = 3; // 패리 상태로 전환할 가드 성공 횟수
    private int guardSuccessCount = 0; // 가드 성공 횟수
    
    private Collider parryCollider; // 패리 콜라이더
   

    public bool isAttacking = false;
    public bool isGuarding = false;
    public AttackPattern[] attackPatterns; // 사용할 공격 패턴 배열
    public AttackPattern currentPattern;

    public GameObject attackTrail;
    public GameObject spiritattackTrail;

    private float lastAttackTime; // 마지막 공격 시간

    public NavMeshAgent navMeshAgent; // NavMeshAgent 컴포넌트
    private bool isPlayerOnNavMesh = true; // 플레이어가 NavMesh에 있는지 여부


    private Animator animator; // Animator 컴포넌트

    public float maxChaseDistance = 15f; // 적이 플레이어를 쫓아갈 최대 거리
    private Vector3 spawnPosition; // 초기 위치 저장


    private int currentPatternIndex = 0; // 현재 사용 중인 패턴 인덱스
    private int currentAttackIndex = 0;  // 현재 패턴 내 공격 인덱스
    public enum State { Idle, Chasing, Returning, Guard, Parry }
    public State currentState;
    private EnemyStats enemyStats;


    public TextMeshProUGUI indicatorText;
    public float indicatorDuration = 1f; // 인디케이터가 표시되는 시간
    private bool isIndicatorActive = false;


    //가드상태일때 물약먹으면 가드풀고 공격 구현해야함


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
                attackCollider.gameObject.SetActive(false); //공격콜라이더 비활성화
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
                    CheckForGuard(); // 가드 상태 진입 조건 확인
                }
   

                break;

            case State.Returning:

                navMeshAgent.SetDestination(spawnPosition);
                enemyStats.RecoverHealth(); // 체력 회복
                if (distanceFromSpawn < 0.5f) // 거의 도착했을 때
                {
                    ResetEnemy(); // 상태 초기화
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
                guardSuccessCount = 0; // 카운트 초기화
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


        yield return new WaitForSeconds(animationDuration); // 가드 애니메이션이 끝날 때까지 대기

        if (currentState == State.Guard || currentState == State.Parry)
        {
            currentState = State.Chasing; // 다시 추격 상태로 전환
            isGuarding = false; // 가드 종료
            parryCollider.gameObject.SetActive(false);
            // **이동 재개**
            navMeshAgent.isStopped = false;
            if (player != null)
            {
                navMeshAgent.SetDestination(player.position); // 플레이어를 다시 추적
            }

          
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
       
    }

    private void CheckForChase()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // 탐지 범위 안에 들어오면 추격 시작
        if (distance <= detectionRange)
        {
            currentState = State.Chasing;

            // 이동 활성화
            navMeshAgent.isStopped = false;
            // 목적지 초기화
            navMeshAgent.SetDestination(player.position);

            
        }
    }

    private void FollowPlayer()
    {
        if (navMeshAgent == null || navMeshAgent.isStopped) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 공격 범위에 들어오지 않은 경우 계속 추격
        if (distance > attackRange)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.ResetPath(); // 공격 범위 안에서는 이동 멈춤
        }
       
    }
    private void CheckForGuard()
    {
        if (currentState == State.Guard)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);

        // 공격 쿨타임이 끝나지 않았다면 가드를 하고, 쿨타임이 끝나면 패리로 전환
        if (distance <= hitbox && IsPlayerAttacking())
        {
            if (Time.time - lastAttackTime < attackCooldown)
            {
                // 공격 쿨타임이 끝나지 않았다면 가드
                currentState = State.Guard;
                guardStartTime = Time.time;
               
               
            }
            else
            {
                // 공격 쿨타임이 끝났으면 패리로 전환
                currentState = State.Parry;
                parryStartTime = Time.time;
                
                parryCollider.gameObject.SetActive(true); // 패리 콜라이더 활성화
              
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

        if (player != null && player.IsAttacking) // 플레이어가 공격 중일 때
        {
            if (currentState == State.Guard || currentState == State.Parry)
            {

                StartCoroutine(MoveBackwardAfterGuard());

            }
        }
    }


    private void TryAttack()
    {

        if (currentState != State.Chasing) return; // 추격 상태에서만 공격

        // 쿨다운 확인
        if (Time.time - lastAttackTime < attackCooldown) return;
        attackCooldown = Random.Range(3.5f, 4.5f);
        // 플레이어가 공격 범위 안에 있을 경우
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            isAttacking = true;
            navMeshAgent.isStopped = true;
            ExecuteAttack(); // 공격 실행
        }
    }

    private void ExecuteAttack()
    {
        if (attackPatterns.Length == 0) return;

        // 현재 패턴과 공격 가져오기
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
        // 공격 애니메이션 트리거 추가
       
        animator.SetTrigger(currentAttack.attackName);  // 예: "Heavy Strike" 또는 "Quick Parry" 등
        lastAttackTime = Time.time; // 현재 시간 저장
        float attackDuration = GetAnimationLength(currentAttack.attackName);
        StartCoroutine(ManageAttackCollider(currentAttack.attackName, attackDuration, currentAttack.attackType));
        StartCoroutine(MoveAfterAttack());



        // 공격 수행



        // 다음 공격으로 이동
        //currentAttackIndex = (currentAttackIndex + 1) % currentPattern.attacks.Length;//이건 순차적으로 공격
        currentAttackIndex = Random.Range(0, currentPattern.attacks.Length);//이건 랜덤

        // 패턴 변경 로직 (원하는 시점에)
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
                return clip.length; // 해당 애니메이션의 길이 반환
            }
        }
        return 1.0f; // 기본값 (애니메이션 길이를 찾지 못하면 1초로 설정)
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

                // AttackBase의 attackType 설정
                AttackBase attackBase = attackCollider.GetComponent<AttackBase>();
                if (attackBase != null)
                {
                    attackBase.currentAttackType = attackType;
                }

                yield return new WaitForSeconds(duration * 0.7f); // 공격 애니메이션 중간쯤에 비활성화

                attackCollider.gameObject.SetActive(false);
            }
        }
    }
    private IEnumerator MoveAfterAttack()
    {
        // 애니메이션이 진행 중인지 확인
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = currentStateInfo.length;

        // 애니메이션 진행 시간 동안 반복
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            // 애니메이션 진행 중에 살짝 앞으로 이동
            if (elapsedTime > 0.2f && elapsedTime < 0.8f) // 예: 애니메이션의 중간 부분에 살짝 앞으로 이동
            {
                Vector3 forwardMovement = transform.forward * 2f * Time.deltaTime; // 살짝 앞으로 이동
                transform.position += forwardMovement; // 이동 적용
            }

            yield return null; // 한 프레임 대기
        }
        if (attackTrail != null)
            attackTrail.SetActive(false);

        if (spiritattackTrail != null)
            spiritattackTrail.SetActive(false);
        // 애니메이션 끝난 후 이동 재개
        navMeshAgent.isStopped = false; // 이동 재개
        isAttacking = false;

    }
    private IEnumerator MoveBackwardAfterGuard()
    {
        // 가드 애니메이션 길이 확인
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = currentStateInfo.length;

       
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            // 애니메이션 진행 중에 뒤로 이동
            if (elapsedTime > 0.2f && elapsedTime < 0.8f) // 애니메이션의 중간 부분에 뒤로 이동
            {
                Vector3 backwardMovement = -transform.forward * 0.2f * Time.deltaTime; // 뒤로 이동
                transform.position += backwardMovement; // 이동 적용
            }

            yield return null; // 한 프레임 대기
        }

        // 애니메이션 끝난 후, 추격 상태로 전환
        if (currentState != State.Parry)  // 패리 상태가 아닐 때만 추격 상태로 전환
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

        // 인디케이터가 새로 활성화되었으므로, 상태 변경
        isIndicatorActive = true;
        // 공격 전조 텍스트 설정
        indicatorText.text = new string('!', attack.indicatorhLevel); // 느낌표 갯수
        indicatorText.color = attack.indicatorColor; // 전조 색상 설정



        // 일정 시간 후 인디케이터 숨기기
        StartCoroutine(HideAttackIndicatorAfterTime(indicatorDuration));
    }

    private IEnumerator HideAttackIndicatorAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        // 텍스트를 숨기기
        if (indicatorText != null)
        {
            indicatorText.text = ""; // 텍스트 비우기
            isIndicatorActive = false; // 인디케이터가 비활성화됨

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