using System.Collections.Generic;
using UnityEngine;

public class GuardCheck : MonoBehaviour
{
    [SerializeField] PlayerController player;
    BoxCollider guardCollider;
    List<GameObject> monstersAttackingPlayer = new(); // 중복 체크를 위한 리스트
    void Awake()
    {
        TryGetComponent(out guardCollider);
    }

    void Update()
    {
        OnGuardCollider();
    }

    void OnGuardCollider()
    {
        if (player.IsGuarding)
        {
            guardCollider.enabled = true;
        }
        else
        {
            guardCollider.enabled = false;
        }
    }

    //private void OnTriggerEnter(Collider other) // 공격하는 몬스터 내에 같은 대상은 2번 공격하지 않는 로직 필요
    //{
    //    if (player.IsGuarding && other.CompareTag("Normal"))
    //    {
    //        if (player.IsParring)
    //        {
    //            player.Animator.SetTrigger("DoParry");
    //            player.IsParring = false; // 패리에 성공하면 패리 판정 즉시 종료
    //        }
    //        else
    //        {
    //            player.Animator.SetTrigger("DoGuardHit");
    //        }
    //    }
    //}
}
