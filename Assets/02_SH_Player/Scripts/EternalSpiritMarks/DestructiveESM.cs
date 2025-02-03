using UnityEngine;

public class DestructiveESM : EternalSpiritMark
{
    float increaseAmount = 0;
    public DestructiveESM() : base(
        "�ı����� ������ ��ȥ����",
        "��� ������ ������ ������ 15�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 6�� �� ���ݷ��� 30% ����մϴ�.",
        15,
        false,
        6f
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
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