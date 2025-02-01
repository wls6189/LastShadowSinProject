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
        // OwnedSpiritMark = JsonConvert // 세이브에서 가져오기
        // EquipedSpiritMark = JsonConvert // 세이브에서 가져오기
        int count = 3; // 추후 세이브에서 문드러진 도장과 상호작용한 횟수를 가져와서 추가하기

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
                        ActivatedMark[mark] += 1; // 낙인이 이미 있다면 1 추가
                    }
                    else
                    {
                        ActivatedMark[mark] = 1; // 낙인이 없다면 새롭게 키 생성
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
    public void SwapSpiritMark(int whichSpiritMark, SpiritMark spiritMark) // 장비 시 모든 스탯 상승 효과를 초기화
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
