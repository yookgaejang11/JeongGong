using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public PlayableDirector PlayableDirector;
    public CanvasGroup PlayerUI;
    public CanvasGroup bossUi;
    bool isPause;
    public GameObject pauseUi;
    private static UIManager instance;
    public TextMeshProUGUI scoreTxt;
    [Header("#GameOver")]
    public GameObject GameoverUi;
    public RectTransform board;
    public CanvasGroup homeButton;
    public CanvasGroup retryButton;
    public CanvasGroup bg;
    private Vector3 originalPos;
    public TextMeshProUGUI tmpText;
    [Header("#GameClear")]
    public GameObject GameClearUi;
    public RectTransform board1;
    public CanvasGroup homeButton1;
    public CanvasGroup NextButton;
    public CanvasGroup retryButton1;
    public CanvasGroup bg1;
    private Vector3 originalPos1;
    public TextMeshProUGUI tmpText1;
    [TextArea]
    public string fullText = "Game\nOver";
    public float typingSpeed = 0.05f;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        
        tmpText.text = "";
        originalPos = board.anchoredPosition;
        originalPos1 = board1.anchoredPosition;
        GameoverUi.SetActive(false);
    }


    private void Update()
    {
        scoreTxt.text = "SCORE\n" + GameManager.Instance.score.ToString();
        if(!isPause && Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = true;
            GameManager.Instance.isPause = true;
            pauseUi.SetActive(true);
            Time.timeScale = 0;
        }
        else if(isPause && Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = false;
            GameManager.Instance.isPause = false;
            pauseUi.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Resume()
    {
        isPause = false;
        GameManager.Instance.isPause = false;
        pauseUi.SetActive(false);
        Time.timeScale = 1;
    }
    public void HomeBtn()
    {
        SceneManager.LoadScene(0);
    }



    public void ActiveTimeLine()
    {
        PlayerUI.alpha = 0;
        bossUi.alpha = 0;
        StartCoroutine(PlayTimeLine());
    }


    IEnumerator PlayTimeLine()
    {
        PlayableDirector.Play();
        Debug.Log(PlayableDirector.duration);
        yield return new WaitForSeconds((float)PlayableDirector.duration);
        AudioManager.instance.PlayBgm(true);
        yield return new WaitForSeconds(0.1f);
        PlayerUI.alpha = 1;
        yield return new WaitForSeconds(0.1f);
        bossUi.DOFade(1,0.25f);
        GameManager.Instance.moveContainer();
        GameObject.Find("Mino").GetComponent<BossPattern>().isActive = true;
        GameObject.Find("Mino").GetComponent<BossPattern>().LoopPattern();
        GameObject.Find("TimeLine").gameObject.tag = "Untagged";
    }

    public void RetryBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ShowGameOver()
    {
        GameoverUi.SetActive(true);
        board.anchoredPosition = new Vector2(0, 1000); // 화면 위에서 시작
        homeButton.alpha = 0;
        retryButton.alpha = 0;
        bg.alpha = 0;

        bg.DOFade(1, 0.3f).OnComplete(() =>
        {
            board.DOAnchorPos(originalPos, 0.6f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                StartCoroutine(StartTyping());

            });
        });

    }
    public void ShowGameClear()
    {
        GameClearUi.SetActive(true);
        tmpText1.text = "";
        board1.anchoredPosition = new Vector2(0, 1000); // 화면 위에서 시작
        homeButton1.alpha = 0;
        NextButton.alpha = 0;
        retryButton1.alpha = 0;
        bg1.alpha = 0;

        bg1.DOFade(1, 0.3f).OnComplete(() =>
        {
            board1.DOAnchorPos(originalPos1, 0.6f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                StartCoroutine(StartTyping1());

            });
        });

    }


    IEnumerator StartTyping1()
    {
        
        StartCoroutine(TypeText1());
        yield return new WaitForSeconds(0.6f);
        homeButton1.DOFade(1, 0.3f);
        NextButton.DOFade(1, 0.3f);
        retryButton1.DOFade(1, 0.3f);

    }
    IEnumerator StartTyping()
    {
        StartCoroutine(TypeText());

        yield return new WaitForSeconds(0.6f);
        homeButton.DOFade(1, 0.3f);
        retryButton.DOFade(1, 0.3f);

    }

    private IEnumerator TypeText()
    {
        tmpText.text = ""; // 초기화

        foreach (char c in fullText)
        {
            tmpText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    } 
    private IEnumerator TypeText1()
    {
        foreach (char c in fullText)
        {
            tmpText1.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }


    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
