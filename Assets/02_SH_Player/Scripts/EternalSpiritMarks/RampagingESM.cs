using UnityEngine;

public class RampagingESM : EternalSpiritMark
{
    public RampagingESM() : base(
        "광폭하는 영원의 영혼낙인",
        "매 초 20의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 현재 체력의 80%을 잃고 5초간 주변 적에게 공격력의 30%의 피해를 주는 충격파를 일으킵니다.",
        20,
        true,
        5f
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;

            player.StartCoroutine(player.FireRampagingESMProjectile());

            player.PlayerStats.CurrentHealth -= (player.PlayerStats.CurrentHealth * 0.8f);
        }
    }
}
