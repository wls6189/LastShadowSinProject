using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>(); // 보유한 아이템 목록
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void AddItem(Item item) // 아이템 추가
    {
        items.Add(item);
       
    }
}