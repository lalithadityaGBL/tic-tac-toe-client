using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using System.Collections;

public class Board : MonoBehaviour
{
    [SerializeField]
    private LayerMask _boxesLayerMask;
    [SerializeField]
    private float _touchRadius;
    [SerializeField]
    private Sprite _spriteX;
    [SerializeField]
    private Sprite _spriteO;
    [SerializeField]
    private AudioClip _celebrationAudio, _turnAudio;
    private AudioSource _audioSource;
    private Camera sceneCamera;
    private UIManager _uiManager;
    private GameManager _gameManager;
    private int _counter = 0;
    public static bool _canPlay = true;
    public static Mark currentMark;
    public Mark[] marks;
    public Vector3Int cells = Vector3Int.zero;
    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();
        if (_uiManager == null)
            Debug.LogError("_uiManager is NULl");
        if (_gameManager == null)
            Debug.LogError("_gameManager is NULl");
        if (_audioSource == null)
            Debug.Log("_audioSource is NULL");
        sceneCamera = Camera.main;
        currentMark = Mark.X;
        //Dynimically allocating marks array
        marks = new Mark[9];
        for (int i = 0; i < 9; i++)
            marks[i] = Mark.None;
        // Multiplayer online setup
        if (GameManager.isPlayWithBot == false)
        {
            NetworkManager.GetNetworkManager.EstablishWebSocketCommunication();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager._isGameOver == true)
            return;
        if (GameManager.isPlayWithBot == false && _canPlay && Input.GetMouseButtonUp(0))
            MakePlayerMove();
        if (GameManager.isPlayWithBot == false && !_canPlay)
            MakeOpponentsMove();
        if (GameManager.isPlayWithBot && _canPlay && Input.GetMouseButtonUp(0))
            PlayOneRoundWithBot();
        if (_canPlay && _uiManager.GetDisplayMessage() != "Your Turn!")
            _uiManager.DisplayMessage("Your Turn!");
        else if (!_canPlay && _uiManager.GetDisplayMessage() != "Opponent's Turn")
            _uiManager.DisplayMessage("Opponent's Turn");
    }

    private void MakeOpponentsMove()
    {
        if (PayloadBuffer.payloads.Count <= 0)
            return;
        JsonClass.JsonMessageClass payload = PayloadBuffer.payloads.Peek();
        if (payload.Action == "move")
        {
            Box box = transform.GetChild(payload.Move).GetComponent<Box>();
            UpdateBoxState(box, box.gameObject);
            _canPlay = true;
        }
        else if (payload.Action == "close")
        {
            _uiManager.DisplayMessage("Opponent lost connection");
        }
        PayloadBuffer.payloads.Dequeue();
    }

    private void MakePlayerMove()
    {
        Vector2 touchPosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D chosenEntity = Physics2D.OverlapCircle(touchPosition, _touchRadius, _boxesLayerMask);
        Box box = chosenEntity.GetComponent<Box>();
        if (chosenEntity != null)
        {
            UpdateBoxState(box, chosenEntity.gameObject);
            ServerCommunication.SendMove(box.index);
        }
        _canPlay = false;
    }

    void PlayOneRoundWithBot()
    {
        if (_canPlay)
        {
            Vector2 touchPosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
            Collider2D chosenEntity = Physics2D.OverlapCircle(touchPosition, _touchRadius, _boxesLayerMask);
            if (chosenEntity != null)
            {
                UpdateBoxState(chosenEntity.GetComponent<Box>(), chosenEntity.gameObject);
                _canPlay = false;
            }
        }
        if (_canPlay == false)
        {
            StartCoroutine(ComputerThinkAndPlayRoutine());
        }
    }

    private void UpdateBoxState(Box box, GameObject boxGameObject)
    {
        boxGameObject.transform.localScale = Vector3.zero;
        boxGameObject.transform.DOScale(1.0f, 0.27f).SetEase(Ease.OutBack);
        if (box._isMarked)
            return;
        marks[box.index] = currentMark;
        var sprite = currentMark == Mark.X ? _spriteX : _spriteO;
        box.MarkTheBox(currentMark, sprite);
        //Decide game state and switch player
        string winner = GetWinner(marks);
        if (winner == "X" || winner == "O")
        {
            FindAndFade(cells.x, cells.y, cells.z);
            _audioSource.clip = _celebrationAudio;
            _audioSource.Play();
            _uiManager.DisplayGameOver(currentMark);
        }
        else if (_counter >= 8)
            _uiManager.DisplayGameOver(Mark.None);
        else
            SwitchPlayer();
    }

    private void UpdateBoxBot()
    {
        int boxIndex = BestMove(marks.ToArray());
        Box box = transform.GetChild(boxIndex).GetComponent<Box>();
        if (box == null)
            Debug.LogError("box is null");
        if (box._isMarked)
            return;
        marks[box.index] = currentMark;
        var sprite = currentMark == Mark.X ? _spriteX : _spriteO;
        box.MarkTheBox(currentMark, sprite);
        //Decide game state and switch player
        string winner = GetWinner(marks);
        if (winner == "X" || winner == "O")
        {
            FindAndFade(cells.x, cells.y, cells.z);
            _audioSource.clip = _celebrationAudio;
            _audioSource.Play();
            _uiManager.DisplayGameOver(currentMark);
        }
        else if (_counter >= 8)
            _uiManager.DisplayGameOver(Mark.None);
        else
            SwitchPlayer();
    }

    private String GetWinner(Mark[] board)
    {
        //checking all rows
        for (int i = 0; i <= 6; i += 3)
        {
            if (board[i] != Mark.None && board[i] == board[i + 1] && board[i + 1] == board[i + 2])
            {
                cells = new Vector3Int(i, i + 1, i + 2);
                return board[i] == Mark.X ? "X" : "O";
            }

        }
        //checking all columns
        for (int i = 0; i <= 2; i++)
        {
            if (board[i] != Mark.None && board[i] == board[i + 3] && board[i + 3] == board[i + 6])
            {
                cells = new Vector3Int(i, i + 3, i + 6);
                return board[i] == Mark.X ? "X" : "O";
            }
        }
        //checking left diagonal
        if (board[0] != Mark.None && board[0] == board[4] && board[4] == board[8])
        {
            cells = new Vector3Int(0, 4, 8);
            return board[0] == Mark.X ? "X" : "O";
        }
        //checking right diagonal
        if (board[2] != Mark.None && board[2] == board[4] && board[4] == board[6])
        {
            cells = new Vector3Int(2, 4, 6);
            return board[2] == Mark.X ? "X" : "O";
        }
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == Mark.None)
                return "XO";
        }
        return "Tie";
    }

    public void FindAndFade(int a, int b, int c)
    {
        int[] list = { a, b, c };
        for (int i = 0; i < 3; i++)
        {
            Box box = transform.GetChild(list[i]).GetComponent<Box>();
            box.gameObject.GetComponent<SpriteRenderer>().DOColor(Color.green, 1.0f);
        }
    }

    private void SwitchPlayer()
    {
        _counter++;
        _audioSource.clip = _turnAudio;
        _audioSource.Play();
        currentMark = currentMark == Mark.X ? Mark.O : Mark.X;
    }

    int BestMove(Mark[] board)
    {
        int bestScore = Int32.MinValue;
        int bestMove = 0;
        for (int i = 0; i < 9; i++)
        {
            if (board[i] != Mark.None)
                continue;
            board[i] = Mark.O;
            int score = MiniMax(board, false);
            board[i] = Mark.None;
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = i;
            }
        }
        return bestMove;
    }

    int MiniMax(Mark[] board, bool isMaximizing)
    {
        String winner = GetWinner(board);
        if (winner == "Tie")
            return 0;
        else if (winner == "X")
            return -1;
        else if (winner == "O")
            return 1;

        if (isMaximizing)
        {
            int bestScore = Int32.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != Mark.None)
                    continue;
                board[i] = Mark.O;
                int score = MiniMax(board, false);
                board[i] = Mark.None;
                bestScore = Math.Max(score, bestScore);
            }
            return bestScore;
        }
        else
        {
            int bestScore = Int32.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != Mark.None)
                    continue;
                board[i] = Mark.X;
                int score = MiniMax(board, true);
                board[i] = Mark.None;
                bestScore = Math.Min(score, bestScore);
            }
            return bestScore;
        }
    }

    IEnumerator ComputerThinkAndPlayRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        UpdateBoxBot();
        _canPlay = true;
    }
}
