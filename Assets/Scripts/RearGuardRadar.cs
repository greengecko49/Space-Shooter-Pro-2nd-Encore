using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearGuardRadar : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            transform.parent.GetComponent<Enemy>().ReverseLaserFiring();
            
        }
    }
}
