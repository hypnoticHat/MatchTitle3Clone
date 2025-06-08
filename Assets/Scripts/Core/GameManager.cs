using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum GameState { Init, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; }

    public delegate void GameStateChanged(GameState newState);
    public static event GameStateChanged OnGameStateChanged;

    public AudioClip musicClip;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Slider progressBar; 
    private float musicDuration;
    private float musicStartTime;
    private TileSpawner tileSpawner;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            State = GameState.Init;
        }
        else
        {
            Destroy(gameObject);
        }
        tileSpawner = FindObjectOfType<TileSpawner>();

    }

    private void Update()
    {
        if (State == GameState.Playing)
        {
            UpdateProgressBar();
        }
    }

    public void StartGame()
    {
        if (State != GameState.Init && State != GameState.GameOver) return;

        Time.timeScale = 0f;

        ScoreController.Instance?.ResetScore();
        TileManager.Instance?.ClearZone();
        EffectController.Instance?.ClearEffect();

        // G?i countdown trý?c khi th?c s? b?t ð?u game
        StartCoroutine(CountdownCoroutine(() =>
        {
            State = GameState.Playing;
            AudioManager.Instance?.PlayMusic(musicClip);
            musicDuration = musicClip.length;
            musicStartTime = Time.time;

            if (progressBar != null)
                progressBar.value = 0f;

            Time.timeScale = 1f;
            OnGameStateChanged?.Invoke(State);
        }));
    }


    private void UpdateProgressBar()
    {
        if (progressBar == null) return;

        float elapsed = Time.time - musicStartTime;
        float progress = Mathf.Clamp01(elapsed / musicDuration);

        progressBar.value = progress;

        if (progress >= 1f)
        {
            EndGame();
        }
    }

    public void PauseGame()
    {
        if (State != GameState.Playing) return;

        State = GameState.Paused;
        Time.timeScale = 0f;
        AudioManager.Instance.PauseMusic();
        OnGameStateChanged?.Invoke(State);
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;

        State = GameState.Playing;
        Time.timeScale = 0f;

        TogglePausePanel(false);

        StartCoroutine(CountdownCoroutine(() =>
        {
            Time.timeScale = 1f;
            AudioManager.Instance.ResumeMusic();
            OnGameStateChanged?.Invoke(State);
        }));
    }


    public void EndGame()
    {
        if (State != GameState.Playing) return;

        State = GameState.GameOver;
        Time.timeScale = 1f;
        AudioManager.Instance?.PauseMusic();

        if (progressBar != null)
            progressBar.value = 1f;

        OnGameStateChanged?.Invoke(State);
        TogglePausePanel(true);
    }

    public void ResetGame()
    {
        State = GameState.Init;
        Time.timeScale = 1f;

        TileManager.Instance?.ClearZone();
        ScoreController.Instance?.ResetScore();
        EffectController.Instance?.ClearEffect();
        AudioManager.Instance?.PauseMusic();
        PoolManager.Instance.normalTilePool.ClearPool();
        PoolManager.Instance.longTilePool.ClearPool();
        TogglePausePanel(false);
        if (progressBar != null)
            progressBar.value = 0f;

        if (tileSpawner != null)
        {
            tileSpawner.ResetSpawner();
        }

        OnGameStateChanged?.Invoke(State);
    }

    public void TogglePausePanel(bool show)
    {
        if (pausePanel != null)
            pausePanel.SetActive(show);
    }

    private IEnumerator CountdownCoroutine(System.Action onComplete)
    {
        if (countdownText == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            countdownText.transform.localScale = Vector3.one * 2f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * 2f;
                float scale = Mathf.Lerp(2f, 1f, t);
                countdownText.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.2f);
        }

        countdownText.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

}
