using System.Linq;
using UnityEngine;

public class SpiritSpring : Interaction
{
    //ó�� ��ȣ�ۿ� �� ������ ���� �� Destroy �ߴٰ� �ٽ� �ε��ϸ� ��Ȱ��ȭ. 

    public string currentSceneSpiritSpring;

    [SerializeField]
    private GameObject EffectPrefab;

    void Start()
    {
       

        if (DataManager.Instance.nowPlayer.currentSceneSpiritSpring.Contains(currentSceneSpiritSpring))
        {
            if (DataManager.Instance.nowPlayer.SpiritSpringCount > 0)
            {
                //Destroy(this.gameObject);
               
                this.gameObject.SetActive(false);
            }
        }
    
       

    }

    public override void InteractionPlayer()
    {
        EffectPrefab.gameObject.SetActive(false);

        DataManager.Instance.nowPlayer.SpiritSpringCount += 1;
        DataManager.Instance.nowPlayer.currentSceneSpiritSpring.Add(currentSceneSpiritSpring);

        DataManager.Instance.SaveData();
       
    }
}
