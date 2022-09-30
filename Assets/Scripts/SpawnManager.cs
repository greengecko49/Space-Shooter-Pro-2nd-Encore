using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _spaceFighter;
    [SerializeField]
    private GameObject _enemyDodger;
    [SerializeField]
    private GameObject _bossShip;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] powerups;
    [SerializeField]
    private float spawnRate = 1f;

    [SerializeField]
    private float _spaceFightertimer;
    [SerializeField]
    private float _enemyDodgertimer;
    [SerializeField]
    private float _enemyTimer;

    private int _waveCount;
    private int _enemyCount;
    [SerializeField]
    private int[] _waveEnemyTotal;
    

    [SerializeField]
    private bool _isWaveActive = true;
    private bool _stopSpawning = false;

    private int _powerUpToSpawn;



    private UI_Manager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        StartCoroutine(WaveSpawn());
    }

    public void StartSpawning()
    {
        _uiManager.UpdateWave(_waveCount + 1);
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnSpaceFighterRoutine());
        StartCoroutine(SpawnEnemyDodgerRoutine());

    }

    // Update is called once per frame
    void Update()
    {
       
        
    }




    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(_enemyTimer);
        while (_stopSpawning == false)
        {
            

            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);

            newEnemy.transform.parent = _enemyContainer.transform;



            yield return new WaitForSeconds(5.0f);



        }

    }

    IEnumerator SpawnSpaceFighterRoutine()
    {
        yield return new WaitForSeconds(_spaceFightertimer);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(0, 10, 0);
            GameObject newSpaceFighter = Instantiate(_spaceFighter, posToSpawn, Quaternion.identity);

            newSpaceFighter.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(7.0f);
        }
    }

    IEnumerator SpawnEnemyDodgerRoutine()
    {
        yield return new WaitForSeconds(_enemyDodgertimer);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemyDodger = Instantiate(_enemyDodger, posToSpawn, Quaternion.identity);

            newEnemyDodger.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(7.0f);
        }
    }

    void SpawnBossShip()
    {
        Vector3 posToSpawn = new Vector3(0, 10, 0);
        GameObject newBossShip = Instantiate(_bossShip, posToSpawn, Quaternion.identity);
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            ChooseAPowerUp();
            Instantiate(powerups[_powerUpToSpawn], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));

        }

    }

    IEnumerator WaveSpawn()
    {
        while (_isWaveActive == true && _stopSpawning == false)
        {
            if (_enemyCount == _waveEnemyTotal[_waveCount])
            {
                _stopSpawning = true;
                Enemy[] enemychildren = _enemyContainer.GetComponentsInChildren<Enemy>();
                for(int i = 0; i < enemychildren.Length; i++)
                {
                    enemychildren[i].StopRespawning();
                }
                
                EnemyDodger[] dodgerchildren = _enemyContainer.GetComponentsInChildren<EnemyDodger>();
                for(int i = 0; i < dodgerchildren.Length; i++)
                {
                    dodgerchildren[i].StopRespawning();
                }
                _waveCount++;
                yield return new WaitForSeconds(8.0f);
                _stopSpawning = false;
                _enemyCount = 0;
                StartSpawning();
            }
            else if(_waveCount == 5)
            {
                _stopSpawning = true;
                SpawnBossShip();
            }

            yield return null;
        }
    }

    void ChooseAPowerUp()
    {
        int weightedTotal = 100;

        int[] powerupTable =
        {
            30, //ammo
            10, //missile
            10, //health
            10, //shield
            10, //speed
            10, //sword
            10, //tripleshot
            10, //negative speed down
        };
        int[] PowerupToAward =
        {
            3, //ammo
            7, //missile
            4, //health
            2, //shield
            1, //speed
            5, //sword
            0, //tripleshot
            6, //negative speed down
        };

         foreach(var item in powerupTable)
         {
            weightedTotal += item;
         }

        var randomNumber = Random.Range(0, weightedTotal);
        var i = 0;

        foreach(var weight in powerupTable)
        {
            if(randomNumber <= weight)
            {
                _powerUpToSpawn = PowerupToAward[i];
                return;
            }
            else
            {
                i++;
                randomNumber -= weight;
            }
        }
    }

    public void EnemyCount()
    {
        _enemyCount++;
    }
    

    
    

    public void OnPlayerDeath()
    {
        _stopSpawning = true;

    }
}
