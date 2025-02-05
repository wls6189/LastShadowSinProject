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
        if (other.GetComponent<Enemy>() != null && other.GetComponent<Enemy>().isPiercing) // 피어싱 공격을 날릴 때.
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
