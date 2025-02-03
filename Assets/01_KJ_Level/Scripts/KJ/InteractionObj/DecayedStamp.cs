using UnityEngine;

public class DecayedStamp : Interaction
{
    //ó�� ��ȣ�ۿ� �� ������ ���� �� Destroy �ߴٰ� �ٽ� �ε��ϸ� ��Ȱ��ȭ. 

   

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
