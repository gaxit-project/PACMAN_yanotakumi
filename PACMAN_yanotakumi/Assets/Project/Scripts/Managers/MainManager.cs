using Unity.VisualScripting;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    bool isOnce;
    public GameObject Pause;
    private void Awake()
    {
        isOnce = true;
        Pause.SetActive(false);
    }

    private void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            Pause.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void MainGame()
    {
        if (isOnce)
        {
            Time.timeScale = 1f;
            Pause.SetActive(false);
        }
    }

    public void OnSetting()
    {
        if (isOnce)
        {
            SceneManager.Instance.Setting();
        }
    }

    public void OnTitle()
    {
        if (isOnce)
        {
            Time.timeScale = 1f;
            Pause.SetActive(false);
            SceneManager.Instance.Title();
        }
    }
}
