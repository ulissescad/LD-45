using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{

    public static GameManager SINGLETON;

    public Vector2 _playerPos = new Vector2(7, 8);

    public Vector3 _initialPosPlayer;

    public enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }

    [SerializeField]
    private float _movTime = 0.1f;

    [SerializeField]
    private int _revealTime=100;

    [SerializeField]
    private int _finalDelay=5000;

    [SerializeField]
    Animator _animBonus;

    [SerializeField]
    Animator _animEnding;

    [SerializeField]
    GameObject _prefabEnemy;

    [SerializeField]
    GameObject _prefabBonus;

    [SerializeField]
    GameObject[] _respawnEnemyPoint = new GameObject[4];

    [SerializeField]
    List<GameObject> _enemylist = new List<GameObject>();

    [SerializeField]
    int _maxEnemies = 10;

    [SerializeField]
    AudioClip[] _inGame = new AudioClip[3];

    [SerializeField]
    AudioClip _levelAudio;

    [SerializeField]
    AudioClip _mainAudio;

    [SerializeField]
    AudioClip _movPlayer;

    [SerializeField]
    AudioClip _victoryAudio;

    [SerializeField]
    AudioClip _lossAudio;

    [SerializeField]
    AudioSource _sourceBonus;

    [SerializeField]
    AudioSource _sourcePlayer;

    [SerializeField]
    Button _startButton;

    [SerializeField]
    Button _restartButton;

    [SerializeField]
    Button _finalButton;

    [SerializeField]
    GameObject _startScreen;

    [SerializeField]
    GameObject _restartScreen;

    [SerializeField]
    TMP_Text _finalScore;

    [SerializeField]
    TMP_Text _looseScore;

    [SerializeField]
    GameObject _finalScreen;

    [SerializeField]
    GameObject _optionScreen;

    [SerializeField]
    GameObject _grid;

    [SerializeField]
    GameObject _gridColisor;

    [SerializeField]
    GameObject _gridFloor;

    [SerializeField]
    GameObject _gridBonus;

    [SerializeField]
    GameObject _player;

    [SerializeField]
    TMP_Text _scoreText;

    [SerializeField]
    int _totalGrey = 1;

    [SerializeField]
    int _actualGrey = 0;

    [SerializeField]
    List<GameObject> _mapsPrefabs = new List<GameObject>();

    [SerializeField]
    Image _bgImage;

    [SerializeField]
    Transform _mapRespawnPoint;

    private Color _charColor;
    private Color _OBSColor;

    private float lastMov;
    private float _dashMovSpeed;

    private int score = 0;
    private int bonus = 0;
    private int lastPointResapwn;

    private Transform[,] _gridRef = new Transform[16, 16];
    private Transform[,] _colRef = new Transform[16, 16];
    private Transform[,] _floorRef = new Transform[16, 16];
    private Transform[,] _bonusRef = new Transform[5, 5];

    private bool[,] _gridBonusBool = new bool[5, 5];
    private bool[,] _gridChange = new bool[16, 16];

    private List<Transform> Destroy = new List<Transform>();
    private bool isFinished = false;
    private bool move = false;
    private int totalMaps = 0;
    private Direction _direction = Direction.Right;

    private void Start()
    {

        GameObject obj = Instantiate(_gridBonus, this.transform);

        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Transform t = obj.GetComponent<Transform>().GetChild(count);
                if (t.GetComponent<Renderer>().material.name == "Floor (Instance)")
                {
                    _gridBonusBool[i, j] = true;
                }
                else
                {
                    _gridBonusBool[i, j] = false;
                }
                count++;
            }
        }

        SINGLETON = this;

        _startButton.onClick.AddListener(Restart);
        _finalButton.onClick.AddListener(Restart);
        _restartButton.onClick.AddListener(Restart);


        lastMov = Time.time;

        InvokeRepeating("RespawnEnemy", 1, 4);

    }

    private void Initialize()
    {

        int audio = Random.RandomRange(0, 3);

        Camera.main.GetComponent<AudioSource>().loop = true;
        Camera.main.GetComponent<AudioSource>().clip = _inGame[audio];
        Camera.main.GetComponent<AudioSource>().Play();

        _startScreen.SetActive(false);

         GameObject obj =  Instantiate(_mapsPrefabs[totalMaps], _mapRespawnPoint);
        _grid = obj.GetComponent<MapConfig>()._grid;
        _gridColisor = obj.GetComponent<MapConfig>()._gridOBS;
        _gridFloor = obj.GetComponent<MapConfig>()._gridFloor;
        _bgImage.sprite = obj.GetComponent<MapConfig>()._character.sprite;
        _charColor = obj.GetComponent<MapConfig>()._charColor;
        _OBSColor= obj.GetComponent<MapConfig>()._OBSColor;

        isFinished = false;
        _totalGrey = 1;

        _dashMovSpeed = _movTime / 3f;

        Invoke("MountRefGrid", 1);
    }

    private void Restart()
    {

        foreach (var item in _enemylist)
        {
            GameObject en = item;
            Destroy(en);
        }

        _enemylist.Clear();
        _maxEnemies = 10;

        int audio = Random.RandomRange(0, 3);

        Camera.main.GetComponent<AudioSource>().clip = _inGame[audio];
        Camera.main.GetComponent<AudioSource>().Play();
        Camera.main.GetComponent<AudioSource>().loop = true;

        int count = _mapRespawnPoint.childCount;

        for (int i = 0; i < count; i++)
        {
            var child = _mapRespawnPoint.GetChild(i);
            Destroy(child.gameObject);
        }
        _startScreen.SetActive(false);
        _finalScreen.SetActive(false);
        _restartScreen.SetActive(false);
        _optionScreen.SetActive(false);

        totalMaps = 0;
        score = 0;
        _scoreText.text = " " + score;
        bonus = 0;

        GameObject obj = Instantiate(_mapsPrefabs[totalMaps], _mapRespawnPoint);
        _grid = obj.GetComponent<MapConfig>()._grid;
        _gridColisor = obj.GetComponent<MapConfig>()._gridOBS;
        _gridFloor = obj.GetComponent<MapConfig>()._gridFloor;
        _bgImage.sprite = obj.GetComponent<MapConfig>()._character.sprite;
        _charColor = obj.GetComponent<MapConfig>()._charColor;
        _OBSColor = obj.GetComponent<MapConfig>()._OBSColor;

        isFinished = false;
        _totalGrey = 1;

        Invoke("MountRefGrid", 1);
    }

    private async void Print()
    {


        Camera.main.GetComponent<AudioSource>().clip = _levelAudio;
        Camera.main.GetComponent<AudioSource>().Play();

        _player.SetActive(false);

        _player.SetActive(false);

        foreach (GameObject item in _enemylist)
        {
            item.SetActive(false);
        }

        int count = _grid.GetComponent<Transform>().childCount;
        for (int i = 0; i < count; i++)
        {
            var child = _grid.GetComponent<Transform>().GetChild(i);
            var name = child.GetComponent<Renderer>().material.name;
            if (name== "Floor (Instance)"|| name == "OBS (Instance)")
            {
                //Destroy.Add(child);
                child.GetComponent<Renderer>().enabled=false;
            }
            else {
                child.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
                child.name = "" + i;
            }

            var childCol = _gridColisor.GetComponent<Transform>().GetChild(i);
            childCol.GetComponent<Renderer>().enabled = false;

            var childFloor = _gridFloor.GetComponent<Transform>().GetChild(i);
            childFloor.GetComponent<Renderer>().enabled = false;

            await Task.Delay(_revealTime);
        }

        GameObject obj =_mapsPrefabs[totalMaps];

        totalMaps++;
        _maxEnemies += 3;

        _animEnding.SetTrigger("action");

        await Task.Delay(_finalDelay); ;

        if (totalMaps < _mapsPrefabs.Count)
        {
            Initialize();
        }
        else
        {
            _finalScreen.SetActive(true);
            Camera.main.GetComponent<AudioSource>().clip = _victoryAudio;
            Camera.main.GetComponent<AudioSource>().Play();
            _finalScore.text = "YOU SCORE IS: " + score;
        }

        DestroyV();
    }

    private async void Hide()
    {
        int count = _grid.GetComponent<Transform>().childCount;
        for (int i = 0; i < count; i++)
        {
            var child = _grid.GetComponent<Transform>().GetChild(i);

            child.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
            child.name = "" + i;

            await Task.Delay(_revealTime);
            //yield return new WaitForSeconds(0.01f);
        }

        _player.transform.position = _initialPosPlayer;
        _playerPos = new Vector2(7, 8);
        _player.SetActive(true);
        move = true;

        StopAllCoroutines();
    }

    void DestroyV()
    {
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                if (_gridRef[i, j].GetComponent<Renderer>().material.name == "Floor (Instance)")
                    _gridRef[i, j].GetComponent<Renderer>().enabled = false;
            }
        }
    }

    void MountRefGrid()
    {
        int value = 0;
        int grey = 0;

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                Transform t = _grid.GetComponent<Transform>().GetChild(value);
              _gridRef[i,j]= t ;
                value++;

                if (t.GetComponent<Renderer>().material.name == "Floor (Instance)")
                {
                    grey++;
                    _gridChange[i, j] = false;
                }
            }
        }

        value = 0;

        for (int k = 0; k < 16; k++)
        {
            for (int l = 0; l < 16; l++)
            {
                Transform t = _gridColisor.GetComponent<Transform>().GetChild(value);
                t.GetComponent<Renderer>().material.color=_OBSColor ;
                _colRef[k, l] = t;
                value++;

            }
        }


        value = 0;

        for (int m = 0; m < 16; m++)
        {
            for (int n = 0; n < 16; n++)
            {
                Transform t = _gridFloor.GetComponent<Transform>().GetChild(value);
                _floorRef[m, n] = t;
                value++;

            }
        }

        _totalGrey = grey;
        Hide();
    }

    private void Update()
    {

        if (_totalGrey==0 && !isFinished)
        {
            Print();
            isFinished = true;
            move = false;
        }

        if (!move)
        {
            return;
        }

        if (SwipeManager.swipeRight)
            _direction = Direction.Right;
        if (SwipeManager.swipeLeft)
            _direction = Direction.Left;
        if (SwipeManager.swipeUp)
            _direction = Direction.Up;
        if (SwipeManager.swipeDown)
            _direction = Direction.Down;

        switch (_direction)
        {
            case Direction.Right:    
                if (lastMov < Time.time)
                {
                    _player.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
                    lastMov = Time.time + _movTime;
                    if ((int)_playerPos.y + 1 < 16)
                    {
                        if (_gridRef[(int)_playerPos.x, (int)_playerPos.y + 1].GetComponent<Renderer>().material.name
                            == "Floor (Instance)")
                        {
                            _floorRef[(int)_playerPos.x, (int)_playerPos.y + 1].GetComponent<Renderer>().material.color =
                                _charColor;

                            _player.transform.position += new Vector3(1, 0, 0);

                            if (_gridChange[(int)_playerPos.x, (int)_playerPos.y + 1] == false)
                            {
                                _totalGrey--;
                                _gridChange[(int)_playerPos.x, (int)_playerPos.y + 1] = true;
                                score++;
                                _scoreText.text = " " + score;
                            }

                            _playerPos.y += 1;

                            if ((int)_playerPos.y + 1 < 16 && _gridRef[(int)_playerPos.x, (int)_playerPos.y + 1].GetComponent<Renderer>().material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x, (int)_playerPos.y + 1].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.y - 1 > -1 && _gridRef[(int)_playerPos.x, (int)_playerPos.y - 1].GetComponent<Renderer>().material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x, (int)_playerPos.y - 1].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.x - 1 > -1 && _gridRef[(int)_playerPos.x - 1, (int)_playerPos.y].GetComponent<Renderer>().material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x - 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.x + 1 < 16 && _gridRef[(int)_playerPos.x + 1, (int)_playerPos.y].GetComponent<Renderer>().material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x + 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;

                            _sourcePlayer.PlayOneShot(_movPlayer);

                        }
                        else
                        {
                            SwipeManager.swipeRight = false;
                            SwipeManager.swipeLeft = true;
                            _direction = Direction.Left;

                            _colRef[(int)_playerPos.x, (int)_playerPos.y + 1].GetComponent<Renderer>().enabled = true;
                        }
                    }

                }
                break;
            case Direction.Left:
                if (lastMov < Time.time)
                {
                    _player.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
                    lastMov = Time.time + _movTime;
                    if ((int)_playerPos.y - 1 > -1)
                    {
                        if (_gridRef[(int)_playerPos.x, (int)_playerPos.y - 1].GetComponent<Renderer>().material.name
                            == "Floor (Instance)")
                        {
                            _floorRef[(int)_playerPos.x, (int)_playerPos.y - 1].GetComponent<Renderer>().material.color =
                                _charColor;

                            _player.transform.position += new Vector3(-1, 0, 0);

                            if (_gridChange[(int)_playerPos.x, (int)_playerPos.y - 1] == false)
                            {
                                _totalGrey--;
                                _gridChange[(int)_playerPos.x, (int)_playerPos.y - 1] = true;
                                score++;
                                _scoreText.text = " " + score;
                            }

                            _playerPos.y -= 1;

                            if ((int)_playerPos.y + 1 < 16
                                && _gridRef[(int)_playerPos.x, (int)_playerPos.y + 1]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x, (int)_playerPos.y + 1].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.y - 1 > -1
                                && _gridRef[(int)_playerPos.x, (int)_playerPos.y - 1]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x, (int)_playerPos.y - 1].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.x - 1 > -1
                                && _gridRef[(int)_playerPos.x - 1, (int)_playerPos.y]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x - 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.x + 1 < 16
                                && _gridRef[(int)_playerPos.x + 1, (int)_playerPos.y]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x + 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;

                            _sourcePlayer.PlayOneShot(_movPlayer);

                        }
                        else
                        {
                            SwipeManager.swipeRight = true;
                            SwipeManager.swipeLeft = false;
                            _direction = Direction.Right;
                            _colRef[(int)_playerPos.x, (int)_playerPos.y - 1].GetComponent<Renderer>().enabled = true;
                        }
                    }
                }
                break;
            case Direction.Up:
                if (lastMov < Time.time)
                {
                    _player.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
                    lastMov = Time.time + _movTime;
                    if ((int)_playerPos.x - 1 > -1)
                    {
                        if (_gridRef[(int)_playerPos.x - 1, (int)_playerPos.y].GetComponent<Renderer>().material.name
                            == "Floor (Instance)")
                        {
                            _floorRef[(int)_playerPos.x - 1, (int)_playerPos.y].GetComponent<Renderer>().material.color =
                                _charColor;

                            _player.transform.position += new Vector3(0, 0, 1);

                            if (_gridChange[(int)_playerPos.x - 1, (int)_playerPos.y] == false)
                            {
                                _totalGrey--;
                                _gridChange[(int)_playerPos.x - 1, (int)_playerPos.y] = true;
                                score++;
                                _scoreText.text = " " + score;
                            }

                            _playerPos.x -= 1;

                            if ((int)_playerPos.y + 1 < 16
                                && _gridRef[(int)_playerPos.x, (int)_playerPos.y + 1]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x, (int)_playerPos.y + 1].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.y - 1 > -1
                                && _gridRef[(int)_playerPos.x, (int)_playerPos.y - 1]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x, (int)_playerPos.y - 1].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.x - 1 > -1
                                && _gridRef[(int)_playerPos.x - 1, (int)_playerPos.y]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x - 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.x + 1 < 16
                                && _gridRef[(int)_playerPos.x + 1, (int)_playerPos.y]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x + 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;

                            _sourcePlayer.PlayOneShot(_movPlayer);
                        }
                        else
                        {
                            SwipeManager.swipeUp = false;
                            SwipeManager.swipeDown = true;
                            _direction = Direction.Down;

                            _colRef[(int)_playerPos.x - 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;
                        }
                    }
                    else
                    {
                        SwipeManager.swipeUp = false;
                        SwipeManager.swipeDown = true;
                        _direction = Direction.Down;
                    }
                }
                break;
            case Direction.Down:
                if (lastMov < Time.time)
                {
                    _player.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
                    lastMov = Time.time + _movTime;
                    if ((int)_playerPos.x + 1 < 16)
                    {
                        if (_gridRef[(int)_playerPos.x + 1, (int)_playerPos.y].GetComponent<Renderer>().material.name
                            == "Floor (Instance)")
                        {
                            _floorRef[(int)_playerPos.x + 1, (int)_playerPos.y].GetComponent<Renderer>().material.color =
                                _charColor;

                            _player.transform.position += new Vector3(0, 0, -1);

                            if (_gridChange[(int)_playerPos.x + 1, (int)_playerPos.y] == false)
                            {
                                _totalGrey--;
                                _gridChange[(int)_playerPos.x + 1, (int)_playerPos.y] = true;
                                score++;
                                _scoreText.text = " " + score;
                            }

                            _playerPos.x += 1;

                            if ((int)_playerPos.y + 1 < 16
                                && _gridRef[(int)_playerPos.x, (int)_playerPos.y + 1]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x, (int)_playerPos.y + 1].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.y - 1 > -1
                                && _gridRef[(int)_playerPos.x, (int)_playerPos.y - 1]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x, (int)_playerPos.y - 1].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.x - 1 > -1
                                && _gridRef[(int)_playerPos.x - 1, (int)_playerPos.y]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x - 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;

                            if ((int)_playerPos.x + 1 < 16
                                && _gridRef[(int)_playerPos.x + 1, (int)_playerPos.y]
                                    .GetComponent<Renderer>()
                                    .material.name
                                == "OBS (Instance)")
                                _colRef[(int)_playerPos.x + 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;

                            _sourcePlayer.PlayOneShot(_movPlayer);

                        }
                        else
                        {
                            SwipeManager.swipeUp = true;
                            SwipeManager.swipeDown = false;
                            _direction = Direction.Up;
                            _colRef[(int)_playerPos.x + 1, (int)_playerPos.y].GetComponent<Renderer>().enabled = true;
                        }
                    }
                    else
                    {
                        SwipeManager.swipeUp = true;
                        SwipeManager.swipeDown = false;
                        _direction = Direction.Up;
                    }
                }
                break;
            default:
                break;
        }
        
        if (Input.GetKey(KeyCode.Escape))
        {
            _optionScreen.SetActive(true);
            _player.SetActive(false);
            move = false;
        }
    }

    public void Bonus()
    {

        _sourceBonus.Play();

        _animBonus.SetTrigger("action");

        int x =0;
        int y =0;

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {

                if (_gridBonusBool[i, j] == true)
                {
                    if (i + (int)_playerPos.x - 2 >= 0 && i + (int)_playerPos.x - 2 < 16 && j + (int)_playerPos.y - 2 >= 0 && j + (int)_playerPos.y - 2 < 16)
                    {
                        if (_gridRef[i + (int)_playerPos.x - 2, j + (int)_playerPos.y - 2].GetComponent<Renderer>().material.name == "Floor (Instance)")
                        {
                            _floorRef[i + (int)_playerPos.x - 2, j + (int)_playerPos.y - 2].GetComponent<Renderer>().material.color = _charColor;

                            if (_gridChange[i + (int)_playerPos.x - 2, j + (int)_playerPos.y - 2] == false)
                            {
                                _totalGrey--;
                                _gridChange[i + (int)_playerPos.x - 2, j + (int)_playerPos.y - 2] = true;
                                score++;
                                _scoreText.text = " " + score;
                            }
                        }

                        if (_gridRef[i + (int)_playerPos.x - 2, j + (int)_playerPos.y - 2].GetComponent<Renderer>().material.name == "OBS (Instance)")
                            _colRef[i + (int)_playerPos.x - 2, j + (int)_playerPos.y - 2].GetComponent<Renderer>().enabled = true;

                    }
                }
                y++;
            }
            x++;
        }

        score+=15;
    }

    void RespawnEnemy()
    {
        if (move)
        {
            int location = Random.Range(0, 3);

            if (location == lastPointResapwn)
            {
                RespawnEnemy();
                return;
            }
            else
            {
                lastPointResapwn = location;
            }

            int respawn = Random.Range(0,10);

            if (_enemylist.Count < _maxEnemies)
            {
                if (respawn > 2)
                {
                    GameObject obj = Instantiate(_prefabEnemy, _respawnEnemyPoint[location].transform.position, _respawnEnemyPoint[location].transform.rotation);
                    obj.GetComponent<EnemyBehavior>()._speed = (Random.Range(0.2f, 1));
                    _enemylist.Add(obj);
                }
                else
                {
                    GameObject obj = Instantiate(_prefabBonus, _respawnEnemyPoint[location].transform.position, _respawnEnemyPoint[location].transform.rotation);
                    obj.GetComponent<Bonus>()._speed = (Random.Range(0.2f, 1));

                    _enemylist.Add(obj);
                }
            }
            else
            {
                foreach (GameObject item in _enemylist)
                {
                    if (item.activeInHierarchy == false)
                    {
                        int loc = Random.Range(0, 3);
                        item.transform.position = _respawnEnemyPoint[loc].transform.position;
                        item.transform.rotation = _respawnEnemyPoint[loc].transform.rotation;

                        if (item.GetComponent<EnemyBehavior>())
                            item.GetComponent<EnemyBehavior>()._speed = (Random.Range(0.2f, 1));

                        if (item.GetComponent<Bonus>())
                            item.GetComponent<Bonus>()._speed = (Random.Range(0.2f, 1));

                        item.SetActive(true);
                    }
                }
            }
        }
    }

    public void Dead()
    {
        Camera.main.GetComponent<AudioSource>().loop=false;
        Camera.main.GetComponent<AudioSource>().clip = _lossAudio;
        Camera.main.GetComponent<AudioSource>().Play();

        _looseScore.text = "SCORE " + score;

        _player.transform.position = _initialPosPlayer;
        _playerPos = new Vector2(7, 8);
        _player.SetActive(false);
        move = false;
        _restartScreen.SetActive(true);
        foreach (GameObject item in _enemylist)
        {
            item.SetActive(false);
        }

    }

    public void BackMain()
    {
        Camera.main.GetComponent<AudioSource>().loop = true;
        Camera.main.GetComponent<AudioSource>().clip = _mainAudio;
        Camera.main.GetComponent<AudioSource>().Play();

        _startScreen.SetActive(true);
        _finalScreen.SetActive(false);
        _restartScreen.SetActive(false);
        _optionScreen.SetActive(false);

        _player.SetActive(false);

    }

    public void Play()
    {
        move = true;
        _player.SetActive(true);
        _optionScreen.SetActive(false);
    }
}
