using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Sfx { Hit1,Hit2, GameOver = 2, Playershot,EnemyShot,skeletonAwake,skeleton_die,punch,SkeletonWalk,Slash,Sting,hit_mino,Stomp, GameClear, revolver}

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    public float curBgVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;
    public float curSfxVolume;

    public Sfx sfx;





    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        Init();
    }

    void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObj = new GameObject("BgmPlayer");//배경음 담당 오브젝트 생성
        bgmObj.transform.parent = transform;
        bgmPlayer = bgmObj.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.volume = curBgVolume;
        bgmPlayer.clip = bgmClip;
        bgmPlayer.loop =true;


        //효과음 플레이어 초기화
        GameObject sfxObj = new GameObject("SfxPlayer");//배경음 담당 오브젝트 생성
        sfxObj.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObj.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = curSfxVolume;

        }
    }

    public void PlayBgm(bool isPlay)
    {
        if(isPlay)
        {
            Debug.Log("dsf");
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }


    public void VolumReset()
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i].volume = sfxVolume;

        }
        bgmPlayer.volume = bgmVolume;
    }

    public void PlaySfxs(Sfx sfx)
    {
        for(int i = 0; i  < sfxPlayers.Length;i++)
        {
            int looPchannelIndex = (i + channelIndex) % sfxPlayers.Length;
            if (sfxPlayers[looPchannelIndex].isPlaying)
            {
                continue;
            }
            
            channelIndex = looPchannelIndex;
            sfxPlayers[looPchannelIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[looPchannelIndex].Play();
            break;
        }
        
    }
    private static AudioManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
