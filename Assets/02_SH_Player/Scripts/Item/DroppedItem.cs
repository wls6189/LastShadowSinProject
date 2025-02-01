using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public DropItemData DropItemData;

    public void SetDroppedItemData(DropItemData dropItemData)
    {
        DropItemData = dropItemData;
    }

    public void PickUpItem(PlayerController player) // 플레이어 쪽에서 범위 내에서 상호작용 시 이를 호출하도록 하기
    {
        player.PlayerStats.SpiritAsh += DropItemData.SpiritAsh; // 영혼재 획득

        if (DropItemData.EternalSpiritMark != null) // 드랍 아이템에 영원의 영혼낙인이 있을 때
        {
            if (player.PlayerESMInventory.OwnedESM.Contains(DropItemData.EternalSpiritMark)) // 보유하지 않은 영원의 영혼낙인은 획득하고 이미 있다면 영혼재 획득
            {
                player.PlayerStats.SpiritAsh += 2000;
                Debug.Log("보유 중인 영원의 영혼낙인을 획득해서 대신 영혼재 2000을 획득합니다."); // 이후 인터페이스에서도 보이게끔 처리
            }
            else
            {
                player.PlayerESMInventory.OwnedESM.Add(DropItemData.EternalSpiritMark);
            }
        }

        if (DropItemData.SpiritMarks.Count > 0)
        {
            foreach (SpiritMark spiritMark in DropItemData.SpiritMarks)
            {
                if (player.PlayerMarkInventory.IsInventoryFull())
                {
                    Debug.Log("영혼낙인 인벤토리가 가득 찼습니다."); // 이후 인터페이스에서도 보이게끔 처리
                }
                else
                {
                    player.PlayerMarkInventory.OwnedSpiritMark.Add(spiritMark);
                }
            }
        }
    }
}
