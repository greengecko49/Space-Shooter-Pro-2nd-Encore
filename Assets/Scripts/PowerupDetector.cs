using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupDetector : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Powerup")
        {
            transform.parent.GetComponent<Enemy>().FireLaser();
        }
    }
}
