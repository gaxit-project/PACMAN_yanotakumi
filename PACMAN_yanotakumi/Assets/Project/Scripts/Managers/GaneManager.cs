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

    public int OneUpScore { get; private set; }

    const int OneUpBorder = 10000;

    public int EatSocre { get; private set; }
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

        SetScore(0);
        SetLives(3);
        OneUpScore = 0;
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {

    }

    /// <summary>
    /// 新しいゲームを開始し、スコアとライフを初期化する
    /// </summary>
    void NewGame()
    {
        // 移動停止
        foreach (var ghost in _ghostsArray)
        {
            ghost.Movement.SaveDirections();
            ghost.Movement.Freeze(true);
        }

        _pacman.Movement.SaveDirections();
        _pacman.Movement.Freeze(true); // プレイヤーも止める


        GameCall.Instance.GameReady();
        SetScore(0);
        SetLives(3);
        AudioManager.Instance.PlaySound(0);
        StartCoroutine(StartGame(4.5f));
    }

    IEnumerator StartGame(float time)
    {
        yield return new WaitForSeconds(time);

        GameCall.Instance.ResetText();

        // 移動再開（必要なら IsActive を使って再開できるように）
        foreach (var ghost in _ghostsArray)
        {
            ghost.Movement.Freeze(false);
            ghost.Movement.RestoreDirections();
        }

        _pacman.Movement.Freeze(false);
        _pacman.Movement.RestoreDirections();
        AudioManager.Instance.PlayBGM(0);
        _pacman._canReadInput = true;
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
        GameCall.Instance.ResetText();
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

    public void StopGame()
    {
        foreach (Ghost ghost in _ghostsArray)
        {
            ghost.StopMovement();
        }
        _pacman.GameFinished();
    }

    /// <summary>
    /// ゲームオーバー時の処理
    /// </summary>
    private void GameOver()
    {
        GameCall.Instance.GameOver();

        StopGame();

        AudioManager.Instance.PlaySound(7);
        StartCoroutine(Next(4.5f));

    }

    public void GhostEaten(Ghost ghost)
    {
        IncreaseScore(_ghostMultiplier * ghost.Point);
        _ghostMultiplier *= 2;
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
        EatSocre = score;
        this.Score += score;
        OneUpScore += score;
        if (OneUpScore >= OneUpBorder)
        {
            OneUpLives();
            OneUpScore = 0;
        }
        OnScoreChanged?.Invoke();
    }

    void SetLives(int lives)
    {
        this.Lives = lives;
    }

    void OneUpLives()
    {
        this.Lives++;
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
            GameCall.Instance.GameClear();
            StopGame();
            AudioManager.Instance.PlaySound(9);
            Invoke(nameof(NewRound), 3f);
        }
    }

    private void ResetGhostMultiplier()
    {
        EatSocre = 0;
        _ghostMultiplier = 1;
    }

    IEnumerator Next(float time)
    {

        yield return new WaitForSeconds(time);

        GameCall.Instance.ResetText();
        SceneManager.Instance.Title();
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
