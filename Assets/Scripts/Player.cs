using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //private variable reference
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;
    private float _thrusterSpeed = 10f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private int _ammo = 15;
    [SerializeField]
    private int _maxAmmo = 30;
    [SerializeField]
    private GameObject _tripleshotprefab;
    [SerializeField]
    private GameObject _spaceSword;
    [SerializeField]
    private GameObject _missileprefab;
    [SerializeField]
    private GameObject _speedDown;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _shieldHealth = 3;
    [SerializeField]
    private float _fuel = 100f;
    [SerializeField]
    private float _fuelUseage = 8f;
    private bool _fuelCooldownActive = false;

    private SpawnManager _spawnManager;
    private UI_Manager _uiManager;

    
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    private bool _isSpaceSwordActive = false;
    private bool _isSpeedDownActive = false;
    private bool _isMissilesActive = false;  

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private SpriteRenderer _playerShieldColor;
    [SerializeField]
    private Color _shieldColorFull, _shieldColorHalf, _shieldColorMinimum, _shieldColor0;

    [SerializeField]
    private GameObject _rightEngine, _leftEngine;

    [SerializeField]
    private int _score = 0;

    [SerializeField]
    private AudioClip _laserShotClip;
    
    
    private AudioSource _audioSource;

    private CameraShake _cameraShake;


    // Start is called before the first frame update
    void Start()
    {
        _rightEngine.SetActive(false);

        _leftEngine.SetActive(false);

        _shieldVisualizer.SetActive(false);

        _spaceSword.SetActive(false);

        _speedDown.SetActive(false);


        // take the current position = new position (0, 0 ,0)
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = Camera.main.GetComponent<CameraShake>();
        

        if (_spawnManager == null)
        {

            Debug.LogError("The Spawn Manager is Null.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is null");
        }
        else
        {
            _audioSource.clip = _laserShotClip;
        }

    }
    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {

            FireLaser();
        }





    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift) && !_fuelCooldownActive)
        {
            if (_fuel > 0)
            {
                transform.Translate(direction * _thrusterSpeed * Time.deltaTime);
                _fuel -= _fuelUseage * Time.deltaTime;
                _uiManager.UpdateThrusterFuel(_fuel);
            }
            else
            {
                _fuel = 0;
                _thrusterSpeed = 0;
                StartCoroutine(ThrusterCooldownRoutine());
            }
            
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && !_fuelCooldownActive)
        {
            _thrusterSpeed = 0;
            StartCoroutine(ThrusterCooldownRoutine());
        }

        


        //if player position on the y is greater than 0 then y position = 0
        if (transform.position.y > 4.1f)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y < -3.91f)
        {
            transform.position = new Vector3(transform.position.x, -2.75f, 0);
        }

        //If player goes to far player will loop around to other side of screen

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }

    }

    IEnumerator ThrusterCooldownRoutine()
    {
        yield return new WaitForSeconds(3f);
        _fuelCooldownActive = true;
        while (true)
        {
            _fuel += 15 * Time.deltaTime;
            if (_fuel >= 100f)
            {
                _fuel = 100f;
                _fuelCooldownActive = false;
                break;
            }
            _uiManager.UpdateThrusterFuel(_fuel);
            yield return null;
        }
    }

    void FireLaser()
    {


        {
            _canFire = Time.time + _fireRate;

            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleshotprefab, transform.position, Quaternion.identity);
            }
            else if(_isMissilesActive == true)
            {
                Instantiate(_missileprefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            }
            else if (_ammo > 0)
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
                _ammo--;
                _uiManager.UpdateAmmo(_ammo);
            }

            _audioSource.Play();


        }

    }


    public void Damage()
    {
        if (_isShieldsActive == true)
        {
            _shieldHealth--;

            switch (_shieldHealth)
            {
                case 2:
                    _playerShieldColor.color = _shieldColorHalf;
                    break;
                case 1:
                    _playerShieldColor.color = _shieldColorMinimum;
                    break;
                case 0:
                    _playerShieldColor.color = _shieldColor0;
                    _isShieldsActive = false;
                    _shieldVisualizer.SetActive(false);
                    break;
            }
            return;
        }

        _lives--;
        StartCoroutine(_cameraShake.Shake(.10f, .2f));

        if(_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if(_lives == 1)
        {
            _rightEngine.SetActive(true);   
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
            
        }


    }

    public void Plus1Health()
    {
       if (_lives == 3)
        {
            Debug.Log("Lives at MAX");
            return;
        }

       _lives++;
        _uiManager.UpdateLives(_lives);

        if (_lives == 3)
        {
            _leftEngine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _rightEngine.SetActive(false);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }


    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void ActivateMissiles()
    {
        _isMissilesActive = true;
        StartCoroutine(MissilePowerDown());
    }

    IEnumerator MissilePowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isMissilesActive=false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void ShieldBoostActive()
    {
        _shieldVisualizer.SetActive(true);
        _playerShieldColor.color = _shieldColorFull;
        _shieldHealth = 3;
        _isShieldsActive = true;

    }

    public void ActivateSpaceSword()
    {
        _isSpaceSwordActive = true;
        _spaceSword.gameObject.SetActive(true);
        StartCoroutine(SpaceSwordPowerDown()); 
    }

    IEnumerator SpaceSwordPowerDown()
    {
        yield return new WaitForSeconds(15.0f);
        _isSpaceSwordActive=false;
        _spaceSword.gameObject.SetActive(false);
    }

    public void ActivateSpeedDown()
    {
        _isSpeedDownActive = true;
        _speedDown.SetActive(true);
        _speed -= 2.0f;
        StartCoroutine(SpeedDownPowerDown());
    }

    IEnumerator SpeedDownPowerDown()
    {
        yield return new WaitForSeconds(7.0f);
        _isSpeedDownActive=false;
        _speedDown.gameObject.SetActive(false);
        _speed += 2.0f;
    }



    public void AddAmmo()
    {
        _ammo += 15;
        if(_ammo > _maxAmmo)
        {
            _ammo = _maxAmmo;
        }

        _uiManager.UpdateAmmo(_ammo);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

}
