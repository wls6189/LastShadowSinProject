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
        SFX_PLAYER
    }

    //audio clip 담을 수 있는 배열
    [SerializeField] private AudioClip[] titleBgm;   // 타이틀 화면 음악
    [SerializeField] private AudioClip[] gameBgm;    // 게임 내 배경 음악

    // SFX 각 카테고리별로 여러 사운드 클립을 담는 배열
    [SerializeField] private AudioClip[] uiSfx;      // UI 관련 효과음
    [SerializeField] private AudioClip[] npcSfx;     // NPC 관련 효과음
    [SerializeField] private AudioClip[] playerSfx;  // 플레이어 관련 효과음


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

    //AudioManager.instance.PlaySFX(AudioManager.ESfx.SFX_UI, 2);  // 세 번째 UI 사운드 효과를 재생
    public void PlayBGM(EBgm bgmCategory, int index)
    {
        switch (bgmCategory)
        {
            case EBgm.BGM_TITLE:
                audioBgm.clip = titleBgm[index];
                break;
            case EBgm.BGM_GAME:
                audioBgm.clip = gameBgm[index];
                break;
            default:
                return;
        }
        audioBgm.Play();
    }

    public void StopBGM()
    {
        audioBgm.Stop();//bgm끄기
    }

    public void PlaySFX(ESfx sfxCategory, int index)
    {
        switch (sfxCategory)
        {
            case ESfx.SFX_UI:
                audioSfx.PlayOneShot(uiSfx[index]);
                break;
            case ESfx.SFX_NPC:
                audioSfx.PlayOneShot(npcSfx[index]);
                break;
            case ESfx.SFX_PLAYER:
                audioSfx.PlayOneShot(playerSfx[index]);
                break;

            default:
                return;
        }
    }
    public void StopSFX()
    {
        audioSfx.Stop();  // 효과음 끄기
    }
}