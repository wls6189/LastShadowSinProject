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
        SFX_PLAYER
    }

    //audio clip ���� �� �ִ� �迭
    [SerializeField] private AudioClip[] titleBgm;   // Ÿ��Ʋ ȭ�� ����
    [SerializeField] private AudioClip[] gameBgm;    // ���� �� ��� ����

    // SFX �� ī�װ����� ���� ���� Ŭ���� ��� �迭
    [SerializeField] private AudioClip[] uiSfx;      // UI ���� ȿ����
    [SerializeField] private AudioClip[] npcSfx;     // NPC ���� ȿ����
    [SerializeField] private AudioClip[] playerSfx;  // �÷��̾� ���� ȿ����


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

    //AudioManager.instance.PlaySFX(AudioManager.ESfx.SFX_UI, 2);  // �� ��° UI ���� ȿ���� ���
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
        audioBgm.Stop();//bgm����
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
        audioSfx.Stop();  // ȿ���� ����
    }
}