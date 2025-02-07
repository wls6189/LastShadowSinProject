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
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestHavenMap.gameObject.SetActive(false);
                UIManager.Instance.CorpseCapeMap.gameObject.SetActive(false);
                break;
            case "StartPlayScene":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("시체 곶"));
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestHavenMap.gameObject.SetActive(false);
                UIManager.Instance.CorpseCapeMap.gameObject.SetActive(false);
                break;
            case "LastVillage":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("폐허 된 마을"));
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestHavenMap.gameObject.SetActive(false);
                UIManager.Instance.CorpseCapeMap.gameObject.SetActive(false);
                break;
            case "Village":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("마을"));
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestHavenMap.gameObject.SetActive(false);
                UIManager.Instance.CorpseCapeMap.gameObject.SetActive(false);
                break;
            case "5-2_Boss":
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestHavenMap.gameObject.SetActive(false);
                UIManager.Instance.CorpseCapeMap.gameObject.SetActive(false);
                AudioManager.instance.Playsfx(AudioManager.Sfx.BossBGM);
                break;
            // 서쪽안식처
            case "6-2":
            case "6-3":
            case "7-1":
            case "8-1":
            case "9-1":
            case "10-1":
                UIManager.Instance.InteractRadiantTorchWestStart(); //맵 ui동기화
                break;
            case "6-1":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("서쪽 안식처"));
                UIManager.Instance.InteractRadiantTorchWestStart(); //맵 ui동기화
                break;
            case "DeapSouthernVillage":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("깊은서쪽 마을"));
                UIManager.Instance.InteractRadiantTorchWestStart(); //맵 ui동기화
                break;
            case "SouthernVillage":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("서쪽 마을"));
                UIManager.Instance.InteractRadiantTorchWestStart(); //맵 ui동기화
                break;
            case "10-2_Boss":
                AudioManager.instance.Playsfx(AudioManager.Sfx.BossBGM);
                UIManager.Instance.InteractRadiantTorchWestStart(); //맵 ui동기화
                break;
        }
      
    }


    public void LoadScene(bool isSceneMove, PlayerStats playerStats)
    {

        DataManager.Instance.nowPlayer.CurrentHealth = playerStats.CurrentHealth;
        DataManager.Instance.nowPlayer.CurrentSpiritWave = playerStats.CurrentSpiritWave;
        DataManager.Instance.nowPlayer.CurrentSpiritMarkForce = playerStats.CurrentSpiritMarkForce;
        DataManager.Instance.SaveData();
        Debug.Log("?");
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
