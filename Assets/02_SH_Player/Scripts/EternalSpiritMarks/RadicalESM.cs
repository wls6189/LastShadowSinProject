using UnityEngine;

public class RadicalESM : EternalSpiritMark
{
    public RadicalESM() : base(
        "극단적인 영원의 영혼낙인",
        "매 초 15의 영혼낙인력을 얻습니다.",
        "영혼낙인력이 가득 찼을 때, 영혼낙인 능력 사용 시 모든 영혼낙인력을 소모하고 공격자세 및 수비자세로 변환합니다." +
        "\r\n공격 자세 : 받는 피해가 50% 감소하지만 막기와 관련된 모든 행동이 불가능해집니다." +
        "\r\n수비 자세 : 패리 시 공격력의 20%만큼 데미지를 주지만 공격과 관련된 모든 행동이 불가능해집니다." +
        "\r\n또한 자세 변환 시 주변 적에게 공격력의 200%의 데미지를 줍니다.",
        15,
        true,
        0
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.FireRadicalESMProjectile();

            if (player.IsRadicalESMAttackPosture)
            {
                player.IsRadicalESMAttackPosture = false;
                player.PlayerStats.DamageReducePercentage = 0.5f;
            }
            else
            {
                player.IsRadicalESMAttackPosture = true;
                player.PlayerStats.DamageReducePercentage = 0;

                if (player.CurrentPlayerState == PlayerState.Guard)
                {
                    player.CurrentPlayerState = PlayerState.IdleAndMove;
                }
            }
        }
    }
}
