using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //BGM ������
    public enum EBgm
    {
        BGM_TITLE,
        BGM_GAME,
    }

    //SFX ������
    public enum ESfx
    {
        SFX_UI,
        SFX_NPC,
        SFX_PLAYER,
        SFX_MONSTER
    }

    //audio clip ���� �� �ִ� �迭
    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] sfxs;

    //�÷����ϴ� AudioSource
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

    // ESfx �������� �Ű������� �޾� �ش��ϴ� ȿ���� Ŭ���� ���
    public void PlaySFX(ESfx esfx)
    {
        audioSfx.PlayOneShot(sfxs[(int)esfx]);
    }
}