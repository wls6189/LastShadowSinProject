using UnityEngine;

public class RagingESM : EternalSpiritMark
{
    public RagingESM() : base(
        "����ġ�� ������ ��ȥ����",
        "��� ������ ������ ������ 4�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ��� ��ȥ���η��� �Ҹ��ϰ� 9�ʰ� ������ ������ ������ ������ 1 �׽��ϴ�. 9�� �� ���ݷ��� 15%�� �������� �ִ� ���ڸ� ���ø�ŭ �����մϴ�.",
        4,
        false,
        9f
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
            player.PlayerStats.IsRagingOn = true;
            RemainDuration = Duration;
        }

        if (RemainDuration > 0)
        {
            RemainDuration -= Time.deltaTime;
        }

        if (RemainDuration < 0)
        {
            player.StartCoroutine(player.FireRagingESMProjectile());
            player.PlayerStats.IsRagingOn = false;
            RemainDuration = 0;
        }
    }
}
