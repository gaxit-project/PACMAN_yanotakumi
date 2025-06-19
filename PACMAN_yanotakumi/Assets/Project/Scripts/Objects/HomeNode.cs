using UnityEngine;

public class HomeNode : MonoBehaviour
{
    [SerializeField] Ghost[] _ghostsArray;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Ghost"))
        {
            collision.GetComponent<Ghost>().Movement.ChangeDirection(Vector2.up);

            foreach (var ghost in _ghostsArray)
            {
                if (ghost.StateMachine.CurrentState == GhostStateID.Eaten)
                {
                    ghost.StateMachine.ChangeState(GhostStateID.Chase);
                    ghost.StartBugCheck();
                }
                    
            }
            //if(collision.TryGetComponent<PinkyGhost>(out PinkyGhost ghost))
        }
    }
}
