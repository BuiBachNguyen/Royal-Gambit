using NUnit.Framework;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private PiecePrefabLibrary prefabLibrary;
    public PiecePrefabLibrary PrefabLibrary => prefabLibrary;

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
        GenerateBoard(boardManager);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void GenerateBoard(BoardManager bm)
    {
        board = new Tile[BoardManager.BOARD_SIZE, BoardManager.BOARD_SIZE];

        for (int i = 0; i < BoardManager.BOARD_SIZE; i++)
        {
            for (int j = 0; j < BoardManager.BOARD_SIZE; j++)
            {
                Tile tile = Instantiate(tilePrefab, transform);
                tile.Initialize(bm, new BoardPosition(i, j), null, TileStatus.NoAct);
                board[i, j] = tile;
            }
        }

        //SpawnPieces();
        LoadFEN("RNBQKBNR/PPPPPPPP/8/8/8/8/pppppppp/rnbqkbnr w KQkq - 0 1");
    }

    void CreatePiece(PieceType type, PlayerColor color, int row, int col)
    {
        Piece piece = Instantiate(prefabLibrary.GetPrefab(type, color));
        if (piece == null) return;

        Tile tile = board[row, col];
        tile.PlacePiece(piece);
        piece.MoveTo(tile, false);
    }


    public void ChangeStatusTiles(List<Move> moves, TileStatus tileStatus)
    {
        ResetTiles();

        if (moves == null) return;

        foreach (var move in moves)
        {
            if (!move.to.InBounds()) continue;
            board[move.to.x, move.to.y].ChangeStatus(tileStatus);
        }
    }

    public void ResetTiles()
    {
        for(int i = 0;i < BoardManager.BOARD_SIZE;i++)
        {
            for(int j = 0; j < BoardManager.BOARD_SIZE;j++)
            {
                board[i, j].ChangeStatus(TileStatus.NoAct);
            }    
        }    
    }


    //TRY USING FEN
    public void LoadFEN(string fen)
    {
        string[] parts = fen.Split(' ');
        string boardData = parts[0];

        int row = BoardManager.BOARD_SIZE - 1;
        int col = 0;

        foreach (char c in boardData)
        {
            if (c == '/')
            {
                row--;
                col = 0;
                continue;
            }

            if (char.IsDigit(c))
            {
                col += (c - '0');
            }
            else
            {
                PlacePieceFromSymbol(c, row, col);
                col++;
            }
        }
    }

    void PlacePieceFromSymbol(char symbol, int row, int col)
    {
        bool isWhite = char.IsUpper(symbol);
        char lower = char.ToLower(symbol);

        PieceType type = lower switch
        {
            'p' => PieceType.Pawn,
            'r' => PieceType.Rook,
            'n' => PieceType.Knight,
            'b' => PieceType.Bishop,
            'q' => PieceType.Queen,
            'k' => PieceType.King,
            _ => throw new System.Exception("Unknown piece symbol: " + symbol)
        };

        CreatePiece(type, isWhite ? PlayerColor.White : PlayerColor.Black, row, col);
    }

    public Tile GetTile(int x, int y)
    {
        return board[x, y];
    }


}

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}
