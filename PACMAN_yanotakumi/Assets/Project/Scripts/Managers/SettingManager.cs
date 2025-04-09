using Unity.VisualScripting;
using UnityEngine;

public class SettingManager : MonoBehaviour
{

    bool isOnce;
    void Start()
    {
        isOnce = true;
    }

    void Update()
    {
        
    }

    public void OnBack()
    {
        if (isOnce)
        {
            SceneManager.Instance.BackSetting();
        }
    }
}
