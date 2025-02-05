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

    public void PickUpItem(PlayerController player) // «√∑π¿ÃæÓ ¬ ø°º≠ π¸¿ß ≥ªø°º≠ ªÛ»£¿€øÎ Ω√ ¿Ã∏¶ »£√‚«œµµ∑œ «œ±‚
    {
        if (IsPickUped) return;
        // øµ»•¿Á ∫Œ∫–
        if (DropItemData.SpiritAsh > 0)
        {
            player.PlayerStats.SpiritAsh += DropItemData.SpiritAsh; // øµ»•¿Á »πµÊ
            InGameUIManager.Instance.GeneratePickUpItemText($"øµ»•¿Á {DropItemData.SpiritAsh}∞≥∏¶ »πµÊ«ﬂΩ¿¥œ¥Ÿ.");
        }

        // øµø¯¿« øµ»•≥´¿Œ ∫Œ∫–
        if (DropItemData.EternalSpiritMark != null) // µÂ∂¯ æ∆¿Ã≈€ø° øµø¯¿« øµ»•≥´¿Œ¿Ã ¿÷¿ª ∂ß
        {
            if (player.PlayerESMInventory.OwnedESM.Contains(DropItemData.EternalSpiritMark)) // ∫∏¿Ø«œ¡ˆ æ ¿∫ øµø¯¿« øµ»•≥´¿Œ¿∫ »πµÊ«œ∞Ì ¿ÃπÃ ¿÷¥Ÿ∏È øµ»•¿Á »πµÊ
            {
                player.PlayerStats.SpiritAsh += 2000;
                InGameUIManager.Instance.GeneratePickUpItemText("∫∏¿Ø ¡ﬂ¿Œ øµø¯¿« øµ»•≥´¿Œ¿ª »πµÊ«ÿº≠ ¥ÎΩ≈ øµ»•¿Á 2000∞≥∏¶ »πµÊ«ﬂΩ¿¥œ¥Ÿ.");
            }
            else
            {
                player.PlayerESMInventory.OwnedESM.Add(DropItemData.EternalSpiritMark);
                InGameUIManager.Instance.GeneratePickUpItemText($"{DropItemData.EternalSpiritMark.Name}¿ª »πµÊ«ﬂΩ¿¥œ¥Ÿ.");
            }
        }

        // øµ»•≥´¿Œ ∫Œ∫–
        if (DropItemData.SpiritMarks.Count > 0)
        {
            foreach (SpiritMark spiritMark in DropItemData.SpiritMarks)
            {
                if (player.PlayerMarkInventory.IsInventoryFull())
                {
                    InGameUIManager.Instance.GeneratePickUpItemText("øµ»•≥´¿Œ ¿Œ∫•≈‰∏Æ∞° ∞°µÊ √°Ω¿¥œ¥Ÿ."); 
                }
                else
                {
                    player.PlayerMarkInventory.OwnedSpiritMark.Add(spiritMark);
                    InGameUIManager.Instance.GeneratePickUpItemText($"{spiritMark.Name}¿ª »πµÊ«ﬂΩ¿¥œ¥Ÿ.");
                }
            }
        }

        SMAndESMUIManager.Instance.SetOwnedESMList(); // æ∆¿Ã≈€ »πµÊ »ƒ ESMList ¿Áº≥¡§
        SMAndESMUIManager.Instance.SetOwnedSMList(); // æ∆¿Ã≈€ »πµÊ »ƒ ESMList ¿Áº≥¡§
        IsPickUped = true;

        AudioManager.instance.Playsfx(AudioManager.Sfx.GettingItem);
        Destroy(gameObject, 0.2f);
    }
}
