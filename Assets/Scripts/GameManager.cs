using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public bool _isGameOver;
    [SerializeField]
    private AudioSource _audioSource;
    public static bool isPlayWithBot = true;

    // Start is called before the first frame update
    private void Start()
    {
        _isGameOver = false;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.LogError("_audiosource is null");
    }

    public void SetGameOver()
    {
        _isGameOver = true;
    }

    public void RestartGame()
    {
        _audioSource.Play();
        SceneManager.LoadScene(1); // Reloading the Game Scene
    }

    public void BackToMainMenu()
    {
        _audioSource.Play();
        SceneManager.LoadScene(0); // Loading MainMenu Scene
    }
}
