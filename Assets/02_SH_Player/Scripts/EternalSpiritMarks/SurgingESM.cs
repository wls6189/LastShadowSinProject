using UnityEngine;

public class SurgingESM : EternalSpiritMark
{
    public SurgingESM() : base(
        "솟구치는 영원의 영혼낙인",
        "모든 공격이 적중할 때마다 2의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 25%, 50%, 100%가 찼을 때 영혼낙인 능력을 사용할 수 있습니다. 영혼낙인 능력 사용 시 보고있는 방향으로 영력 방출을 시전합니다. 영혼낙인력이 높을 때 시전할수록 영력 방출 개수가 증가합니다.",
        2,
        false,
        0
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce)
            {
                player.PlayerStats.CurrentSpiritMarkForce -= 100;
                player.FireSpiritUnboundProjectile(100);
            }
            else if (player.PlayerStats.CurrentSpiritMarkForce >= 50)
            {
                player.PlayerStats.CurrentSpiritMarkForce -= 50;
                player.FireSpiritUnboundProjectile(50);
            }
            else if (player.PlayerStats.CurrentSpiritMarkForce >= 25)
            {
                player.PlayerStats.CurrentSpiritMarkForce -= 25;
                player.FireSpiritUnboundProjectile(25);
            }
        }
    }
}
