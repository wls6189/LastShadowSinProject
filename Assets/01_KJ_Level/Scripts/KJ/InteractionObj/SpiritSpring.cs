using System.Linq;
using UnityEngine;

public class SpiritSpring : Interaction
{
    //처음 상호작용 시 데이터 저장 후 Destroy 했다가 다시 로드하면 비활성화. 

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
