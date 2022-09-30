using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeRadar : MonoBehaviour
{
    
    //dodging the laser
    private void OnTriggerEnter2D(Collider2D other)
    {
       if (other.tag == "Laser")
       {
            Debug.Log(other.name,other.gameObject);
            Laser laser;
            if(other.TryGetComponent<Laser>(out laser))
            {
                if (laser.IsEnemyLaser() == false)
                {
                    transform.parent.GetComponent<EnemyDodger>().DodgeLaser(true, other.transform.position.x);
                }
            }
            
            
       }
       
    }

    //resets the laser dodge
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            transform.parent.GetComponent<EnemyDodger>().DodgeLaser(false, 0);
        }
    }
}
