using System.Collections.Generic;
using UnityEngine;

public class GuardCheck : MonoBehaviour
{
    PlayerController player;
    Collider guardCollider;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        TryGetComponent(out guardCollider);
    }

    void Update()
    {
        OnGuardCollider();
    }

    void OnGuardCollider()
    {
        if (player.IsGuarding || player.IsSpiritParring)
        {
            guardCollider.enabled = true;
        }
        else
        {
            guardCollider.enabled = false;
        }
    }
}
