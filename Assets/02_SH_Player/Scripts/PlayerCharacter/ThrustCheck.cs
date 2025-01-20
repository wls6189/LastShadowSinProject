using System.Collections.Generic;
using UnityEngine;

public class ThrustCheck : MonoBehaviour
{
    [SerializeField] PlayerController player;
    BoxCollider thrustCollider;
    List<GameObject> attackedMonstersByPlayer = new(); // 중복 체크를 위한 리스트

    void Awake()
    {
        TryGetComponent(out thrustCollider);
    }

    void Update()
    {
        OnThrustColllider();
    }

    void OnThrustColllider()
    {
        if (player.IsAttackColliderEnabled && player.CurrentPlayerState == PlayerState.Thrust)
        {
            thrustCollider.enabled = true;
        }
        else
        {
            thrustCollider.enabled = false;
        }
    }
}
