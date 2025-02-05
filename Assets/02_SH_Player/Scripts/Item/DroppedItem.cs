using Unity.VisualScripting;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public DropItemData DropItemData;
    bool IsPickUped = false;

    public void SetDroppedItemData(DropItemData dropItemData)
    {
        DropItemData = dropItemData;
        if (dropItemData.SpiritAsh <= 0 && dropItemData.SpiritMarks.Count <= 0 && dropItemData.EternalSpiritMark == null)
        {
            Destroy(gameObject);
        }
    }

    public void PickUpItem(PlayerController player) // �÷��̾� �ʿ��� ���� ������ ��ȣ�ۿ� �� �̸� ȣ���ϵ��� �ϱ�
    {
        if (IsPickUped) return;
        // ��ȥ�� �κ�
        if (DropItemData.SpiritAsh > 0)
        {
            player.PlayerStats.SpiritAsh += DropItemData.SpiritAsh; // ��ȥ�� ȹ��
            InGameUIManager.Instance.GeneratePickUpItemText($"��ȥ�� {DropItemData.SpiritAsh}���� ȹ���߽��ϴ�.");
        }

        // ������ ��ȥ���� �κ�
        if (DropItemData.EternalSpiritMark != null) // ��� �����ۿ� ������ ��ȥ������ ���� ��
        {
            if (player.PlayerESMInventory.OwnedESM.Contains(DropItemData.EternalSpiritMark)) // �������� ���� ������ ��ȥ������ ȹ���ϰ� �̹� �ִٸ� ��ȥ�� ȹ��
            {
                player.PlayerStats.SpiritAsh += 2000;
                InGameUIManager.Instance.GeneratePickUpItemText("���� ���� ������ ��ȥ������ ȹ���ؼ� ��� ��ȥ�� 2000���� ȹ���߽��ϴ�.");
            }
            else
            {
                player.PlayerESMInventory.OwnedESM.Add(DropItemData.EternalSpiritMark);
                InGameUIManager.Instance.GeneratePickUpItemText($"{DropItemData.EternalSpiritMark.Name}�� ȹ���߽��ϴ�.");
            }
        }

        // ��ȥ���� �κ�
        if (DropItemData.SpiritMarks.Count > 0)
        {
            foreach (SpiritMark spiritMark in DropItemData.SpiritMarks)
            {
                if (player.PlayerMarkInventory.IsInventoryFull())
                {
                    InGameUIManager.Instance.GeneratePickUpItemText("��ȥ���� �κ��丮�� ���� á���ϴ�."); 
                }
                else
                {
                    player.PlayerMarkInventory.OwnedSpiritMark.Add(spiritMark);
                    InGameUIManager.Instance.GeneratePickUpItemText($"{spiritMark.Name}�� ȹ���߽��ϴ�.");
                }
            }
        }

        SMAndESMUIManager.Instance.SetOwnedESMList(); // ������ ȹ�� �� ESMList �缳��
        SMAndESMUIManager.Instance.SetOwnedSMList(); // ������ ȹ�� �� ESMList �缳��
        IsPickUped = true;

        AudioManager.instance.Playsfx(AudioManager.Sfx.GettingItem);
        Destroy(gameObject, 0.2f);
    }
}
