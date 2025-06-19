using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ghost : MonoBehaviour
{
    [SerializeField] protected Player _targetPacman;
    [SerializeField] GhostStateID _initialState;
    [SerializeField] CircleCollider2D _collider;

    [SerializeField] GameObject _body;
    [SerializeField] GameObject _eyes;
    [SerializeField] GameObject _blueBody;
    [SerializeField] GameObject _whiteBody;

    public GhostScore ghostScore;

    public Movement Movement;
    public int Point = 200;
    
    public bool IsInHome;
    public float HomeExitTime;
    // チェイス（追跡）時のターゲット位置（デフォルトはパックマンの位置）
    public virtual Vector3 ChaseTarget()
    {
        return _targetPacman.transform.position;
    }

    // 各モードのターゲット位置
    public Vector3 ScatterTarget = new Vector3(11.5f, 18.5f, 0); // ブリンキーの散策ターゲット
    [HideInInspector] public Vector3 EatenTarget = new Vector3(0f, 3f, 0f); // 食べられた後のターゲット

    // ノードの方向固定フラグ
    public bool NodeDirectionLock = false;

    // ステートマシン
    StateMachine _stateMachine;
    // 初期位置
    Vector3 _initalPos;
    // 現在の位置
    public Vector2 CurrentPos => transform.position;

    public StateMachine StateMachine { get => _stateMachine; }
    // 経過時間
    public float TimeConsumed = 0;
    // ノード内にいるかどうか
    public bool IsInNode = false;

    private void Awake()
    {
        GetComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.NeverSleep;
        _initalPos = transform.position;
        _stateMachine = new StateMachine();
        _stateMachine.RegisterState(new GhostChase(this));
        _stateMachine.RegisterState(new GhostEaten(this));
        _stateMachine.RegisterState(new GhostFrightened(this));
        _stateMachine.RegisterState(new GhostHome(this));
        _stateMachine.RegisterState(new GhostScatter(this));
    }
    private void OnEnable()
    {
        Movement.OnDirectionChanged += HandleOnDirectionChanged;
        DefaultLook();
    }

    private void Start()
    {
        _stateMachine.ChangeState(_initialState);
    }
    private void Update()
    {
        _stateMachine.Update();
    }
    // 移動を停止
    public void StopMovement()
    {
        Movement.IsActive = false;
    }

    // 移動を開始
    public void StartMovement()
    {
        Movement.IsActive = true;
    }

    // 進行方向を逆に変更
    public void ChangeDirToOpposite()
    {
        Movement.ChangeToOppositeDir();
    }

    // 状態をリセット
    public void ResetState()
    {
        Movement.IsActive = true;
        transform.position = _initalPos;
        Movement.ResetState();
        this.gameObject.SetActive(true);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Pacman"))
        {
            switch (_stateMachine.CurrentState)
            {
                case GhostStateID.Chase:
                    GameManager.Instance.PacmanEaten();
                    break;
                case GhostStateID.Scatter:
                    GameManager.Instance.PacmanEaten();
                    break;
                case GhostStateID.Frightened:
                    GameManager.Instance.GhostEaten(this);
                    AudioManager.Instance.PlaySound(6);
                    ghostScore.SetScore(GameManager.Instance.EatSocre);
                    OnEaten();
                    _stateMachine.ChangeState(GhostStateID.Eaten);
                    break;
                default:
                    break;
            }
        }
    }

    // ノードに接触中の処理
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Node"))
        {
            _stateMachine.OnNodeCollider(collision.GetComponent<Node>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Node"))
        {
            NodeDirectionLock = false;
            IsInNode = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Node"))
        {
            IsInNode = true;
        }
    }
    private void HandleOnDirectionChanged()
    {
        NodeDirectionLock = true;
    }

    // 怯え状態の見た目
    public void FrightenedState()
    {
        _body.gameObject.SetActive(false);
        _eyes.gameObject.SetActive(false);
        _blueBody.gameObject.SetActive(true);
        _whiteBody.gameObject.SetActive(false);

        _collider.isTrigger = false;
    }
    // 怯え状態（白と青が交互）の見た目
    public void WhiteBlueLook()
    {
        _body.gameObject.SetActive(false);
        _eyes.gameObject.SetActive(false);
        _blueBody.gameObject.SetActive(false);
        _whiteBody.gameObject.SetActive(true);

        _collider.isTrigger = false;
    }
    // 通常の見た目
    public void DefaultLook()
    {
        _body.gameObject.SetActive(true);
        _eyes.gameObject.SetActive(true);
        _blueBody.gameObject.SetActive(false);
        _whiteBody.gameObject.SetActive(false);

        _collider.isTrigger = false;
        NodeDirectionLock = false;
    }
    // 食べられた状態の見た目
    public void EatenStateEnter()
    {
        _body.gameObject.SetActive(false);
        _eyes.gameObject.SetActive(true);
        _blueBody.gameObject.SetActive(false);
        _whiteBody.gameObject.SetActive(false);

        _collider.isTrigger = true;
    }

    // 目的地への最短ルートを決定
    public Vector2 MinDistanceDirection(Node node, Vector3 target)
    {
        float minDistance = float.MaxValue;
        Vector2 minDistanceDir = Vector2.zero;
        foreach (Vector2 dir in node.AvailableDirections)
        {
            if (Movement.OppositeDir() != dir)
            {
                float distance = Vector3.Distance(CurrentPos + dir, target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minDistanceDir = dir;
                }
                else if (distance == minDistance && DirectionPriority(dir) > DirectionPriority(minDistanceDir))
                {
                    minDistance = distance;
                    minDistanceDir = dir;
                }
            }
        }
        return minDistanceDir;
    }
    public Vector2 RandomDirection(Node node)
    {
        List<Vector2> currentAvailableDirections = new List<Vector2>();

        foreach (Vector2 dir in node.AvailableDirections)
        {
            if(dir != Movement.OppositeDir())
                currentAvailableDirections.Add(dir);
        }
        return currentAvailableDirections[Random.Range(0, currentAvailableDirections.Count)];
    }
    private int DirectionPriority(Vector2 direction)
    {
        int priority = 0;
        if (direction == Vector2.up)
        {
            priority = 3;
        }
        else if (direction == Vector2.left)
        {
            priority = 2;
        }
        else if (direction == Vector2.down)
        {
            priority = 1;
        }
        else if (direction == Vector2.right)
        {
            priority = 0;
        }
        return priority;
    }

    public void OnEaten()
    {
        StartCoroutine(EatenSequence());
    }

    IEnumerator EatenSequence()
    {
        // アニメーション再生（Animatorがある場合）
        /*
        if (TryGetComponent(out Animator animator))
        {
            animator.SetTrigger("Eaten");  // "Eaten" トリガーを Animator に設定
        }
        */

        // 全ての Movement を止める
        GhostStatesManager.Instance.StopAllMovements(0.4f);

        // 0.4秒待機
        yield return new WaitForSeconds(0.4f);

        // 状態を Eaten に変更
        _stateMachine.ChangeState(GhostStateID.Eaten);
    }



    public void StartBugCheck()
    {
        StopAllCoroutines();
        StartCoroutine(PosBugControl());
    }
    IEnumerator PosBugControl()
    {
        Vector3 postPos = transform.position;
        yield return new WaitForSeconds(0.5f);
        if(Vector3.Distance(postPos, transform.position)< 1f)
        {
            NodeDirectionLock = false;
        }
        yield return null;
    }



}
