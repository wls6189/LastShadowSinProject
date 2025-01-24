using System.Collections.Generic;
using UnityEngine;

public class AttackingParryCheck : MonoBehaviour
{
    [SerializeField] PlayerController player;
    Collider attackingParryCollider;
    List<GameObject> attackedMonstersByPlayer = new(); // 중복 체크를 위한 리스트

    void Awake()
    {
        TryGetComponent(out attackingParryCollider);
    }

    void Update()
    {
        OnThrustColllider();
    }

    void OnThrustColllider()
    {
        if (player.IsAttackingParring)
        {
            attackingParryCollider.enabled = true;
        }
        else
        {
            attackingParryCollider.enabled = false;
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
                //other.gameObject.transform.parent.gameObject.GetComponent<EnemyStats>().TakeDamage();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<EnemyStats>() != null)
            {
                attackedMonstersByPlayer.Add(other.gameObject);
                //other.gameObject.GetComponent<EnemyStats>().TakeDamage();
            }
        }
    }
}
