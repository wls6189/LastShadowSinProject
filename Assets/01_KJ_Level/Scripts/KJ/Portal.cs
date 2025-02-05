using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField]
    private SceneFlow currentSceneData;

    private void Start()
    {
        switch(currentSceneData.currentSceneName)
        {
            // 시체 곶
            case "1-1":
            case "1-2":
            case "1-3":
            case "2-1":
            case "2-2":
            case "2-3":
            case "3-1":
            case "3-2":
            case "4-1":
            case "5-1":
            case "5-2":
            case "5-2_Boss":
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                break;
            case "StartPlayScene":
            case "LastVillage":
            case "Village":
                StartCoroutine(UIManager.Instance.CurrentSceneUI(currentSceneData.currentSceneName));
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                break;
            // 서쪽안식처
            case "6-2":
            case "6-3":
            case "7-1":
            case "8-1":
            case "9-1":
            case "10-1":
            case "10-2_Boss":       
                UIManager.Instance.InteractRadiantTorchWestStart(); //맵 ui동기화
                break;
            case "6-1":
            case "DeapSouthernVillage":
            case "SouthernVillage":
                StartCoroutine(UIManager.Instance.CurrentSceneUI(currentSceneData.currentSceneName));
                UIManager.Instance.InteractRadiantTorchWestStart(); //맵 ui동기화

                break;
        }
      
    }


    public void LoadScene(bool isSceneMove)
    {
        
        

        if (isSceneMove)
        {
            SceneManager.LoadScene(currentSceneData.nextScene, LoadSceneMode.Single); // ScriptableObject에 저장된 씬으로 이동
        }
        else
        {
            SceneManager.LoadScene(currentSceneData.previousScene, LoadSceneMode.Single); // ScriptableObject에 저장된 씬으로 이동
        }

    }
}
