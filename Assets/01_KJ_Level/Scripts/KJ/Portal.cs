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
            SceneManager.LoadScene(currentSceneData.nextScene); // ScriptableObject�� ����� ������ �̵�
        }
        else
        {
            SceneManager.LoadScene(currentSceneData.previousScene); // ScriptableObject�� ����� ������ �̵�
        }

    }
}
