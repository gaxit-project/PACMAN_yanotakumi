
using UnityEngine;

public class PowerPellet : Pellet
{

    protected override void GetEaten()
    {
        base.GetEaten();
        if (base._point == 50)
        {
            AudioManager.Instance.PlaySound(8);
        }
        GameManager.Instance.PowerPelletEaten();
    }
}
