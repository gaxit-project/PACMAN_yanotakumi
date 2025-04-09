using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCall : MonoBehaviour
{
    public static GameCall Instance { get; private set; }

    [SerializeField]
    private GameObject _Ready;
    private TextMeshProUGUI _Readytext;

    [SerializeField]
    private GameObject _Clear;
    private TextMeshProUGUI _Cleartext;

    [SerializeField]
    private GameObject _Over;
    private TextMeshProUGUI _Overtext;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
            return;
        }
        Instance = this;

        _Readytext = _Ready.GetComponent<TextMeshProUGUI>();
        _Cleartext = _Clear.GetComponent<TextMeshProUGUI>();
        _Overtext = _Over.GetComponent<TextMeshProUGUI>();

        _Ready.SetActive(false);
        _Clear.SetActive(false);
        _Over.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameReady()
    {
        _Ready.SetActive(true);
    }

    public void GameClear()
    {
        _Clear.SetActive(true);
    }
    public void GameOver()
    {
        _Over.SetActive(true);
    }

    public void ResetText()
    {
        _Ready.SetActive(false);
        _Clear.SetActive(false);
        _Over.SetActive(false);
    }
}
