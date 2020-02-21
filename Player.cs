using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 10;
    [SerializeField]
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldPrefab;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine , _leftEngine;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    void Start()
    {
        transform.position = new Vector3(0,0,0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();


        _audioSource = GetComponent<AudioSource>();
        if(_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("Uimanager is null.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("audiosource is null.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
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


    void FireLaser()
    {
            _canFire = Time.time + _fireRate;

        if(_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }


        _audioSource.Play();
    }


    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

            transform.Translate(direction * _speed * Time.deltaTime);

        if (transform.position.y >= 0)
        {
            
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }






        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);

        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    public void Damage()
    {

        if(_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;


        if(_lives == 2)
        {
            _leftEngine.SetActive(true);

        }
        else if(_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
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


    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;

        _uiManager.UpdateScore(_score);
    }

}



