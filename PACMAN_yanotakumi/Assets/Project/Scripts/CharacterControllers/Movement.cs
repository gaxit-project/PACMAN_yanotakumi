using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    // インスペクタで設定可能なRigidbody2Dへの参照
    [SerializeField] Rigidbody2D _rb;

    // 移動速度
    [SerializeField] float _speed;
    // 初期移動方向
    [SerializeField] Vector2 _initialDir;
    // 障害物判定に使用するレイヤーマスク
    [SerializeField] LayerMask _obstacleLayer;

    // 速度倍率（パワーアップなどで速度を変化させる際に使用）
    float _speedMultiplier = 1.0f;
    // 初期位置を保持
    Vector3 _startPos;
    // 現在の移動方向
    Vector2 _currentDir;
    // 次に移動予定の方向
    Vector2 _nextDir;

    // 移動中かどうかを判定するフラグ
    public bool IsActive = false;

    bool _isFrozen = false;

    // 停止状態かどうかを判定（速度がゼロなら停止と判定）
    public bool IsStopeed => _rb.linearVelocity == Vector2.zero;

    // 現在の移動方向を取得・設定
    public Vector2 CurrentDir { get => _currentDir; set => _currentDir = value; }

    // Rigidbody2Dへの参照を取得・設定
    public Rigidbody2D Rb { get => _rb; set => _rb = value; }

    // 次に移動予定の方向を取得・設定
    public Vector2 NextDir { get => _nextDir; set => _nextDir = value; }

    // 方向変更時に発火するイベント
    public event System.Action OnDirectionChanged;

    private Vector2 _savedCurrentDir;
    private Vector2 _savedNextDir;

    private void Awake()
    {
        // 初期位置を保持
        _startPos = transform.position;
        // 初期状態にリセット
        ResetState();
    }

    /// <summary>
    /// 移動状態を初期化
    /// </summary>
    public void ResetState()
    {
        _speedMultiplier = 1.0f;      // 速度倍率をリセット
        _currentDir = _initialDir;    // 初期方向にリセット
        _nextDir = Vector2.zero;      // 次方向をリセット
        transform.position = _startPos; // 初期位置に戻す
        //_rb.isKinematic = false;      // 物理挙動を有効化
    }

    private void FixedUpdate()
    {
        if (!IsActive) return;    // 移動が有効でなければ処理をスキップ

        // Rigidbodyに速度を与えて移動させる
        _rb.linearVelocity = _currentDir * _speed * _speedMultiplier;
    }

    /// <summary>
    /// 移動方向を設定
    /// </summary>
    /// <param name="direction">指定する方向</param>
    /// <param name="forced">強制的に変更する場合はtrue</param>
    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (direction == _nextDir) return;    // 同じ方向なら無視

        if (CheckIfOppositeDir(direction))    // 現在と逆方向ならすぐ変更
        {
            ChangeDirection(direction);
        }
        else if (forced || !IsThereObstacle(direction))   // 強制または障害物がなければ変更
        {
            ChangeDirection(direction);
        }
        else
        {
            _nextDir = direction;   // 障害物がある場合は次方向に保持
        }
    }

    /// <summary>
    /// 指定方向に障害物があるか判定
    /// </summary>
    public bool IsThereObstacle(Vector2 direction)
    {
        // BoxCastで方向に障害物があるかを判定
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,                 // 現在位置
            Vector2.one * 0.7f,                 // コリジョンサイズ
            0f,                                 // 回転なし
            direction,                          // 判定方向
            1.5f,                               // 判定距離
            _obstacleLayer                      // 障害物レイヤー
        );
        return hit.collider != null;            // 障害物があればtrueを返す
    }

    /// <summary>
    /// 次の方向へ進む（ノードで呼び出される）
    /// </summary>
    public void TryNextDirection()
    {
        if (_nextDir != Vector2.zero)
        {
            if (!IsThereObstacle(_nextDir))    // 障害物がなければ方向変更
            {
                ChangeDirection(_nextDir);
            }
        }
    }

    /// <summary>
    /// 現在方向と指定方向が逆か判定
    /// </summary>
    public bool CheckIfOppositeDir(Vector2 direction)
    {
        return (_currentDir == Vector2.left && direction == Vector2.right) ||
               (_currentDir == Vector2.right && direction == Vector2.left) ||
               (_currentDir == Vector2.up && direction == Vector2.down) ||
               (_currentDir == Vector2.down && direction == Vector2.up);
    }

    /// <summary>
    /// 移動方向を変更
    /// </summary>
    public void ChangeDirection(Vector2 direction)
    {
        if (_currentDir == direction)  // 同じ方向なら処理をスキップ
        {
            _nextDir = Vector2.zero;
            return;
        }

        _currentDir = direction;      // 現在方向を更新
        _nextDir = Vector2.zero;      // 次方向をリセット
        OnDirectionChanged?.Invoke(); // イベントを発火
    }

    /// <summary>
    /// 次方向を設定
    /// </summary>
    public void SetNextDirection(Vector2 dir)
    {
        if (!IsThereObstacle(dir))     // 障害物がなければ方向変更
        {
            ChangeDirection(dir);
        }
    }

    /// <summary>
    /// 移動を停止
    /// </summary>
    public void StopMovement()
    {
        _rb.linearVelocity = Vector2.zero;    // 速度をゼロにして停止
    }

    /// <summary>
    /// 反対方向へ変更
    /// </summary>
    public void ChangeToOppositeDir()
    {
        Vector2 opposite = OppositeDir();
        if (!IsThereObstacle(opposite))    // 障害物がなければ変更
        {
            _currentDir = opposite;
            _nextDir = Vector2.zero;
        }
    }

    /// <summary>
    /// 現在の反対方向を取得
    /// </summary>
    public Vector2 OppositeDir()
    {
        if (_currentDir == Vector2.right) return Vector2.left;
        if (_currentDir == Vector2.left) return Vector2.right;
        if (_currentDir == Vector2.up) return Vector2.down;
        if (_currentDir == Vector2.down) return Vector2.up;
        return Vector2.zero;
    }

    /// <summary>
    /// 速度倍率を変更
    /// </summary>
    /// <param name="multiplier">倍率（1.0fが標準）</param>
    public void ChangeSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }

    // 一時的に方向を保存
    public void SaveDirections()
    {
        _savedCurrentDir = CurrentDir;
        _savedNextDir = NextDir;
    }

    // 保存した方向を復元
    public void RestoreDirections()
    {
        CurrentDir = _savedCurrentDir;
        NextDir= _savedNextDir;
    }




    public void Freeze(bool isFreeze)
    {
        _isFrozen = isFreeze;

        if (_isFrozen)
        {
            _currentDir = Vector2.zero;
            _nextDir = Vector2.zero;
            // Rigidbodyがある場合は速度を止める
            if (TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
                //rb.isKinematic = true;
            }
        }
        else
        {
            if (TryGetComponent(out Rigidbody2D rb))
            {
                //rb.isKinematic = false;
            }
        }
    }

    public void Move()
    {
        if (_isFrozen) return;

        // 通常の移動処理
        transform.Translate(_currentDir * _speed * _speedMultiplier * Time.deltaTime);
    }

}
