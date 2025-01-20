using System.Text.RegularExpressions;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField]
    string itemName; 

    private void Start()
    {
        itemName = GetCleanName(this.gameObject.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControllerTest player = other.GetComponent<PlayerControllerTest>();
            player.CollectItem(itemName);
            Destroy(this.gameObject); 
        }
    }

    private string GetCleanName(string originalName)
    {
        // \s* -> 공백을 포함한 0개 이상의 공백 문자 제거
        //\(.*\) -> 괄호 안에 있는 모든 문자 제거.

        return Regex.Replace(originalName, @"\s*\(.*\)", "");
    }
}
