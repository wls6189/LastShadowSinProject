using UnityEngine;

public class ProtectiveESM : EternalSpiritMark
{
    public ProtectiveESM() : base(
        "��ȣ�ϴ� ������ ��ȥ����",
        "�� �� 5�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 3�� �� ��� ���ݿ� �ǰݵ��� �ʴ� ���°� �˴ϴ�.",
        5,
        true,
        3f
        ) { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsImmune = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.PlayerStats.IsImmune = false;
            RemainDuration = 0;
        }
    }
}
