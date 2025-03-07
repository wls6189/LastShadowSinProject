using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelect : MonoBehaviour
{

    public GameObject creat; //비어있는 슬롯을 눌렀을 때 뜨는 창 
    public TextMeshProUGUI[] slotText; //슬롯의 텍스트
    public TextMeshProUGUI newPlayerName;

    bool[] savefile = new bool[3]; //배열이기 때문에 초기화 해줘야 작동함.
    private void Start()
    {
        //슬롯별로 저장된 데이터가 존재하는지 판단.
        for(int i =0; i <3; i++) //슬롯이 현재 3까지 존재 하므로, 0,1,2 범위 사용
        {
            if (File.Exists(DataManager.Instance.path + $"{i}")) //즉, Save0~Save2 중에 파일이 존재한다면.ex) Save0 파일 존재
            {
                savefile[i] = true; //해당 Save0~Save2 중 존재한 파일을 true로. ex) Save0 파일을 true로 
                DataManager.Instance.nowSlot = i; //슬롯넘버 할당.  ex) Save0 파일의 넘버가 0이므로 0을 슬롯0번으로 할당.
                DataManager.Instance.LoadData(); //불러오기
                slotText[i].text = DataManager.Instance.nowPlayer.name; //해당 슬롯0번의 텍스트가 PlayerData의 name으로 저장
       
            }
            else
            {
                slotText[i].text = "비어있음";
            }
        }
        DataManager.Instance.DataClear();
    }
    public void Slot(int number) //버튼 클릭 이벤트 호출
    {
        DataManager.Instance.nowSlot = number; //호출했을 때 매개변수로 받은 숫자가 슬롯의 번호가 되는 개념.

        
        if (savefile[number]) //1. 저장된 데이터가 있다면. 즉, 파일이 존재했다면 -> Start문 참고.
        {
            DataManager.Instance.LoadData(); //불러오기
            GoGame(); //
        }
        else //2.저장된 데이터가 없을 때
        {
            Creat();
        }
   
    }
    public void Creat() 
    {
        creat.gameObject.SetActive(true);
    }

    public void GoGame() //인 게임으로 들어가는 버튼을 통해 함수 호출.
    {
        //저장된 데이터가 없을 때 -> 빈 슬롯이였다면
        if (!savefile[DataManager.Instance.nowSlot]) //Slot 메서드에서 DataManager.Instance.nowSlot을 업데이트 해줬기 때문에 사용 가능.
        {
            DataManager.Instance.nowPlayer.name = newPlayerName.text;

            DataManager.Instance.SaveData(); // 데이터 저장
            SceneManager.LoadScene(1);
        }

        else
        {
            string sceneToLoad = DataManager.Instance.nowPlayer.currentScene;

            SceneManager.LoadScene(sceneToLoad); // 씬 로드


        }

    }
  

}
