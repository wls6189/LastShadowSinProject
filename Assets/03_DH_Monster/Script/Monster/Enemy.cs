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
    public float attackCooldown; // 공격 쿨타임   
    public float guardDuration = 3f; // 가드 상태 유지 시간
    private float guardStartTime; // 가드 상태 시작 시간
    public float parryDuration = 3f;
    public float parryStartTime;
    public int successfulGuardsToParry = 3; // 패리 상태로 전환할 가드 성공 횟수
    private int guardSuccessCount = 0; // 가드 성공 횟수
    private Collider guardCollider; // 가드 콜라이더
    private Collider parryCollider; // 패리 콜라이더
    private bool hasEngaged = false;// 처음 조우했는지 여부
    private bool isAttacking = false;

    public AttackPattern[] attackPatterns; // 사용할 공격 패턴 배열
    public AttackPattern currentPattern;



    private float lastAttackTime; // 마지막 공격 시간

    private NavMeshAgent navMeshAgent; // NavMeshAgent 컴포넌트
    private bool isPlayerOnNavMesh = true; // 플레이어가 NavMesh에 있는지 여부
    
    private Animator animator; // Animator 컴포넌트


    private int currentPatternIndex = 0; // 현재 사용 중인 패턴 인덱스
    private int currentAttackIndex = 0;  // 현재 패턴 내 공격 인덱스
    public enum State { Idle, Chasing, Guard, Parry }
    public State currentState;



    public TextMeshProUGUI indicatorText;
    public float indicatorDuration = 1f; // 인디케이터가 표시되는 시간
    private bool isIndicatorActive = false;


    //가드상태일때 물약먹으면 가드풀고 공격 구현해야함


    private void Start()
    {

        guardCollider = transform.Find("Guard").GetComponent<Collider>();
        parryCollider = transform.Find("Parry").GetComponent<Collider>();

        
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
                Vector3 direction = player.position - transform.position;
                direction.y = 0; // Y값을 0으로 설정하여 수평 회전만 하도록 합니다.

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = targetRotation; // 즉시 회전
                }

                break;

            case State.Guard:
                animator.SetTrigger("Guard");
                navMeshAgent.isStopped = true;


                    currentState = State.Chasing;  // 가드 상태 종료하고 추격 상태로 전환
                    guardCollider.gameObject.SetActive(false);  // 가드 콜라이더 비활성화
                    navMeshAgent.isStopped = false;  // 이동 재개
                    Debug.Log("Guard animation finished, switching to Chasing.");
                
                break;

            case State.Parry:
                animator.SetTrigger("Parry");
                navMeshAgent.isStopped = true;
                
                    currentState = State.Chasing;  // 패리 상태 종료하고 추격 상태로 전환
                    parryCollider.gameObject.SetActive(false);  // 패리 콜라이더 비활성화
                    navMeshAgent.isStopped = false;  // 이동 재개
                    Debug.Log("Parry animation finished, switching to Chasing.");
                
                break;
        }

        TryAttack();
    }


    private void UpdatePlayerNavMeshStatus()
    {
        NavMeshHit hit;
        isPlayerOnNavMesh = NavMesh.SamplePosition(player.position, out hit, 1.0f, NavMesh.AllAreas);

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

            Debug.Log("Player detected, starting chase.");
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("GuardHit"))
        {
            return; // GuardHit 애니메이션이 재생 중이라면 바로 리턴
        }
        float distance = Vector3.Distance(transform.position, player.position);

        // 대치 범위에 처음 들어오면 가드 상태로 전환
        if (!hasEngaged && currentState == State.Chasing && distance <= standoffRange && distance > attackRange)
        {
            currentState = State.Guard;
            guardStartTime = Time.time; // 가드 시작 시간 기록
            guardCollider.gameObject.SetActive(true); // 가드 콜라이더 활성화
            hasEngaged = true; // 처음 조우 상태 기록

            Debug.Log("Guard state triggered on first engagement.");
            return; // 가드 상태로 전환 후 더 이상 조건 확인하지 않음
        }

        // 플레이어가 공격하려고 할 때 가드 상태로 전환
        if (IsPlayerAttacking())
        {
            currentState = State.Guard;
            guardStartTime = Time.time; // 가드 시작 시간 기록
            guardCollider.gameObject.SetActive(true); // 가드 콜라이더 활성화
        }
    }

    //private bool IsPlayerAttacking()
    //{
    //    if (player == null) return false;

    //    // 탐지 범위 내에서 "PlayerAttack" 태그를 가진 콜라이더를 찾기
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);

    //    foreach (Collider collider in colliders)
    //    {
    //        // "PlayerAttack" 태그를 가진 활성화된 콜라이더가 있는지 확인
    //        if (collider.CompareTag("PlayerAttack") && collider.enabled)
    //        {
    //            return true; // 공격 콜라이더가 활성화된 경우
    //        }
    //    }

    //    return false; // 활성화된 공격 콜라이더가 없으면 false
    //}
    //나중에 위에꺼 밑에 이걸로 바꿔야함
    private bool IsPlayerAttacking()
    {
        return player != null && player.GetComponent<PlayerController>().IsAttacking;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null && player.IsAttacking) // 플레이어가 공격 중일 때
        {
            if (currentState == State.Guard)
            {
                guardSuccessCount++;
                Debug.Log($"Guard successful! Total: {guardSuccessCount}");

                if (guardSuccessCount >= successfulGuardsToParry)
                {
                    // 패리 상태로 전환
                    currentState = State.Parry;
                    parryStartTime = Time.time;
                    guardSuccessCount = 0; // 카운트 초기화
                    Debug.Log("Switching to Parry State.");
                    guardCollider.gameObject.SetActive(false); // 가드 콜라이더 비활성화
                    parryCollider.gameObject.SetActive(true); // 패리 콜라이더 활성화
                }
                else
                {
                    Debug.Log("Guarded successfully.");
                }
            }

            // 패리 상태에서 플레이어 공격 감지
            if (currentState == State.Parry)
            {

                // 패리 상태 종료
                currentState = State.Chasing;
                parryCollider.gameObject.SetActive(false); // 패리 콜라이더 비활성화
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




}









