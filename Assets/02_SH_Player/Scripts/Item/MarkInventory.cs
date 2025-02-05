using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MarkInventory : MonoBehaviour
{
    PlayerController player;

    [HideInInspector] public List<SpiritMark> OwnedSpiritMark = new();
    [HideInInspector] public SpiritMark[] EquipedSpiritMark;
    [HideInInspector] public Dictionary<Mark, int> ActivatedMark = new Dictionary<Mark, int>();

    void Awake()
    {
        TryGetComponent(out player);

        // ���̺꿡�� �ҷ�����
        LoadSMData();

        CheckActivatedMark();
        ApplyAbilityToStats();
    }
    public void LoadSMData() // ���巯�� ���� ��ȣ�ۿ� �� �����ϰ� ȣ���ϵ���
    {
        EquipedSpiritMark = new SpiritMark[1 + DataManager.Instance.nowPlayer.DecayedStampCount]; // ���̺꿡�� ���巯�� ����� ��ȣ�ۿ��� Ƚ���� �����ͼ� �߰��ϱ�
        // OwnedSpiritMark = JsonConvert // ���̺꿡�� ��������
        // EquipedSpiritMark = JsonConvert // ���̺꿡�� ��������
    }
    void CheckActivatedMark()
    {
        ActivatedMark.Clear();

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
        player.PlayerStats.InitializeStats();

        foreach (Mark mark in ActivatedMark.Keys)
        {
            if (!ActivatedMark.ContainsKey(mark))
            {
                Debug.Log(mark.Name);
            }
            mark.Effect(player, ActivatedMark[mark]);
        }

        foreach (SpiritMark spiritMark in EquipedSpiritMark)
        {
            if (spiritMark != null)
            {
                player.PlayerStats.MaxHealthIncreaseAmount += spiritMark.Health;
                player.PlayerStats.AttackPowerIncreaseAmount += spiritMark.AttackPower;
            }
        }

        player.PlayerStats.CalculateStatsChange();
    }
    public void SwapSpiritMark(int whichSpiritMark, SpiritMark spiritMark) // ��� �� ��� ���� ��� ȿ���� �ʱ�ȭ
    {
        if (spiritMark == null)
        {
            EquipedSpiritMark[whichSpiritMark] = null;
        }
        else
        {
            EquipedSpiritMark[whichSpiritMark] = spiritMark;
        }

        CheckActivatedMark();
        ApplyAbilityToStats();

        SMAndESMUIManager.Instance.SetOwnedSMList();
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
