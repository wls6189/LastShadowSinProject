using UnityEngine;

public class RadiantTorch : Interaction
{

    [SerializeField]
    private GameObject EffectObject;
    void Start()
    {
        EffectObject.gameObject.SetActive(false);
    }

    public override void InteractionPlayer()
    {
        EffectObject.gameObject.SetActive(true);

        UIManager.Instance.InteractRadiantTorch();
    }
}
