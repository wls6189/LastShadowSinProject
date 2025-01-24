using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    [SerializeField] PlayerController player;
    Collider attackCollider;
    HashSet<GameObject> attackedMonstersByPlayer = new(); // �ߺ� üũ�� ���� ����Ʈ

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
        if (player.IsAttackColliderEnabled)
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
            player.IsAttackSucceed = false;
            attackedMonstersByPlayer.Clear();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (attackedMonstersByPlayer.Contains(other.gameObject.transform.parent.gameObject)) return;

        if (other.CompareTag("EnemyGuard"))
        {
            if (other.gameObject.transform.parent.gameObject.GetComponent<EnemyStats>() != null)
            {
                attackedMonstersByPlayer.Add(other.gameObject.transform.parent.gameObject);
                Damaging(other.gameObject.transform.parent.gameObject.GetComponent<EnemyStats>(), false);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<EnemyStats>() != null)
            {
                attackedMonstersByPlayer.Add(other.gameObject);
                Damaging(other.gameObject.GetComponent<EnemyStats>(), true);
            }
        }


    }
    void Damaging(EnemyStats enemyStats, bool isDirectAttack)
    {
        player.IsAttackSucceed = true;

        switch (player.CurrentPlayerState)
        {
            case PlayerState.BasicHorizonSlash1:
                // ���� Damaged ó���Ǹ� �ۼ�
                break;
        }
    }
}
