using UnityEngine;

public class MonsterDrop : MonoBehaviour
{
    public GameObject DropItemPrefab;
    public int Ash;
    public float FaintProbability;
    public int MaxFaint;
    public float VividProbability;
    public int MaxVivid;
    public float AzureProbability;
    public int MaxAzure;
    public float ESMProbability;

    public void Drop() // ��� �� ���� �ʿ��� ȣ��
    {
        GameObject go = Instantiate(DropItemPrefab);
        go.transform.position = new Vector3(transform.position.x, go.transform.position.y, transform.position.z);
        go.GetComponent<DroppedItem>().SetDroppedItemData(GenerateItem.GeneratingItem(Ash, FaintProbability, MaxFaint, VividProbability, MaxVivid, AzureProbability, MaxAzure, ESMProbability));
    }
}
