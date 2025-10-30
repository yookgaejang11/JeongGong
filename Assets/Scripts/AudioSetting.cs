using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    public static AudioSetting Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public List<AudioClip> bgms = new List<AudioClip>();

    [Header("UI Sliders (�ڵ� ���� ����)")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("SFX Clips")]
    public List<SFXData> sfxList = new List<SFXData>();
    private Dictionary<SFXType, AudioClip> sfxDict = new Dictionary<SFXType, AudioClip>();

    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �� ȣ��� �Լ� ���
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // SFX Dictionary ����
        foreach (var sfx in sfxList)
        {
            if (!sfxDict.ContainsKey(sfx.type))
                sfxDict.Add(sfx.type, sfx.clip);
        }

        // ����� ���� �ҷ�����
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyVolume();
        bgmSource.clip = bgms[0];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    private void ApplyVolume()
    {
        if (bgmSource != null) bgmSource.volume = bgmVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        if (bgmSource != null) bgmSource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        if (sfxSource != null) sfxSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void PlaySFX(SFXType type)
    {
        if (sfxDict.ContainsKey(type) && sfxDict[type] != null)
        {
            sfxSource.PlayOneShot(sfxDict[type], sfxVolume);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] '{type}' SFX�� ��ϵ��� �ʾҽ��ϴ�.");
        }
    }
    private void Start()
    {
        //TryFindSliders();
    }
    //���� �ٲ� ������ �ڵ����� �� �����̴� ã�� ����
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryFindSliders();
        Debug.Log("d");
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            bgmSource.Stop();
            bgmSource.clip = bgms[0];
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            bgmSource.Stop();
            bgmSource.clip = bgms[1];
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            bgmSource.Stop();
            bgmSource.clip = bgms[2];
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            bgmSource.Stop();
           
        }
    }
    private void Update()
    {

    }
    private void TryFindSliders()
    {
        Debug.Log("[AudioSetting] TryFindSliders �����");
        // Ȱ��/��Ȱ�� ������Ʈ �����ؼ� ���� Ž��
        var allSliders = GameObject.FindObjectsOfType<Slider>(true);
        Debug.Log($"[AudioSetting] �����̴� ��ü Ž��: {allSliders.Length}�� �߰�");

        foreach (var slider in allSliders)
        {
            Debug.Log($"[AudioSetting] �߰ߵ� �����̴�: {slider.name}, Tag: {slider.tag}");
            if (slider.CompareTag("Bgm_Slider"))
                bgmSlider = slider;
            else if (slider.CompareTag("SFX_Slider"))
                sfxSlider = slider;
        }

        if (bgmSlider != null)
        {
            Debug.Log("[AudioSetting] BGM �����̴� ���� �Ϸ�");
            bgmSlider.value = bgmVolume;
            bgmSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }
        else
        {
            Debug.LogWarning("[AudioSetting] BGM �����̴��� ã�� ���߽��ϴ�.");
        }

        if (sfxSlider != null)
        {
            Debug.Log("[AudioSetting] SFX �����̴� ���� �Ϸ�");
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        else
        {
            Debug.LogWarning("[AudioSetting] SFX �����̴��� ã�� ���߽��ϴ�.");
        }
    }
}

[System.Serializable]
public class SFXData
{
    public SFXType type;
    public AudioClip clip;
}

public enum SFXType
{
    Hit1, Hit2, GameOver = 2, Playershot, EnemyShot, skeletonAwake, skeleton_die, punch, SkeletonWalk, Slash, Sting, hit_mino, Stomp, GameClear, revolver,StageClear
}