using System.Collections;
using UnityEngine;

public class GhostStatesManager : MonoBehaviour
{
    [SerializeField] Ghost[] _ghostsArray;
    [SerializeField] float _maxScatterTime;
    [SerializeField] float _minScatterTime;
    [SerializeField] float _maxChaseTime;
    [SerializeField] float _minChaseTime;
    float _scatterTime;
    float _chaseTime;
    private Movement _playerMovement;

    GhostStateID _initialState = GhostStateID.Scatter;
    public GhostStateID CurrentState;
    float _timeCounter;

    public static GhostStatesManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _scatterTime = GetRandomScatterTime();
        _chaseTime = GetRandomChaseTime();
        ChangeGhostStates(_initialState);
        _playerMovement = GameObject.Find("Player").GetComponent<Movement>();
    }
    private void Update()
    {
        _timeCounter+=Time.deltaTime;
        if(CurrentState == GhostStateID.Scatter)
        {
            if(_timeCounter >= _scatterTime)
            {
                ChangeGhostStates(GhostStateID.Chase);
                CurrentState = GhostStateID.Chase;
                _chaseTime = GetRandomChaseTime();
                _timeCounter = 0;
            }
        }
        else if(CurrentState == GhostStateID.Chase)
        {
            if (_timeCounter >= _chaseTime)
            {
                ChangeGhostStates(GhostStateID.Scatter);
                CurrentState = GhostStateID.Scatter;
                _scatterTime = GetRandomScatterTime();
                _timeCounter = 0;
            }
        }

    }
    public void ResetStates()
    {
        _scatterTime = GetRandomScatterTime();
        _chaseTime = GetRandomChaseTime();
        foreach (Ghost ghost in _ghostsArray)
        {
            if(ghost.TryGetComponent(out BlinkyGhost blinky))
            {
                blinky.StateMachine.ChangeState(GhostStateID.Chase);
                continue;
            }
            else
            {
                ghost.StateMachine.ChangeState(GhostStateID.Home);
            }

        }
        ChangeGhostStates(_initialState);
    }
    float GetRandomScatterTime()
    {
        return Random.Range(_minScatterTime, _maxScatterTime);
    }
    float GetRandomChaseTime()
    {
        return Random.Range(_minChaseTime, _maxChaseTime);
    }
    void ChangeGhostStates(GhostStateID ghostStateID)
    {
        foreach (Ghost ghost in _ghostsArray)
        {
            if(ghost.IsInHome) { continue; }
            if (ghost.StateMachine.CurrentState == GhostStateID.Frightened) continue;
            if (ghost.StateMachine.CurrentState == GhostStateID.Home) continue;
            if (ghost.StateMachine.CurrentState == GhostStateID.Eaten) continue;
            ghost.StateMachine.ChangeState(ghostStateID);
        }
    }

    public void PowerPelletMode()
    {
        foreach (Ghost ghost in _ghostsArray)
        {
            if (ghost.StateMachine.CurrentState == GhostStateID.Eaten) continue;
            ghost.StateMachine.ChangeState(GhostStateID.Frightened);
        }
    }

    public void StopAllMovements(float duration)
    {
        StartCoroutine(HitStopRoutine(duration));
    }

    IEnumerator HitStopRoutine(float duration)
    {
        // 移動停止
        foreach (var ghost in _ghostsArray)
        {
            ghost.Movement.SaveDirections();
            ghost.Movement.Freeze(true);
        }

        _playerMovement.SaveDirections();
        _playerMovement.Freeze(true); // プレイヤーも止める


        yield return new WaitForSeconds(duration);

        // 移動再開（必要なら IsActive を使って再開できるように）
        foreach (var ghost in _ghostsArray)
        {
            ghost.ghostScore.ResetScore();
            ghost.Movement.Freeze(false);
            ghost.Movement.RestoreDirections();
        }

        _playerMovement.Freeze(false);
        _playerMovement.RestoreDirections();

    }
}
