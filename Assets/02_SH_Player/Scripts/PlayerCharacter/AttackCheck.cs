using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    PlayerController player;
    Collider attackCollider;
    HashSet<GameObject> attackedMonstersByPlayer = new(); // �ߺ� üũ�� ���� ����Ʈ

    Vector3 bHS12Pos = new Vector3(0, 1, 1);
    Vector3 bHS12Scale = new Vector3(0.5f, 2, 1.7f);

    Vector3 RetreatPos = new Vector3(0, 1, 1.2f);
    Vector3 RetreatScale = new Vector3(0.5f, 2, 2.1f);

    Vector3 bVSSpiritCleaveSpiritSwordDancePos = new Vector3(0, 1, 1.4f);
    Vector3 bVSSpiritCleaveSpiritSwordDanceScale = new Vector3(0.5f, 2, 2.5f);

    Vector3 SpiritPiercingPos = new Vector3(0, 1, 1.6f);
    Vector3 SpiritPiercingScale = new Vector3(0.5f, 2, 2.9f);

    Vector3 thrustPos = new Vector3(0, 1, 0.35f);
    Vector3 thrustScale = new Vector3(0.5f, 2, 0.6f);

    Vector3 spiritSwordDancePos = new Vector3(0, 1, 1);
    Vector3 spiritSwordDanceScale = new Vector3(0.5f, 2, 1.7f);

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
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
            gameObject.tag = "Untagged";

            if (player.CurrentPlayerState == PlayerState.BasicHorizonSlash1 || player.CurrentPlayerState == PlayerState.BasicHorizonSlash2)
            {
                transform.localPosition = bHS12Pos;
                transform.localScale = bHS12Scale;
            }
            else if (player.CurrentPlayerState == PlayerState.RetreatSlash)
            {
                transform.localPosition = RetreatPos;
                transform.localScale = RetreatScale;
            }
            else if (player.CurrentPlayerState == PlayerState.SpiritPiercing)
            {
                transform.localPosition = SpiritPiercingPos;
                transform.localScale = SpiritPiercingScale;
            }
            else if (player.CurrentPlayerState == PlayerState.BasicVerticalSlash ||
            player.CurrentPlayerState == PlayerState.SpiritCleave1 ||
            player.CurrentPlayerState == PlayerState.SpiritCleave2 ||
            player.CurrentPlayerState == PlayerState.SpiritCleave3)
            {
                transform.localPosition = bVSSpiritCleaveSpiritSwordDancePos;
                transform.localScale = bVSSpiritCleaveSpiritSwordDanceScale;
            }

            attackCollider.enabled = true;
        }
        else if (player.IsAttackingParryColliderEnabled)
        {
            gameObject.tag = "PlayerGuard";

            if (player.CurrentPlayerState == PlayerState.Thrust)
            {
                transform.localPosition = thrustPos;
                transform.localScale = thrustScale;
            }
            else if (player.CurrentPlayerState == PlayerState.SpiritSwordDance)
            {
                transform.localPosition = bVSSpiritCleaveSpiritSwordDancePos;
                transform.localScale = bVSSpiritCleaveSpiritSwordDanceScale;
            }

            attackCollider.enabled = true;
        }
        else
        {
            gameObject.tag = "Untagged";

            attackCollider.enabled = false;
            player.IsAttackSucceed = false;
            attackedMonstersByPlayer.Clear();
        }
    }
    void Damaging(EnemyStats enemyStats, bool isDirectAttack)
    {
        player.CallWhenDamaging?.Invoke();
        player.IsAttackSucceed = true;

        switch (player.CurrentPlayerState)
        {
            case PlayerState.BasicHorizonSlash1:
                // ���� Damaged ó���Ǹ� �ۼ�
                break;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.transform.parent == null) return;
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
}
