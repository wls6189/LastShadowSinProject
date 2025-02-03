using UnityEngine;

public class RavenousESM : EternalSpiritMark
{
    public RavenousESM() : base(
        "굶주린 영원의 영혼낙인",
        "모든 공격이 적중할 때마다 8의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 6초 간 공격 적중 시 공격력만큼 체력을 회복합니다.",
        8,
        false,
        6f
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsRavenous = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerStats.IsRavenous = false;
            RemainDuration = 0;
        }
    }
}
