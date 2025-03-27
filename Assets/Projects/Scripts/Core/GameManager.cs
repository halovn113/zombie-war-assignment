using System.Collections;
using KHCore;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private MapSpawner mapSpawner;
    [SerializeField] private LevelSO levelSO;
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private AudioManager audioManager;

    private GAME_STATE state = GAME_STATE.NONE;
    private GameScore _gameScore;
    private float _currentTimer = 0f;
    private int _currentLevel = 0;
    private int _currentScore = 0;
    private GameObject _currentMap;

    void Awake()
    {
        ServiceLocator.Register(this);
        gameStateManager.OnStateChanged += HandelGameStateChanged;
        _gameScore = new GameScore();
        gameStateManager.ChangeState(GAME_STATE.MENU);
    }

    void Start()
    {

    }

    private void HandelGameStateChanged(GAME_STATE state)
    {
        this.state = state;
        switch (state)
        {
            case GAME_STATE.MENU:
                Menu();
                break;
            case GAME_STATE.LOAD_MAP:
                LoadMap();
                break;
            case GAME_STATE.START_GAME:
                StartNewGame();
                break;
            case GAME_STATE.GAME:
                break;
            case GAME_STATE.GAME_OVER:
                GameOver();
                break;
            case GAME_STATE.COMPLETE:
                LevelComplete();
                break;
        }
    }

    void Update()
    {
        if (state == GAME_STATE.GAME)
        {
            if (_currentTimer > 0)
            {
                _currentTimer -= Time.deltaTime;
                uIManager.GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>().UpdateTime(_currentTimer);
            }
            else
            {
                _currentTimer = 0f;
                uIManager.GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>().UpdateTime(_currentTimer);
                gameStateManager.ChangeState(GAME_STATE.COMPLETE);
            }
        }
    }

    public void LoadMap()
    {
        if (_currentMap != null)
            Destroy(_currentMap);
        uIManager.GetUI(UIEnum.LOADING).gameObject.SetActive(true);
        StartCoroutine(LoadMapAsync(_currentLevel));
    }

    IEnumerator LoadMapAsync(int levelIndex)
    {
        _currentMap = Instantiate(levelSO.levelInformation[levelIndex].mapPrefab);
        yield return null;
        _currentMap.transform.position = Vector3.zero;
        uIManager.GetUI(UIEnum.LOADING).gameObject.SetActive(false);
        gameStateManager.ChangeState(GAME_STATE.START_GAME);
    }

    public void StartNewGame()
    {
        uIManager.HideUI(UIEnum.TITLE);
        uIManager.ShowUI(UIEnum.GAMEPLAY);
        _currentTimer = levelSO.levelInformation[_currentLevel].time;
        _currentScore = 0;
        _gameScore.ResetScore();
        var uiGameplay = uIManager.GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>();
        uiGameplay.SetUIState(true);
        uiGameplay.UpdateTime(_currentTimer);
        uiGameplay.UpdateScore(0);
        audioManager.PlayMusic(BGM.GAME, true);

        // spawn the map
        mapSpawner.Clear();
        mapSpawner.StartSpawning(player.transform, levelSO.levelInformation[_currentLevel]);
        player.ResetStatus();
        player.transform.position = mapSpawner.playerSpawn.position;
        gameStateManager.ChangeState(GAME_STATE.GAME);
    }

    private void Menu()
    {
        uIManager.ShowUI(UIEnum.TITLE);
        uIManager.HideUI(UIEnum.GAMEPLAY);
        uIManager.HideUI(UIEnum.LOADING);
        audioManager.StopBGM();
    }

    public void UpdateGameScore()
    {
        _gameScore.UpdateScore();
        _currentScore = _gameScore.GetCurrentScore();
        uIManager.GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>().UpdateScore(_currentScore);
    }

    private void GameOver()
    {
        var uiGameplay = uIManager.GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>();
        uiGameplay.SetUIState(false);
        uiGameplay.ShowResult(false, _currentScore);
    }

    private void LevelComplete()
    {
        var uiGameplay = uIManager.GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>();
        uiGameplay.SetUIState(false);
        uiGameplay.ShowResult(true, _currentScore);
        player.GetComponent<PlayerAnimation>().PlayIdle();
    }

    public void NextLevel()
    {
        _currentLevel += 1;
        if (_currentLevel >= levelSO.levelInformation.Length)
        {
            gameStateManager.ChangeState(GAME_STATE.COMPLETE);
        }
        else
        {
            gameStateManager.ChangeState(GAME_STATE.LOAD_MAP);
        }
    }
}
