using UnityEngine;

public class PenetrateCheck : MonoBehaviour
{
    [SerializeField] PlayerController player;


    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<AttackBase>() != null && other.GetComponent<AttackBase>().currentAttackType == AttackType.Piercing)
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
