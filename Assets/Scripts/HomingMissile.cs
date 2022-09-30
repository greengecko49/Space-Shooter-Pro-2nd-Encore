using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private GameObject target;
    Rigidbody2D rb;
    private float _thrustSpeed = 5f;
    private float rotationSpeed = 75f;
    [SerializeField]
    private GameObject _explosionprefab;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Enemy");

        StartCoroutine(FindNewTarget());
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            var dir = (target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var rotateToTarget = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateToTarget, Time.deltaTime * rotationSpeed);
            rb.velocity = new Vector2(dir.x * _thrustSpeed, dir.y * _thrustSpeed);
        }
        else
        {
            transform.Translate(Vector3.forward * _thrustSpeed * Time.deltaTime);
        }

        BoundaryCheck();
    }

    void BoundaryCheck()
    {
        if (transform.position.z > 1 || transform.position.z < -1)
        {
            Explosion();
        }
    }


    public void Explosion()
    {
        Instantiate(_explosionprefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }


    IEnumerator FindNewTarget()
    {
        while (target == null)
        {
            yield return new WaitForSeconds(.2f);
            target = GameObject.FindWithTag("Enemy");
        }
    }

}
