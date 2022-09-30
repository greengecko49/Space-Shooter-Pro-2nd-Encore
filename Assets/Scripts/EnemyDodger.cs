using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDodger : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private Vector3 _laserOffset;

    private Player _player;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = 0f;

    private bool _isRespawning = true;
    private bool _firelaser = true;

    public bool isDodging;
    public float laserXpos;

    private SpawnManager _spawnManager;
    private UI_Manager _uiManager;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        //move down at 4 meters per second
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //if bottom of screen respawn at the top with a different position on x

        if (transform.position.y < -5f && _isRespawning == true)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
        else if (transform.position.y < -5f && _isRespawning == false)
        {
            Destroy(this.gameObject);
        }
        if (isDodging)
        {
            if (laserXpos > this.transform.position.x)
            {
                this.transform.position += Vector3.left * Time.deltaTime * (_speed * 2);
            }
            else
            {
                this.transform.position -= Vector3.right * Time.deltaTime * (_speed * 2);
            }
        }


    }

    public void DodgeLaser(bool dodging, float xPosLaser)
    {
        isDodging = dodging;
        laserXpos = xPosLaser;
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

            

            Kill();
        }


        if (other.tag == "Laser")
        {
            HomingMissile missile = other.transform.GetComponent<HomingMissile>();
            if (missile != null)
            {
                missile.Explosion();
            }

            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }

            Laser laser = other.transform.GetComponent<Laser>();
            if (laser != null && laser.IsEnemyLaser() == false)
            {
                Destroy(other.gameObject);
                if (_player != null)
                {
                    _player.AddScore(10);
                }
            }



                Kill();

        }


        if (other.tag == "SpaceSword")
        {

            if (_player != null)
            {
                _player.AddScore(10);
            }

            

            Kill();
        }


    }

    public void Kill()
    {
        
        _speed = 0;
        _audioSource.Play();


        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, 2.0f);
        _spawnManager.EnemyCount();
    }

    public void StopRespawning()
    {
        _isRespawning = false;
    }
}
