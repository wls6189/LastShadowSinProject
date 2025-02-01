using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SoulFragMent : MonoBehaviour
{
     public int fragmentID; // �� ��ȥ ������ ���� ID

    public bool isSave = false;


    public SceneFlow sceneflow;

    private void Start()
    {
        // �� �ε� �� ����� ���� ����

        if (DataManager.Instance.nowPlayer.saveObjectId.Contains(fragmentID))
        {
            isSave = true;
            GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]";
        }
        else
        {
            isSave = false;
            GetComponentInChildren<TextMeshProUGUI>().text = "Save [F]";
        }
    }
}
