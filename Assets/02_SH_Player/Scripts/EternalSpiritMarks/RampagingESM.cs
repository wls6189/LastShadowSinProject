using UnityEngine;

public class RampagingESM : EternalSpiritMark
{
    public RampagingESM() : base(
        "�����ϴ� ������ ��ȥ����",
        "�� �� 20�� ��ȥ���η��� ����ϴ�.",
        "��ȥ���η��� ���� á�� ��, ��ȥ���� �ɷ� ��� �� ���� ü���� 80%�� �Ұ� 5�ʰ� �ֺ� ������ ���ݷ��� 30%�� ���ظ� �ִ� ����ĸ� ����ŵ�ϴ�.",
        20,
        true,
        5f
        )
    { }

    public override void Ability(PlayerController player) // Update���� ȣ�� ��
    {
        if (player.PlayerStats.CurrentSpiritMarkForce >= player.PlayerStats.MaxSpiritMarkForce && player.spiritMarkAbilityAction.WasPressedThisFrame())
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;

            player.StartCoroutine(player.FireRampagingESMProjectile());

            player.PlayerStats.CurrentHealth -= (player.PlayerStats.CurrentHealth * 0.8f);
        }
    }
}
