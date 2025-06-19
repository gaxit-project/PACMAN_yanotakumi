using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    public CanvasManager canvas;

    public int prevBuildIndex = -1; // 遷移前のシーンのBuildIndexを保存

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
            return;
        }
        Instance = this;
    }

    void Update()
    {
        Debug.Log("番号" + prevBuildIndex);
    }

    public string GetScene()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public void Title()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        canvas.InitUI();
    }

    public void MainGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
        canvas.InitUI();
    }

    public void Setting()
    {
        prevBuildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Setting", LoadSceneMode.Additive);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Setting");
        switch (prevBuildIndex)
        {
            case 0:
                canvas.InitUI();
                canvas.TitleSetting();
                break;
            case 1:

                break;
            case 2:
                canvas.InitUI();
                canvas.MainSetting();
                break;

            default:

                break;
        }
    }

    public void BackSetting()
    {
        //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Setting");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        switch (prevBuildIndex)
        {
            case 0:
                canvas.InitUI();
                canvas.BackTitle();
                UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
                break;
            case 1:

                break;
            case 2:
                canvas.InitUI();
                canvas.BackMain();
                UnityEngine.SceneManagement.SceneManager.LoadScene("main");
                break;

            default:
                
                break;
        }
        
    }

    public void Exit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // ゲーム終了
        #else
                    Application.Quit(); // ゲーム終了
        #endif
    }
}
