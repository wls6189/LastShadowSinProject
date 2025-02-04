using UnityEngine;

public class RampagingESM : EternalSpiritMark
{
    float increaseAmount = 0;
    float decreaseAmount = 0;
    bool isRampaging = false;
    bool isAttackPowerUp;
    public RampagingESM() : base(
        "�����ϴ� ������ ��ȥ����",
        "�� �� 20�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���� �ɷ� ��� �� �ִ� ü���� 80%�� �Ұ� ���ݷ��� 80% ����մϴ�." +
        "\r\n��ȥ���� �ɷ� ���� �� �Ҿ��� �ִ� ü���� ������ ȸ������ �ʽ��ϴ�. ���ݷµ� ������� ���ƿɴϴ�.",
        20,
        true,
        0
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
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
