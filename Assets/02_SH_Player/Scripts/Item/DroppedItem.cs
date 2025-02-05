using Unity.VisualScripting;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public DropItemData DropItemData;
    bool IsPickUped = false;

    public void SetDroppedItemData(DropItemData dropItemData)
    {
        DropItemData = dropItemData;
    }

    public void PickUpItem(PlayerController player) // 플레이어 쪽에서 범위 내에서 상호작용 시 이를 호출하도록 하기
    {
        if (IsPickUped) return;
        // 영혼재 부분
        if (DropItemData.SpiritAsh > 0)
        {
            player.PlayerStats.SpiritAsh += DropItemData.SpiritAsh; // 영혼재 획득
            InGameUIManager.Instance.GeneratePickUpItemText($"영혼재 {DropItemData.SpiritAsh}개를 획득했습니다.");
        }

        // 영원의 영혼낙인 부분
        if (DropItemData.EternalSpiritMark != null) // 드랍 아이템에 영원의 영혼낙인이 있을 때
        {
            if (player.PlayerESMInventory.OwnedESM.Contains(DropItemData.EternalSpiritMark)) // 보유하지 않은 영원의 영혼낙인은 획득하고 이미 있다면 영혼재 획득
            {
                player.PlayerStats.SpiritAsh += 2000;
                InGameUIManager.Instance.GeneratePickUpItemText("보유 중인 영원의 영혼낙인을 획득해서 대신 영혼재 2000개를 획득했습니다.");
            }
            else
            {
                player.PlayerESMInventory.OwnedESM.Add(DropItemData.EternalSpiritMark);
                InGameUIManager.Instance.GeneratePickUpItemText($"{DropItemData.EternalSpiritMark.Name}을 획득했습니다.");
            }
        }

        // 영혼낙인 부분
        if (DropItemData.SpiritMarks.Count > 0)
        {
            foreach (SpiritMark spiritMark in DropItemData.SpiritMarks)
            {
                if (player.PlayerMarkInventory.IsInventoryFull())
                {
                    InGameUIManager.Instance.GeneratePickUpItemText("영혼낙인 인벤토리가 가득 찼습니다."); 
                }
                else
                {
                    player.PlayerMarkInventory.OwnedSpiritMark.Add(spiritMark);
                    InGameUIManager.Instance.GeneratePickUpItemText($"{spiritMark.Name}을 획득했습니다.");
                }
            }
        }

        SMAndESMUIManager.Instance.SetOwnedESMList(); // 아이템 획득 후 ESMList 재설정
        SMAndESMUIManager.Instance.SetOwnedSMList(); // 아이템 획득 후 ESMList 재설정
        IsPickUped = true;
        Destroy(gameObject, 0.2f);
    }
}
