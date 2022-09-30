using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _reverseLaserPrefab;
    

    [SerializeField]
    private GameObject _shield;

    private bool _isShieldOn;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    private bool _isRespawning = true;
    public bool isChasing;
    private bool _firelaser = true;

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
        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("animator is NULL.");
        }

        int x = Random.Range(0, 100);
        if(x > 50)
        {
            _isShieldOn = true;
        }
        else
        {
            _isShieldOn = false;
        }
        _shield.SetActive(_isShieldOn);
    }

    // Update is called once per frame
    void Update()
    {

        CalculateMovement();

        if (Time.time > _canFire)
        {
            FireLaser();
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
        else if(transform.position.y < -5f && _isRespawning == false)
        {
            Destroy(this.gameObject);
        }

        if (isChasing)
        {
            Debug.Log("Moving Towards Player");
            Vector3 dir = this.transform.position - _player.transform.forward;
            dir = dir.normalized;
            this.transform.position -= dir * Time.deltaTime * (_speed);
        }

    }

    public void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    IEnumerator FireReverseLaser()
    {
        Instantiate(_reverseLaserPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(5.0f);
    }

    public void ReverseLaserFiring()
    {
        StartCoroutine(FireReverseLaser());
    }

    

    public void ChasePlayer(bool chasing)
    {
        Debug.Log("Is Chasing Player");
        isChasing = chasing;
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

            if (_isShieldOn == true)
            {
                _shield.SetActive(false);
                _isShieldOn = false;
            }

            Kill();
        }


        if (other.tag == "Laser")
        {
            HomingMissile missile = other.transform.GetComponent<HomingMissile>();
            if(missile != null)
            {
                missile.Explosion();

                Destroy(other.gameObject);
                if (_player != null)
                {
                    _player.AddScore(10);
                }

                if (_isShieldOn == true)
                {

                    _shield.SetActive(false);
                    _isShieldOn = false;
                    return;

                }

                Kill();
            }

            Laser laser = other.transform.GetComponent<Laser>();
            if (laser != null && laser.IsEnemyLaser() == false)
            {
                Destroy(other.gameObject);
                if (_player != null)
                {
                    _player.AddScore(10);
                }

                if (_isShieldOn == true)
                {

                    _shield.SetActive(false);
                    _isShieldOn = false;
                    return;

                }

                Kill();
            }

           

            
            
        }


        if (other.tag == "SpaceSword")
        {
            
            if (_player != null)
            {
                _player.AddScore(10);
            }

            if (_isShieldOn == true)
            {
                _shield.SetActive(false);
                _isShieldOn = false;

            }

            Kill();    
        }


    }

    public void Kill()
    {
            _anim.SetTrigger("On_Enemy_Death");
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
