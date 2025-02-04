using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //BGM 종류들
    public enum EBgm
    {
        BGM_TITLE,
        BGM_GAME,
    }

    //SFX 종류들
    public enum ESfx
    {
        SFX_UI,
        SFX_NPC,
        SFX_PLAYER,
        SFX_MONSTER
    }

    //audio clip 담을 수 있는 배열
    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] sfxs;

    //플레이하는 AudioSource
    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayBGM(EBgm bgmIdx)
    {
    
        audioBgm.clip = bgms[(int)bgmIdx];
        audioBgm.Play();
    }

  
    public void StopBGM()
    {
        audioBgm.Stop();
    }

    // ESfx 열거형을 매개변수로 받아 해당하는 효과음 클립을 재생
    public void PlaySFX(ESfx esfx)
    {
        audioSfx.PlayOneShot(sfxs[(int)esfx]);
    }
}