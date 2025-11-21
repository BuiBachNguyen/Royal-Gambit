using NUnit.Framework;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] Tile tilePrefab;
    [SerializeField] BoardManager boardManager;
    [SerializeField] Tile[,] board;

    [SerializeField] private Piece kingWhite;
    [SerializeField] private Piece queenWhite;
    [SerializeField] private Piece rookWhite;
    [SerializeField] private Piece bishopWhite;
    [SerializeField] private Piece knightWhite;
    [SerializeField] private Piece pawnWhite;

    [SerializeField] private Piece kingBlack;
    [SerializeField] private Piece queenBlack;
    [SerializeField] private Piece rookBlack;
    [SerializeField] private Piece bishopBlack;
    [SerializeField] private Piece knightBlack;
    [SerializeField] private Piece pawnBlack;

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
        board = new Tile[8, 8];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Tile tile = Instantiate(tilePrefab, transform);
                tile.Initialize(bm, new BoardPosition(i, j), null, TileStatus.NoAct);
                board[i, j] = tile;
            }
        }

        //SpawnPieces();
        LoadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
    }


    void CreatePiece(Piece prefab, int row, int col)
    {
        Piece p = Instantiate(prefab);
        Tile tile = board[row, col];

        tile.PlacePiece(p);

        p.MoveTo(tile, false);
    }

    void SpawnPieces()
    {
        // Black major pieces (row 0)
        CreatePiece(rookBlack, 0, 0);
        CreatePiece(knightBlack, 0, 1);
        CreatePiece(bishopBlack, 0, 2);
        CreatePiece(queenBlack, 0, 3);
        CreatePiece(kingBlack, 0, 4);
        CreatePiece(bishopBlack, 0, 5);
        CreatePiece(knightBlack, 0, 6);
        CreatePiece(rookBlack, 0, 7);

        // Black pawns (row 1)
        for (int j = 0; j < 8; j++)
            CreatePiece(pawnBlack, 1, j);

        // White pawns (row 6)
        for (int j = 0; j < 8; j++)
            CreatePiece(pawnWhite, 6, j);

        // White major pieces (row 7)
        CreatePiece(rookWhite, 7, 0);
        CreatePiece(knightWhite, 7, 1);
        CreatePiece(bishopWhite, 7, 2);
        CreatePiece(queenWhite, 7, 3);
        CreatePiece(kingWhite, 7, 4);
        CreatePiece(bishopWhite, 7, 5);
        CreatePiece(knightWhite, 7, 6);
        CreatePiece(rookWhite, 7, 7);
    }



    public void ChangeStatusTiles(List<Move> moves, TileStatus tileStatus)
    {
        ResetTiles();

        if (moves == null)
        {
            return;
        }

        //moves != null
        foreach (var move in moves)
        {
            //Limit border
            if (move.to.x < 0 || move.to.x >= 8 || move.to.y < 0 || move.to.y >= 8)
                continue;

            board[move.to.x, move.to.y].ChangeStatus(tileStatus);
        }
    }

    public void ResetTiles()
    {
        for(int i = 0;i < 8;i++)
        {
            for(int j = 0; j < 8;j++)
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

        int row = 7;
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
        Piece prefab = GetPrefabFromSymbol(symbol);
        if (prefab == null)
            return;

        CreatePiece(prefab, row, col);
    }

    Piece GetPrefabFromSymbol(char c)
    {
        bool isWhite = char.IsUpper(c);
        char p = char.ToLower(c);

        return p switch
        {
            'k' => isWhite ? kingWhite : kingBlack,
            'q' => isWhite ? queenWhite : queenBlack,
            'r' => isWhite ? rookWhite : rookBlack,
            'b' => isWhite ? bishopWhite : bishopBlack,
            'n' => isWhite ? knightWhite : knightBlack,
            'p' => isWhite ? pawnWhite : pawnBlack,
            _ => null
        };
    }
}
