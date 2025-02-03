using UnityEngine;

public class ProtectiveESM : EternalSpiritMark
{
    public ProtectiveESM() : base(
        "수호하는 영원의 영혼낙인",
        "매 초 5의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 3초 간 모든 공격에 피격되지 않는 상태가 됩니다.",
        5,
        true,
        3f
        ) { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsImmune = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerStats.IsImmune = false;
            RemainDuration = 0;
        }
    }
}
