using UnityEngine;

public class DestructiveESM : EternalSpiritMark
{
    float increaseAmount = 0;
    public DestructiveESM() : base(
        "파괴적인 영원의 영혼낙인",
        "모든 공격이 적중할 때마다 15의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 6초 간 공격력이 30% 상승합니다.",
        15,
        false,
        6f
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            increaseAmount = player.PlayerStats.AttackPower * 0.3f;
            player.PlayerStats.AttackPower += increaseAmount;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerStats.AttackPower -= increaseAmount;
            RemainDuration = 0;
        }
    }
}