using UnityEngine;

public class Player : MonoBehaviour
{
    // 移動制御スクリプトへの参照
    [SerializeField] Movement _movement;

    // スプライトの反転制御（向き変更）
    [SerializeField] Roll _roll;

    // アニメーターコンポーネントへの参照
    [SerializeField] Animator _anim;

    // 入力を受け付けるかどうか
    bool _canReadInput = true;

    // Movementコンポーネントのプロパティ
    public Movement Movement { get => _movement; }

    private void OnEnable()
    {
        // 方向変更イベントにハンドラを登録
        _movement.OnDirectionChanged += HandleOnDirectionChanged;
    }

    private void Update()
    {
        if (!_canReadInput) return;  // 入力不可ならスキップ

        // 初回キー入力で移動を開始
        if (!_movement.IsActive && Input.anyKeyDown)
        {
            _movement.IsActive = true;
        }

        // 入力による移動方向の変更
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _movement.SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _movement.SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _movement.SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _movement.SetDirection(Vector2.right);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // ノードに接触中なら次の方向へ移動を試みる
        if (collision.CompareTag("Node"))
        {
            _movement.TryNextDirection();
        }

        // 停止アニメーションの切り替え
        if (_movement.IsStopeed)
        {
            _anim.SetBool("Stop", true);    // 停止時はアニメーションを停止
        }
        else
        {
            _anim.SetBool("Stop", false);   // 移動中は再生
        }
    }

    /// <summary>
    /// パックマンを初期状態にリセット
    /// </summary>
    public void ResetState()
    {
        _anim.SetBool("Reset", true);          // リセットアニメーションを再生
        _movement.Rb.simulated = true;         // Rigidbodyを有効化
        _movement.ResetState();                // 移動状態をリセット
        _roll.RotateSprite(Vector2.right);     // 右向きにリセット
        _canReadInput = true;                  // 入力を許可
    }

    /// <summary>
    /// 方向変更時の処理
    /// </summary>
    private void HandleOnDirectionChanged()
    {
        // スプライトの向きを移動方向に合わせる
        _roll.RotateSprite(_movement.CurrentDir);
    }

    /// <summary>
    /// パックマンが食べられたときの処理
    /// </summary>
    public void Eaten()
    {
        _canReadInput = false;               // 入力を禁止
        _movement.StopMovement();            // 移動を停止
        _movement.Rb.simulated = false;      // Rigidbodyを無効化
        _movement.IsActive = false;          // 移動を無効化
        _anim.SetTrigger("Eaten");           // 食べられたアニメーション再生
    }

    /// <summary>
    /// ゲーム終了時の処理
    /// </summary>
    public void GameFinished()
    {
        _movement.StopMovement();            // 移動を停止
        _movement.IsActive = false;          // 移動を無効化
        _anim.SetTrigger("Finished");        // 終了アニメーション再生
    }

    /// <summary>
    /// 食べられたアニメーションのイベント処理
    /// </summary>
    public void EatenAnimationEvent()
    {
        // ゲームオブジェクトを非アクティブ化
        this.gameObject.SetActive(false);
    }
}
