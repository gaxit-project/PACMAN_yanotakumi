using UnityEngine;

public class Pellet : MonoBehaviour
{
    [SerializeField]protected int _point = 50;
    protected virtual void GetEaten()
    {
        GameManager.Instance.IncreaseScore(_point);
        this.gameObject.SetActive(false);
        GameManager.Instance.IsGameEnded();
        if(_point == 10)
        {
            AudioManager.Instance.PlaySound(2);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pacman"))
        {
            GetEaten();
        }

    }
}
