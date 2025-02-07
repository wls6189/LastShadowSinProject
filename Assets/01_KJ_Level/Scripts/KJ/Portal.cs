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
            // ��ü ��
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
                StartCoroutine(UIManager.Instance.CurrentSceneUI("��ü ��"));
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestHavenMap.gameObject.SetActive(false);
                UIManager.Instance.CorpseCapeMap.gameObject.SetActive(false);
                break;
            case "LastVillage":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("���� �� ����"));
                UIManager.Instance.WestStartMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestVillageMapCursor.gameObject.SetActive(false);
                UIManager.Instance.WestHavenMap.gameObject.SetActive(false);
                UIManager.Instance.CorpseCapeMap.gameObject.SetActive(false);
                break;
            case "Village":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("����"));
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
            // ���ʾȽ�ó
            case "6-2":
            case "6-3":
            case "7-1":
            case "8-1":
            case "9-1":
            case "10-1":
                UIManager.Instance.InteractRadiantTorchWestStart(); //�� ui����ȭ
                break;
            case "6-1":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("���� �Ƚ�ó"));
                UIManager.Instance.InteractRadiantTorchWestStart(); //�� ui����ȭ
                break;
            case "DeapSouthernVillage":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("�������� ����"));
                UIManager.Instance.InteractRadiantTorchWestStart(); //�� ui����ȭ
                break;
            case "SouthernVillage":
                StartCoroutine(UIManager.Instance.CurrentSceneUI("���� ����"));
                UIManager.Instance.InteractRadiantTorchWestStart(); //�� ui����ȭ
                break;
            case "10-2_Boss":
                AudioManager.instance.Playsfx(AudioManager.Sfx.BossBGM);
                UIManager.Instance.InteractRadiantTorchWestStart(); //�� ui����ȭ
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
            SceneManager.LoadScene(currentSceneData.nextScene, LoadSceneMode.Single); // ScriptableObject�� ����� ������ �̵�
        }
        else
        {
            SceneManager.LoadScene(currentSceneData.previousScene, LoadSceneMode.Single); // ScriptableObject�� ����� ������ �̵�
        }

    }
}
