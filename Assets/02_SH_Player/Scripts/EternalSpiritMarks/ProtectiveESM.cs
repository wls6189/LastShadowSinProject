using UnityEngine;

public class ProtectiveESM : EternalSpiritMark
{
    float immuneDuration = 0;
    public ProtectiveESM() : base(
        "수호하는 영원의 영혼낙인",
        "매 초 5의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 3초 간 모든 공격에 피격되지 않는 상태가 됩니다.",
        5,
        true
        ) { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsImmune = true;
            immuneDuration = 3f;
        }

        if (immuneDuration > 0)
        {
            immuneDuration -= Time.deltaTime;
        }

        if (immuneDuration < 0)
        {
            player.PlayerStats.IsImmune = false;
            immuneDuration = 0;
        }
    }
}
