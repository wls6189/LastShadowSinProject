using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField]
    private SceneFlow currentSceneData;




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
