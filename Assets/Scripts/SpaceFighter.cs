using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceFighter : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    public GameObject _photonPrefab;
    [SerializeField]
    private Vector3 _photonOffset;

    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    private bool _isRespawning = true;

    private Player _player;
    private AudioSource _audioSource;
    private PhotonBurst _photonBurst;

    private SpawnManager _spawnManager;
    private UI_Manager _uiManager;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        _photonBurst = GetComponent<PhotonBurst>();
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
            FirePhoton();
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < 0 && _isRespawning == true)
        {
            transform.position = Vector3.zero;
            int x = Random.Range(0, 100);
            if (x > 50)
            {

                transform.Rotate(0.0f, 0.0f, 90.0f);
            }
            else
            {

                transform.Rotate(0.0f, 0.0f, -90.0f);
            }


        }

        if (transform.position.x == 12.0f && _isRespawning == true)
        {
            Destroy(this.gameObject);
        }
        else if (transform.position.x == -12.0f && _isRespawning == true)
        {
            Destroy(this.gameObject);
        }
    }

    void FirePhoton()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        Instantiate(_photonPrefab, transform.position + _photonOffset, Quaternion.identity);

        
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

        if (other.tag == "SpaceSword")
        {

            if (_player != null)
            {
                _player.AddScore(30);
            }

            Kill();

        }

        if (other.tag == "Laser")
        {

            Destroy(other.gameObject);

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
