using UnityEngine;

public class ProtectiveESM : EternalSpiritMark
{
    float immuneDuration = 0;
    public ProtectiveESM() : base(
        "��ȣ�ϴ� ������ ��ȥ����",
        "�� �� 5�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 3�� �� ��� ���ݿ� �ǰݵ��� �ʴ� ���°� �˴ϴ�.",
        5,
        true
        ) { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsImmune = true;
            immuneDuration = 3f;
        }

        if (immuneDuration > 0)
        {
            immuneDuration -= Time.deltaTime;
        }

        if (immuneDuration < 0)
        {
            player.PlayerStats.IsImmune = false;
            immuneDuration = 0;
        }
    }
}
