using UnityEngine;

public class TitleManager : MonoBehaviour
{

    bool isOnce;
    void Start()
    {
        isOnce = true;
        AudioManager.Instance.PlayBGM(0);
    }

    void Update()
    {
        
    }

    public void OnStart()
    {
        if (isOnce)
        {
            SceneManager.Instance.MainGame();
        }
    }

    public void OnSetting()
    {
        if (isOnce)
        {
            SceneManager.Instance.Setting();
        }
    }

    public void OnExit()
    {
        if (isOnce)
        {
            SceneManager.Instance.Exit();
        }
    }
}
