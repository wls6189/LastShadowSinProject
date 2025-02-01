using System.Collections.Generic;
using UnityEngine;

public class MarkInventory : MonoBehaviour
{
    PlayerController player;

    [HideInInspector] public List<SpiritMark> OwnedSpiritMark = new();
    [HideInInspector] public SpiritMark[] EquipedSpiritMark;
    [HideInInspector] public Dictionary<Mark, int> ActivatedMark = new Dictionary<Mark, int>();

    void Awake()
    {
        // OwnedSpiritMark = JsonConvert // ���̺꿡�� ��������
        // EquipedSpiritMark = JsonConvert // ���̺꿡�� ��������
        int count = 3; // ���� ���̺꿡�� ���巯�� ����� ��ȣ�ۿ��� Ƚ���� �����ͼ� �߰��ϱ�

        EquipedSpiritMark = new SpiritMark[1 + count];

        TryGetComponent(out player);

        player.PlayerStats.InitializeStats();

        CheckActivatedMark();
        ApplyAbilityToStats();

        player.PlayerStats.CurrentHealth = player.PlayerStats.MaxHealth;
    }

    void CheckActivatedMark()
    {
        foreach (SpiritMark spiritMark in EquipedSpiritMark)
        {
            if (spiritMark != null)
            {
                foreach (Mark mark in spiritMark.Marks)
                {
                    if (ActivatedMark.ContainsKey(mark))
                    {
                        ActivatedMark[mark] += 1; // ������ �̹� �ִٸ� 1 �߰�
                    }
                    else
                    {
                        ActivatedMark[mark] = 1; // ������ ���ٸ� ���Ӱ� Ű ����
                    }
                }
            }
        }
    }
    void ApplyAbilityToStats()
    {
        foreach (Mark mark in ActivatedMark.Keys)
        {
            mark.Effect(player, ActivatedMark[mark]);
        }

        foreach (SpiritMark spiritMark in EquipedSpiritMark)
        {
            if (spiritMark != null)
            {
                player.PlayerStats.MaxHealth += spiritMark.Health;
                player.PlayerStats.AttackPower += spiritMark.AttackPower;
            }
        }
    }
    public void SwapSpiritMark(int whichSpiritMark, SpiritMark spiritMark) // ��� �� ��� ���� ��� ȿ���� �ʱ�ȭ
    {
        EquipedSpiritMark[whichSpiritMark] = spiritMark;

        player.PlayerStats.InitializeStats();

        CheckActivatedMark();
        ApplyAbilityToStats();
    }
    public bool IsInventoryFull()
    {
        if (OwnedSpiritMark.Count > 50)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
