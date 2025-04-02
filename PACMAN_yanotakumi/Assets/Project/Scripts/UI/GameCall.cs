using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCall : MonoBehaviour
{
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

    void GameReady()
    {
        _Ready.SetActive(true);
    }

    void GameClear()
    {
        _Clear.SetActive(true);
    }
    void GameOver()
    {
        _Over.SetActive(true);
    }
}
