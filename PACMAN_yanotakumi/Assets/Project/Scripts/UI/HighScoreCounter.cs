using TMPro;
using UnityEngine;

public class HighScoreCounter : MonoBehaviour
{
    TextMeshProUGUI _text;
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();        
    }
    private void OnEnable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += HandleOnHighScore;
        }
    }

    private void LateUpdate()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += HandleOnHighScore;
        }
    }
    void HandleOnHighScore()
    {
        _text.SetText(GameManager.Instance.Score.ToString());   
    }
}
