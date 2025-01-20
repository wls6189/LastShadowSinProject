using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    [SerializeField] PlayerController player;
    CapsuleCollider attackCollider;
    List<GameObject> attackedMonstersByPlayer = new(); // 중복 체크를 위한 리스트

    void Awake()
    {
        TryGetComponent(out attackCollider);
    }

    void Update()
    {
        OnAttackColllider();
    }

    void OnAttackColllider()
    {
        if (player.IsAttackColliderEnabled && player.CurrentPlayerState != PlayerState.Thrust)
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
        }
    }
}
