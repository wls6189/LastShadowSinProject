using System.Text.RegularExpressions;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField]
    string itemName; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteraction player = other.GetComponent<PlayerInteraction>();
            player.CollectItem(GetCleanName(this.gameObject.name));
            Destroy(this.gameObject);
        }
    }

    private string GetCleanName(string originalName)
    {
        // \s* -> ������ ������ 0�� �̻��� ���� ���� ����
        //\(.*\) -> ��ȣ �ȿ� �ִ� ��� ���� ����.

        return Regex.Replace(originalName, @"\s*\(.*\)", "");
    }
}
