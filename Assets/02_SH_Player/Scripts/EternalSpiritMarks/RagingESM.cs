using UnityEngine;

public class RagingESM : EternalSpiritMark
{
    public RagingESM() : base(
        "몰아치는 영원의 영혼낙인",
        "모든 공격이 적중할 때마다 4의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 9초간 공격이 적중할 때마다 스택을 1 쌓습니다. 9초 후 공격력의 15%의 데미지를 주는 낙뢰를 스택만큼 시전합니다.",
        4,
        false,
        9f
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsRagingOn = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.StartCoroutine(player.FireRagingESMProjectile());
            player.PlayerStats.IsRagingOn = false;
            RemainDuration = 0;
        }
    }
}
