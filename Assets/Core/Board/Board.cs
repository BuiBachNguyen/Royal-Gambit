using NUnit.Framework;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] Tile tilePrefab;
    [SerializeField] BoardManager boardManager;
    [SerializeField] Tile[,] board;

    #region Singleton
    public static Board Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    void Start()
    {
        if (boardManager == null)
            boardManager = BoardManager.Instance;
        GenerateTile(boardManager);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void GenerateTile(BoardManager bm)
    {

        board = new Tile[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //if (i == j && j == 0) continue;
                Tile tile = Instantiate(tilePrefab, transform);
                tile.Initialize(bm, new BoardPosition(i, j), null, TileStatus.NoAct);
                board[i, j] = tile;
            }
        }

    }
    public void ChangeStatusTiles(List<Move> moves, TileStatus tileStatus)
    {

        if (moves == null)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j].ChangeStatus(TileStatus.NoAct);
                }
            }
            return;
        }

        //moves != null
        foreach (var move in moves)
        {
            // ✅ FIX: check cả âm và vượt 8
            if (move.to.x < 0 || move.to.x >= 8 || move.to.y < 0 || move.to.y >= 8)
                continue;

            board[move.to.x, move.to.y].ChangeStatus(tileStatus);
            if (DEBUG.isLogicDebuging || DEBUG.overviewDebug)
                Debug.Log("dổi hết toàn bộ sprite rồi nha");
        }

        if (DEBUG.isLogicDebuging || DEBUG.overviewDebug)
            Debug.Log("DONE with moves != null");
    }
}
