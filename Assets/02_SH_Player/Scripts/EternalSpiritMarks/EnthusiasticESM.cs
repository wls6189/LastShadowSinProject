using UnityEngine;

public class EnthusiasticESM : EternalSpiritMark
{
    public EnthusiasticESM() : base(
        "열성적인 영원의 영혼낙인",
        "모든 공격이 적중할 때마다 5의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 8초간 공격 적중 시 영혼의 파동을 1개 회복합니다.",
        5,
        false,
        8f
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsEnthusiastic = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerStats.IsEnthusiastic = false;
            RemainDuration = 0;
        }
    }
}
