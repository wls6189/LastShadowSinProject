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

        // 세이브에서 불러오기
        LoadSMData();

        CheckActivatedMark();
        ApplyAbilityToStats();
    }
    public void LoadSMData() // 문드러진 낙인 상호작용 시 저장하고 호출하도록
    {
        EquipedSpiritMark = new SpiritMark[1 + DataManager.Instance.nowPlayer.DecayedStampCount]; // 세이브에서 문드러진 도장과 상호작용한 횟수를 가져와서 추가하기
        // OwnedSpiritMark = JsonConvert // 세이브에서 가져오기
        // EquipedSpiritMark = JsonConvert // 세이브에서 가져오기
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
    public void SwapSpiritMark(int whichSpiritMark, SpiritMark spiritMark) // 장비 시 모든 스탯 상승 효과를 초기화
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
