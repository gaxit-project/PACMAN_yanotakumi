using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Lifi : MonoBehaviour
{

    public static Lifi Instance { get; private set; }

    [Header("スライダー")]
    public Slider slider1;

    [SerializeField]
    private GameObject Image;
    [SerializeField]
    private GameObject LifeText;
    private TextMeshProUGUI lifeText;

    int Life;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
            return;
        }
        Instance = this;




        lifeText = LifeText.GetComponent<TextMeshProUGUI>();

        Image.SetActive(false);
        LifeText.SetActive(false);
    }
    void Start()
    {
        Life = GameManager.Instance.Lives;
    }



    private void LateUpdate()
    {
        Life = GameManager.Instance.Lives;


        if (Life <= 5)
        {
            Image.SetActive(false);
            LifeText.SetActive(false);
            slider1.value = 1f - (float)Life / 5f;
        }
        else
        {
            Image.SetActive(true);
            LifeText.SetActive(true);
            lifeText.text = "×　" + Life;
        }
    }
}
