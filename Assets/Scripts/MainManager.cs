using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{

    public GameObject playGuide;
    public GameObject Setting;

    public void Settings()
    {
        Setting.SetActive(true);
    }
    
    public void CloseSetting()
    {
        Setting.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void HowToPlay()
    {
        playGuide.SetActive(true);
    }

    public void Close()
    {
        playGuide.SetActive(false);
    }
}
