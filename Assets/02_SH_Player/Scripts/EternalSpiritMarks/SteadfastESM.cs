using UnityEngine;

public class SteadfastESM : EternalSpiritMark
{
    public SteadfastESM() : base(
        "�ε��� ������ ��ȥ����",
        "�� �� 5�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 6�� �� ���⵵ �и��� ȿ���� �����ϴ�.",
        5,
        true,
        6f
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsSteadfast = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerStats.IsSteadfast = false;
            RemainDuration = 0;
        }
    }
}
