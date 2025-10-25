using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];

    private float _spawnTimer;
    private float _spawnCounter;
    private int _enemiesRemoved;

    [SerializeField] private ObjectPooler crawlerPool;
    [SerializeField] private ObjectPooler brutePool;
    [SerializeField] private ObjectPooler wraithPool;

    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

    private float _timeBetweenWaves = 2f;
    private float _waveCooldown;
    private bool _isBetweenWaves = false;

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Crawler, crawlerPool },
            { EnemyType.Brute, brutePool },
            { EnemyType.Wraith, wraithPool },
        };
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
    }
    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }

    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0 && _spawnCounter < CurrentWave.enemiesPerWave)
        {
            _spawnTimer = CurrentWave.spawnInterval;
            SpawnEnemy();
            _spawnCounter++;
        } 
        else if (_spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >= CurrentWave.enemiesPerWave)
        {
            _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
            _spawnCounter = 0;
            _enemiesRemoved = 0;
        }
    }

    private void SpawnEnemy()
    {
        if (_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;
            spawnedObject.SetActive(true);
        }
        
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }
}
