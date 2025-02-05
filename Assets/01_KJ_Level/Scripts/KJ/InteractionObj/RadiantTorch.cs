using UnityEngine;
using UnityEngine.SceneManagement;

public class RadiantTorch : Interaction
{

    [SerializeField]
    private GameObject EffectObject;
    void Start()
    {
        EffectObject.gameObject.SetActive(false);
    }

    public override void InteractionPlayer( )
    {
        EffectObject.gameObject.SetActive(true);

        string currentSceneName = SceneManager.GetActiveScene().name;

        switch(currentSceneName)
        {
            case "Village":
                UIManager.Instance.InteractRadiantTorchCCVillage();
                break;
            case "SouthernVillage":
                UIManager.Instance.InteractRadiantTorchWestVillage();
                break;
        }

        AudioManager.instance.Playsfx(AudioManager.Sfx.InteractAdventure);
    }
}
