using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShip : MonoBehaviour
{
    [SerializeField]
    private int _speed = 4;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private int _bossHealth = 50;

    private bool _isRespawning = true;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    private Player _player;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if( Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < 0 && _isRespawning == true)
        {
            transform.position = new Vector3(0,2,0);
            _speed = 0;
        }
    }

    void FireLaser()
    {
        _fireRate = 3f;
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            


        }
    }

    public void Damage()
    {
        _bossHealth--;
        if (_bossHealth == 0)
        {
            Kill();
        }
    }

    public void Kill()
    {

        _speed = 0;
        _audioSource.Play();


        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, 2.0f);
    }
}
