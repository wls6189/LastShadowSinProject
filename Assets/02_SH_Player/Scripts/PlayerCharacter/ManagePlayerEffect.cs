using UnityEngine;

public class ManagePlayerEffect : MonoBehaviour
{
    [SerializeField] GameObject immuneEffectPrefab;
    [SerializeField] GameObject spiritMarkForceFullEffectPrefab;
    [SerializeField] GameObject basicAttackTrailPrefab;
    [SerializeField] GameObject spiritAttackTrailPrefab;
    [SerializeField] GameObject sparkPrefab;
    [SerializeField] Transform sparkPos;
    [SerializeField] Material dashMaterial;
    [SerializeField] Material basicMaterial;
    [SerializeField] GameObject dashTrailPrefab;
    [SerializeField] GameObject characterMesh;

    PlayerController player;

    void Awake()
    {
        TryGetComponent(out player);

        player.CallWhenGuarding += OnSpark;
    }

    private void OnDisable()
    {
        player.CallWhenGuarding -= OnSpark;
    }

    void Update()
    {
        ManageDashEffect();
        ManageImmuneEffect();
        ManageSpiritMarkForceFullEffect();
        ManageAttackTrail();
    }

    void ManageImmuneEffect()
    {
        if (player.PlayerStats.IsImmune)
        {
            immuneEffectPrefab.SetActive(true);
        }
        else
        {
            immuneEffectPrefab.SetActive(false);
        }
    }
    void ManageSpiritMarkForceFullEffect()
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce)
        {
            spiritMarkForceFullEffectPrefab.SetActive(true);
        }
        else
        {
            spiritMarkForceFullEffectPrefab.SetActive(false);
        }
    }
    void ManageAttackTrail()
    {
        if (player.IsAttacking)
        {
            if (player.CurrentPlayerState == PlayerState.BasicHorizonSlash1 ||
                player.CurrentPlayerState == PlayerState.BasicHorizonSlash2 ||
                player.CurrentPlayerState == PlayerState.BasicVerticalSlash ||
                player.CurrentPlayerState == PlayerState.RetreatSlash)
            {
                basicAttackTrailPrefab.SetActive(true);
            }
            else if (player.CurrentPlayerState == PlayerState.SpiritCleave1 ||
                player.CurrentPlayerState == PlayerState.SpiritCleave2 ||
                player.CurrentPlayerState == PlayerState.SpiritCleave3 ||
                player.CurrentPlayerState == PlayerState.SpiritPiercing ||
                player.CurrentPlayerState == PlayerState.SpiritSwordDance ||
                player.CurrentPlayerState == PlayerState.SpiritNova)
            {
                spiritAttackTrailPrefab.SetActive(true);
            }
        }
        else
        {
            basicAttackTrailPrefab.SetActive(false);
            spiritAttackTrailPrefab.SetActive(false);
        }
    }
    void ManageDashEffect()
    {
        if (player.OnDashEffect)
        {
            player.PlayerCharacterController.excludeLayers = 1 << LayerMask.NameToLayer("Enemy");
            characterMesh.GetComponent<SkinnedMeshRenderer>().material = dashMaterial;
            dashTrailPrefab.SetActive(true);
        }
        else
        {
            player.PlayerCharacterController.excludeLayers = 0;
            characterMesh.GetComponent<SkinnedMeshRenderer>().material = basicMaterial;
            dashTrailPrefab.SetActive(false);
        }
    }
    void OnSpark()
    {
        Instantiate(sparkPrefab, sparkPos.position, Quaternion.identity);
    }
}
