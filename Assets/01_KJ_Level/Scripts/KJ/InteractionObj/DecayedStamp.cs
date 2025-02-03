using UnityEngine;

public class DecayedStamp : Interaction
{
    //처음 상호작용 시 데이터 저장 후 Destroy 했다가 다시 로드하면 비활성화. 

   

    [SerializeField]
    private string currentScenesDecayedStamp;
    void Start()
    {
        if(DataManager.Instance.nowPlayer.currentScenesDecayedStamp.Contains(currentScenesDecayedStamp))
        {
            if (DataManager.Instance.nowPlayer.DecayedStampCount > 0)
            {
                //Destroy(this.gameObject);
                this.gameObject.SetActive(false);
            }
        }
        

    }

    public override void InteractionPlayer()
    {
        
        DataManager.Instance.nowPlayer.DecayedStampCount += 1;
        DataManager.Instance.nowPlayer.currentScenesDecayedStamp.Add(currentScenesDecayedStamp);
        DataManager.Instance.SaveData();
        Destroy(this.gameObject);
    }
}
