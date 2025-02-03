using UnityEngine;

public class EnthusiasticESM : EternalSpiritMark
{
    public EnthusiasticESM() : base(
        "�������� ������ ��ȥ����",
        "��� ������ ������ ������ 5�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 8�ʰ� ���� ���� �� ��ȥ�� �ĵ��� 1�� ȸ���մϴ�.",
        5,
        false,
        8f
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsEnthusiastic = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerStats.IsEnthusiastic = false;
            RemainDuration = 0;
        }
    }
}
