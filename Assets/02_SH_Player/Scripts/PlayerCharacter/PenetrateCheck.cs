using UnityEngine;

public class PenetrateCheck : MonoBehaviour
{
    PlayerController player;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<AttackBase>() != null && other.GetComponent<AttackBase>().currentAttackType == AttackType.Piercing) // �Ǿ�� ������ ���� ��.
        {
            if (player.CurrentPlayerState == PlayerState.Penetrate)
            {
                player.CanPenetrate = false;
            }
            else
            {
                player.CanPenetrate = true;
            }
        }
        else
        {
            player.CanPenetrate = false;
        }
    }
}
