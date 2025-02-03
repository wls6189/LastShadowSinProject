using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SpiritShardOfTheDevoted : MonoBehaviour
{
     public int fragmentID; // 각 영혼 파편의 고유 ID

    public bool isSave = false;


    public SceneFlow sceneflow;

    
    private void Start()
    {
        // 씬 로드 시 저장된 상태 복원

        if (DataManager.Instance.nowPlayer.SpiritShardOfTheDevotedID.Contains(fragmentID))
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
