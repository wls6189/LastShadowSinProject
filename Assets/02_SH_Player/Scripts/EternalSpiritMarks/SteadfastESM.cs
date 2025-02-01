using UnityEngine;

public class SteadfastESM : EternalSpiritMark
{
    float parryDuration = 0;
    public SteadfastESM() : base(
        "부동의 영원의 영혼낙인",
        "매 초 5의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 5초 간 막기도 패리의 효과를 가집니다.",
        5,
        true
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsSteadfast = true;
            parryDuration = 5f;
        }

        if (parryDuration > 0)
        {
            parryDuration -= Time.deltaTime;
        }

        if (parryDuration < 0)
        {
            player.PlayerStats.IsSteadfast = false;
            parryDuration = 0;
        }
    }
}
