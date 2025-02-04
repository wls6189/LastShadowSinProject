using UnityEngine;

public class RampagingESM : EternalSpiritMark
{
    float increaseAmount = 0;
    float decreaseAmount = 0;
    bool isRampaging = false;
    bool isAttackPowerUp;
    public RampagingESM() : base(
        "광폭하는 영원의 영혼낙인",
        "매 초 20의 영혼낙인력을 얻습니다.",
        "영혼낙인 능력 사용 시 최대 체력의 80%을 잃고 공격력이 80% 상승합니다." +
        "\r\n영혼낙인 능력 재사용 시 잃었던 최대 체력을 얻지만 회복되지 않습니다. 공격력도 원래대로 돌아옵니다.",
        20,
        true,
        0
        )
    { }

    public override void Ability(PlayerController player) // Update에서 호출 요
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;

            if (isRampaging)
            {
                isRampaging = false;

                player.PlayerStats.AttackPower -= increaseAmount;
                player.PlayerStats.MaxHealth += decreaseAmount;
            }
            else
            {
                isRampaging = true;

                increaseAmount = player.PlayerStats.AttackPower * 0.8f;
                decreaseAmount = player.PlayerStats.MaxHealth * 0.8f;
                player.PlayerStats.AttackPower += increaseAmount;
                player.PlayerStats.MaxHealth -= decreaseAmount;

                if (player.PlayerStats.CurrentHealth > player.PlayerStats.MaxHealth)
                {
                    player.PlayerStats.CurrentHealth = player.PlayerStats.MaxHealth;
                }
            }
        }
    }
}
