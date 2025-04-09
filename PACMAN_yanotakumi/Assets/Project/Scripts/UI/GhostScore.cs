using TMPro;
using UnityEngine;

public class GhostScore : MonoBehaviour
{
    public Transform targetTran;

    TextMeshProUGUI _text;
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.SetText("");
    }

    void Update()
    {
        transform.position = RectTransformUtility.WorldToScreenPoint(
             Camera.main,
             targetTran.position + Vector3.up);
    }

    public void SetScore(int score)
    {
        _text.SetText(score.ToString());
    }

    public void ResetScore()
    {
        _text.SetText("");
    }
}
