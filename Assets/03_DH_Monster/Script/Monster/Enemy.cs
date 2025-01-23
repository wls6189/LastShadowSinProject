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
    private Collider guardCollider; // 가드 콜라이더
    private Collider parryCollider; // 패리 콜라이더
    public Collider attackCollider;


    public float standoffMoveSpeed = 2f; // 왔다리갔다리 이동 속도
    public float standoffWanderRange = 5f; // 대치 상태에서 이동 범위
    private Vector3 standoffTargetPosition; // 대치 상태에서 이동할 목표 위치
    public float standoffDecisionInterval = 2f; // 대치 상태에서 행동을 결정하는 시간 간격
    private float lastStandoffDecisionTime; // 마지막으로 행동을 결정한 시간
    private bool isMaintainingStandoff; // 대치를 유지할지 여부
    private float lastDistanceToPlayer;

    private bool isAttacking = false;

    public AttackPattern[] attackPatterns; // 사용할 공격 패턴 배열
    public AttackPattern currentPattern;



    private float lastAttackTime; // 마지막 공격 시간

    private NavMeshAgent navMeshAgent; // NavMeshAgent 컴포넌트
    private bool isPlayerOnNavMesh = true; // 플레이어가 NavMesh에 있는지 여부

    private Animator animator; // Animator 컴포넌트


    private int currentPatternIndex = 0; // 현재 사용 중인 패턴 인덱스
    private int currentAttackIndex = 0;  // 현재 패턴 내 공격 인덱스
    public enum State { Idle, Chasing, Guard, Parry, Standoff }
    public State currentState;



    public TextMeshProUGUI indicatorText;
    public float indicatorDuration = 1f; // 인디케이터가 표시되는 시간
    private bool isIndicatorActive = false;


    //가드상태일때 물약먹으면 가드풀고 공격 구현해야함


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
                    CheckForGuard(); // 가드 상태 진입 조건 확인
                }
                FacePlayer();

                break;

            case State.Standoff: // 대치 상태 로직 추가
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
        yield return new WaitForSeconds(delay); // 지정된 시간 동안 대기

        if (currentState == State.Guard || currentState == State.Parry)
        {
            currentState = State.Chasing;
            guardCollider.gameObject.SetActive(false); // 가드 콜라이더 비활성화
            parryCollider.gameObject.SetActive(false);
            navMeshAgent.isStopped = false; // 이동 재개
            
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
            // 대치 상태로 진입
            if (distance > standoffRange)
            {
                currentState = State.Standoff;
                lastStandoffDecisionTime = Time.time; // 행동 결정 시간 초기화
                SetStandoffTarget();
            }
            else
            {
                // 추격 상태로 전환
                currentState = State.Chasing;
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(player.position);
            }
        }
    }
    private void HandleStandoff()
    {
        float currentDistanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 이전 거리와 현재 거리를 비교하여 방향 판단
        if (currentDistanceToPlayer > lastDistanceToPlayer)
        {
            // 플레이어로부터 멀어질 때 (백무빙)
            animator.SetTrigger("BackMove");
            Vector3 backwardDirection = (transform.position - player.position).normalized;
            navMeshAgent.Move(backwardDirection * standoffMoveSpeed * Time.deltaTime);
        }
        else
        {
            // 플레이어에게 가까워질 때 (런 애니메이션)
            animator.SetTrigger("Run");
            navMeshAgent.SetDestination(player.position);
        }

        lastDistanceToPlayer = currentDistanceToPlayer; // 현재 거리를 저장
    

        // 행동 결정 간격을 확인
        if (Time.time - lastStandoffDecisionTime > standoffDecisionInterval)
        {
            DecideStandoffAction(); // 랜덤 행동 결정
            lastStandoffDecisionTime = Time.time; // 마지막 결정 시간 갱신
        }

        if (isMaintainingStandoff)
        {
            // 대치를 유지하면서 왔다 갔다
            navMeshAgent.speed = standoffMoveSpeed;

            if (Vector3.Distance(transform.position, standoffTargetPosition) < 0.1f)
            {
                SetStandoffTarget(); // 새 목표 위치 설정
            }

            navMeshAgent.SetDestination(standoffTargetPosition);
        }
        else
        {
            // 공격하기 위해 플레이어에게 접근
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.position);

            // 플레이어와 거리가 가까워지면 공격
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                TryAttack();
            }
        }
    }
    private void DecideStandoffAction()
    {
        // 50% 확률로 대치 유지 또는 공격 결정
        if (Random.Range(0, 2) == 0)
        {
            isMaintainingStandoff = true; // 대치 유지
            Debug.Log("Enemy decided to maintain standoff.");
        }
        else
        {
            isMaintainingStandoff = false; // 공격 선택
            Debug.Log("Enemy decided to attack.");
        }
    }
    private void SetStandoffTarget()
    {
        // 플레이어 주변에서 랜덤 위치 선택
        Vector3 randomOffset = new Vector3(
            Random.Range(-standoffWanderRange, standoffWanderRange),
            0,
            Random.Range(-standoffWanderRange, standoffWanderRange)
        );
        standoffTargetPosition = player.position + randomOffset;
        standoffTargetPosition.y = transform.position.y; // 높이 고정
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
       
        float distance = Vector3.Distance(transform.position, player.position);

        // 플레이어가 공격하려고 할 때 가드 상태로 전환
        if (distance <= hitbox && IsPlayerAttacking())
        {
            // 가드 시도 시, 성공 횟수 체크
            guardSuccessCount++;

            if (guardSuccessCount < successfulGuardsToParry)
            {
                
                currentState = State.Guard;
                guardStartTime = Time.time; // 가드 시작 시간 기록
                guardCollider.gameObject.SetActive(true); // 가드 콜라이더 활성화
                Debug.Log($"Guard successful! Total: {guardSuccessCount}");
            }
            else
            {
                
                currentState = State.Parry;
                parryStartTime = Time.time;
                guardSuccessCount = 0;
                parryCollider.gameObject.SetActive(true); // 패리 콜라이더 활성화
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
        // 공격 애니메이션 트리거 추가
        animator.SetTrigger(currentAttack.attackName);  // 예: "Heavy Strike" 또는 "Quick Parry" 등
        lastAttackTime = Time.time; // 현재 시간 저장
        StartCoroutine(ResetNavMeshAgentAfterAnimation());



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
    private IEnumerator ResetNavMeshAgentAfterAnimation()
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

        // 애니메이션 끝난 후 이동 재개
        navMeshAgent.isStopped = false; // 이동 재개
        isAttacking = false;
    }
    private IEnumerator MoveBackwardAfterGuard()
    {
        // 가드 애니메이션 길이 확인
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = currentStateInfo.length;

        // 애니메이션 진행 시간 동안 반복
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
    public void EnableAttackCollider()
    {
        attackCollider.gameObject.SetActive(true); // 콜라이더 활성화
    }

    // 공격 애니메이션에서 콜라이더 비활성화
    public void DisableAttackCollider()
    {
        attackCollider.gameObject.SetActive(false); // 콜라이더 비활성화
    }



}









