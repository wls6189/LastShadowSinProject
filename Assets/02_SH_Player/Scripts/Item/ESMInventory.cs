using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ESMInventory : MonoBehaviour
{
    PlayerController player;

    [HideInInspector] public HashSet<EternalSpiritMark> OwnedESM = new();
    [HideInInspector] public EternalSpiritMark EquipedESM;
    void Awake()
    {
        // OwnedESM = JsonConvert // 세이브에서 가져오기
        // CurrentEquipedESM = JsonConvert // 세이브에서 가져오기

        TryGetComponent(out player);

        SwapESM(MarkAndESMList.ChooseESM());

        if (EquipedESM != null && !EquipedESM.IsNaturalRegeneration)
        {
            player.CallWhenDamaging += RegenerationGaugeOnAction;
        }
    }
    void OnDisable()
    {
        player.CallWhenDamaging -= RegenerationGaugeOnAction;
    }
    void Update()
    {
        ApplyESM();
    }
    public void SwapESM(EternalSpiritMark replacedESM)
    {
        EquipedESM = replacedESM;
        player.PlayerStats.CurrentSpiritMarkForce = 0; // 영원의 영혼낙인 교체 시 영혼낙인력 초기화

        if (!EquipedESM.IsNaturalRegeneration)
        {
            player.CallWhenDamaging += RegenerationGaugeOnAction;
        }
        else
        {
            player.CallWhenDamaging -= RegenerationGaugeOnAction;
        }
    }
    public void ApplyESM()
    {
        if (EquipedESM != null)
        {
            if (EquipedESM.IsNaturalRegeneration) // 자연 회복이라면 초당 회복
            {
                player.PlayerStats.CurrentSpiritMarkForce += (1 + player.PlayerStats.RegenerationSpiritWaveIncreasePercent) * EquipedESM.Gain * Time.deltaTime;
            }

            EquipedESM.Ability(player);
        }
        else
        {
            player.PlayerStats.CurrentSpiritMarkForce = 0;
        }
    }
    public bool IsEquipedESM()
    {
        return EquipedESM != null;
    }
    public void RegenerationGaugeOnAction()
    {
        player.PlayerStats.CurrentSpiritMarkForce += (1 + player.PlayerStats.RegenerationSpiritWaveIncreasePercent) * EquipedESM.Gain;

        if (EquipedESM.Equals(new RagingESM()) && player.PlayerStats.IsRagingOn)
        {
            player.PlayerStats.RagingStack += 1;
        }
    }
}
