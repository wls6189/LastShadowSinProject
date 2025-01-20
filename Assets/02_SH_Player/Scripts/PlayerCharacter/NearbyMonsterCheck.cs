using System.Collections.Generic;
using UnityEngine;


public class NearbyMonsterCheck : MonoBehaviour
{
    public List<GameObject> Monsters = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monsters.Add(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !Monsters.Contains(other.gameObject))
        {
            Monsters.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monsters.Remove(other.gameObject);
        }
    }

    public bool IsMonsterExist()
    {
        if (Monsters.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Update()
    {
        if (IsMonsterExist())
        {
        }
    }
}
