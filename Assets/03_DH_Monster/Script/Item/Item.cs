using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;

    public virtual void ApplyEffect(Player player)
    {
        Debug.Log($"{itemName} 효과를 플레이어에 적용합니다.");
    }
}
