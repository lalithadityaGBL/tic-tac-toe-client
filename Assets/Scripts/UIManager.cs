using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _messageText;
    [SerializeField]
    private GameObject _gameOverPanel;
    [SerializeField]
    private GameManager _gameManager;
    private const string winMessage = " rocked the board today! ";

    // Start is called before the first frame update
    void Start()
    {
        _gameOverPanel.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("_gameManager is null");

    }

    public void DisplayGameOver(Mark winner)
    {
        GameOverSequence(winner);
    }
    public void DisplayMessage(string msg)
    {
        _messageText.text = msg;
    }

    public string GetDisplayMessage()
    {
        return _messageText.text;
    }

    void GameOverSequence(Mark winner)
    {
        _gameOverPanel.SetActive(true);
        StartCoroutine(GameOverFlicker(winner));
        _gameManager.SetGameOver();
    }

    private IEnumerator GameOverFlicker(Mark winner)
    {
        while (true)
        {
            _gameOverText.text = winner + winMessage;
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
