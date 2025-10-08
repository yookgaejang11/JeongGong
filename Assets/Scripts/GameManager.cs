using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPause = false;
    public int score = 0;
    bool isTIme = false;
    public TextMeshProUGUI timer;
    public bool StageClear = false;                         //스테이지 클리어
    public bool StageOver = false;                          //스테이지 실패
    private static GameManager instance;
    public float overTime;                                  //오버타임
    public GameObject[] continers;

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
    }
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex != 3)
        {
            AudioManager.instance.PlayBgm(true);
        }
        else
        {
            AudioManager.instance.PlayBgm(false);
        }
        
    }
    
    public void moveContainer()
    {
        StartCoroutine(Removecontainer());
    }


    IEnumerator Removecontainer()
    {
        float timer = 0;
        while(timer <= 2)
        {
            continers[0].transform.Translate(-0.5f, 0, 0);
            continers[1].transform.Translate(-0.5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            timer += 0.01f;
        }
        Destroy(continers[0]);
        Destroy(continers[1]);
    }


    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Timer());


    }

    IEnumerator Timer()
    {
        
        if(!StageClear && !StageOver && !isTIme && !isPause)
        {
            isTIme = true;
            timer.text = "TIME\n" + overTime;
            overTime -=1;

            if( overTime <= 0)
            {
                overTime = 0;
                StageOver = true;
                TimeOver();
            }
            yield return new WaitForSeconds(1);
            isTIme=false;
        }
    }

    void TimeOver()
    {
        StageOver = true;
        overTime = 0;
        UIManager.Instance.fullText = "Time\nOver";
        UIManager.Instance.ShowGameOver();
    }

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            else
            {
                return instance;
            }
        }
    }


}
