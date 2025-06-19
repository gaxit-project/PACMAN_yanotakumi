using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasManager : MonoBehaviour
{

    [SerializeField] CanvasGroup titleUI;
    [SerializeField] CanvasGroup settingUI;
    [SerializeField] CanvasGroup mainUI;
    void Update()
    {
        InitUI();
    }

    public void TitleSetting()
    {
        // メインメニューのUIを無効化
        titleUI.interactable = false;
        titleUI.blocksRaycasts = false;

        // 設定UIを有効化
        settingUI.interactable = true;
        settingUI.blocksRaycasts = true;

        // フォーカスを設定
        EventSystem.current.SetSelectedGameObject(GameObject.Find("SettingCanvas/Button"));
    }

    public void MainSetting()
    {
        // メインメニューのUIを無効化
        mainUI.interactable = false;
        mainUI.blocksRaycasts = false;

        // 設定UIを有効化
        settingUI.interactable = true;
        settingUI.blocksRaycasts = true;

        // フォーカスを設定
        EventSystem.current.SetSelectedGameObject(GameObject.Find("SettingCanvas/Button"));
    }

    public void BackTitle()
    {
        // 設定UIを無効化
        settingUI.interactable = false;
        settingUI.blocksRaycasts = false;

        // メインメニューのUIを有効化
        titleUI.interactable = true;
        titleUI.blocksRaycasts = true;

        // フォーカスを設定
        EventSystem.current.SetSelectedGameObject(GameObject.Find("TitleCanvas/Button"));
    }

    public void BackMain()
    {
        // 設定UIを無効化
        settingUI.interactable = false;
        settingUI.blocksRaycasts = false;

        // メインメニューのUIを有効化
        mainUI.interactable = true;
        mainUI.blocksRaycasts = true;

        // フォーカスを設定
        EventSystem.current.SetSelectedGameObject(GameObject.Find("MainCanvas/Pause/Button"));
    }

    public void InitUI()
    {
        if (titleUI == null)
        {
            titleUI = GameObject.Find("TitleCanvas").GetComponent<CanvasGroup>();
        }
        if (settingUI == null)
        {
            settingUI = GameObject.Find("SettingCanvas").GetComponent<CanvasGroup>();
        }
        if (mainUI == null)
        {
            mainUI = GameObject.Find("MainCanvas").GetComponent<CanvasGroup>();
        }
    }
    
}
