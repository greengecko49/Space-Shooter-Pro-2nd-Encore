using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSword : MonoBehaviour
{
    private GameObject BossShip;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
        }

        if (other.tag == "SpaceFighter")
        {
            Destroy(other.gameObject);
        }

        if (other.tag == "Boss")
        {
            BossShip boss = other.GetComponent<BossShip>();
            boss.Damage();
        }
        
    }
}
