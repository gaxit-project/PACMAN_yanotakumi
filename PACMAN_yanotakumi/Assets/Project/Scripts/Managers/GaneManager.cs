using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Ghost[] _ghostsArray;
    [SerializeField] Player _pacman;
    [SerializeField] Transform _pellets;
    [SerializeField] int _ghostMultiplier = 1;
    [SerializeField] float _powerModeDuration = 8f;

    Vector3 _pacmanInitialPos;
    public event System.Action OnScoreChanged;
    public static GameManager Instance { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
            return;
        }
        Instance = this;
        _pacmanInitialPos = _pacman.transform.position;
    }

    private void Start()
    {
        NewGame();
    }

    /// <summary>
    /// 新しいゲームを開始し、スコアとライフを初期化する
    /// </summary>
    void NewGame()
    {
        SetScore(0);
        SetLives(3);
    }

    /// <summary>
    /// 新しいラウンドを開始し、すべてのペレットを復活させる
    /// </summary>
    void NewRound()
    {
        foreach (Transform pellet in _pellets)
        {
            if (!pellet.gameObject.activeSelf)
                pellet.gameObject.SetActive(true);
        }
        ResetAllStates();
    }

    /// <summary>
    /// すべてのキャラクターの状態をリセットする
    /// </summary>
    void ResetAllStates()
    {
        ResetGhostMultiplier();
        foreach (Ghost ghost in _ghostsArray)
        {
            ghost.DefaultLook();
            ghost.ResetState();
            ghost.Movement.Rb.simulated = true;
        }
        GhostStatesManager.Instance.ResetStates();
        _pacman.ResetState();
        _pacman.transform.position = _pacmanInitialPos;
        _pacman.gameObject.SetActive(true);
    }

    /// <summary>
    /// ゲームオーバー時の処理
    /// </summary>
    private void GameOver()
    {
        foreach (Ghost ghost in _ghostsArray)
        {
            ghost.StopMovement();
        }
        _pacman.GameFinished();
    }

    public void GhostEaten(Ghost ghost)
    {
        IncreaseScore(_ghostMultiplier * ghost.Point);
        _ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        _pacman.Eaten();
        SetLives(this.Lives - 1);

        foreach (Ghost ghost in _ghostsArray)
        {
            ghost.Movement.IsActive = false;
            ghost.StopMovement();
            ghost.Movement.Rb.simulated = false;
        }

        if (Lives > 0)
        {
            Invoke(nameof(ResetAllStates), 2.5f);
        }
        else
        {
            GameOver();
        }
    }

    private void SetScore(int score)
    {
        this.Score = score;
        OnScoreChanged?.Invoke();
    }

    public void IncreaseScore(int score)
    {
        this.Score += score;
        OnScoreChanged?.Invoke();
    }

    void SetLives(int lives)
    {
        this.Lives = lives;
    }

    public void PowerPelletEaten()
    {
        GhostStatesManager.Instance.PowerPelletMode();
        CancelInvoke(); // 以前のInvokeをキャンセル
        Invoke(nameof(ResetGhostMultiplier), _powerModeDuration);
    }

    /// <summary>
    /// 残っているペレットがあるか確認する
    /// </summary>
    bool IsThereAnyPelletLeft()
    {
        foreach (Transform pellet in _pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ゲーム終了条件を判定し、すべてのペレットが消えたらゲーム終了
    /// </summary>
    public void IsGameEnded()
    {
        if (!IsThereAnyPelletLeft())
        {
            GameOver();
            Invoke(nameof(NewRound), 3f);
        }
    }

    private void ResetGhostMultiplier()
    {
        _ghostMultiplier = 1;
    }

    public void DebugPellet()
    {
        foreach (Transform pellet in _pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                Debug.Log(pellet.name);
            }
        }
    }
}
