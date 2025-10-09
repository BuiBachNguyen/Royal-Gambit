using System.Collections.Generic;
using UnityEngine;

// Main board manager: keeps 8x8 piece data, handles turn, generates legal moves,
// checks for check/checkmate, and tracks last move for en passant.
public class BoardManager : MonoBehaviour
{
    /*
    

    //// mapping from piece object to its script is via pieces array; 
    //// you can also maintain GameObject prefab mapping
    //[Header("Prefabs (assign in Inspector)")]
    //public GameObject pawnPrefab;
    //public GameObject rookPrefab;
    //public GameObject knightPrefab;
    //public GameObject bishopPrefab;
    //public GameObject queenPrefab;
    //public GameObject kingPrefab;

    //[Header("Board Visuals")]
    //public Transform piecesParent; // parent transform to spawn piece GameObjects under
    //public Vector3 bottomLeftWorld; // world position of tile (0,0)
    //public float tileSize = 1f;

    //[HideInInspector] public Move lastMove = null;

    //public PlayerColor currentTurn = PlayerColor.White;



    //public Piece GetPieceObjectAt(BoardPosition pos)
    //{
    //    return GetPieceAt(pos);
    //}

    //public Vector3 TileToWorld(BoardPosition pos)
    //{
    //    return bottomLeftWorld + new Vector3(pos.x * tileSize, 0, pos.y * tileSize);
    //}

    //// Create standard initial position
    //public void SetupStandardPosition()
    //{
    //    ClearBoard();

    //    // pawns
    //    for (int x = 0; x < 8; x++)
    //    {
    //        SpawnPiece(pawnPrefab, PlayerColor.White, new BoardPosition(x, 1));
    //        SpawnPiece(pawnPrefab, PlayerColor.Black, new BoardPosition(x, 6));
    //    }

    //    // other pieces
    //    SpawnPiece(rookPrefab, PlayerColor.White, new BoardPosition(0, 0));
    //    SpawnPiece(knightPrefab, PlayerColor.White, new BoardPosition(1, 0));
    //    SpawnPiece(bishopPrefab, PlayerColor.White, new BoardPosition(2, 0));
    //    SpawnPiece(queenPrefab, PlayerColor.White, new BoardPosition(3, 0));
    //    SpawnPiece(kingPrefab, PlayerColor.White, new BoardPosition(4, 0));
    //    SpawnPiece(bishopPrefab, PlayerColor.White, new BoardPosition(5, 0));
    //    SpawnPiece(knightPrefab, PlayerColor.White, new BoardPosition(6, 0));
    //    SpawnPiece(rookPrefab, PlayerColor.White, new BoardPosition(7, 0));

    //    SpawnPiece(rookPrefab, PlayerColor.Black, new BoardPosition(0, 7));
    //    SpawnPiece(knightPrefab, PlayerColor.Black, new BoardPosition(1, 7));
    //    SpawnPiece(bishopPrefab, PlayerColor.Black, new BoardPosition(2, 7));
    //    SpawnPiece(queenPrefab, PlayerColor.Black, new BoardPosition(3, 7));
    //    SpawnPiece(kingPrefab, PlayerColor.Black, new BoardPosition(4, 7));
    //    SpawnPiece(bishopPrefab, PlayerColor.Black, new BoardPosition(5, 7));
    //    SpawnPiece(knightPrefab, PlayerColor.Black, new BoardPosition(6, 7));
    //    SpawnPiece(rookPrefab, PlayerColor.Black, new BoardPosition(7, 7));
    //}

    //private void ClearBoard()
    //{
    //    // destroy existing piece objects
    //    for (int x = 0; x < 8; x++)
    //        for (int y = 0; y < 8; y++)
    //        {
    //            var p = pieces[x, y];
    //            if (p != null) Destroy(p.gameObject);
    //            pieces[x, y] = null;
    //        }
    //    lastMove = null;
    //}

    //// spawn helper
    //private void SpawnPiece(GameObject prefab, PlayerColor color, BoardPosition pos)
    //{
    //    var go = Instantiate(prefab, TileToWorld(pos), Quaternion.identity, piecesParent);
    //    var piece = go.GetComponent<Piece>();
    //    if (piece == null)
    //    {
    //        Debug.LogError("Prefab missing Piece script");
    //        Destroy(go);
    //        return;
    //    }
    //    piece.Initialize(this, color, pos);
    //    SetPieceAt(pos, piece);
    //}

    //// returns all legal moves for a color (filters out moves that leave own king in check)
    //public List<Move> GenerateAllLegalMoves(PlayerColor forColor)
    //{
    //    var all = new List<Move>();
    //    for (int x = 0; x < 8; x++)
    //        for (int y = 0; y < 8; y++)
    //        {
    //            var p = pieces[x, y];
    //            if (p != null && p.color == forColor)
    //            {
    //                var pseudo = p.GeneratePseudoLegalMoves();
    //                foreach (var m in pseudo)
    //                {
    //                    if (WouldBeLegal(m)) all.Add(m);
    //                }
    //            }
    //        }
    //    return all;
    //}

    //// Simulate move and check if own king is left in check
    //public bool WouldBeLegal(Move m)
    //{
    //    // clone minimal board state: we will perform move, then check
    //    var backupFrom = GetPieceAt(m.from);
    //    var backupTo = GetPieceAt(m.to);
    //    var backupLast = lastMove;
    //    // execute (minimal)
    //    SetPieceAt(m.from, null);
    //    SetPieceAt(m.to, backupFrom);
    //    backupFrom.pos = m.to;

    //    // special: en passant capture remove captured pawn behind to
    //    Piece epCaptured = null;
    //    if (m.isEnPassant)
    //    {
    //        var epPos = new BoardPosition(m.to.x, m.from.y);
    //        epCaptured = GetPieceAt(epPos);
    //        SetPieceAt(epPos, null);
    //    }

    //    bool inCheck = IsKingInCheck(backupFrom.color);

    //    // rollback
    //    SetPieceAt(m.from, backupFrom);
    //    SetPieceAt(m.to, backupTo);
    //    backupFrom.pos = m.from;
    //    if (m.isEnPassant && epCaptured != null)
    //    {
    //        var epPos = new BoardPosition(m.to.x, m.from.y);
    //        SetPieceAt(epPos, epCaptured);
    //    }
    //    lastMove = backupLast;
    //    return !inCheck;
    //}

    //public bool IsKingInCheck(PlayerColor kingColor)
    //{
    //    // find king
    //    BoardPosition kingPos = new BoardPosition(-1, -1);
    //    for (int x = 0; x < 8; x++)
    //        for (int y = 0; y < 8; y++)
    //        {
    //            var p = pieces[x, y];
    //            if (p is King && p.color == kingColor) kingPos = new BoardPosition(x, y);
    //        }
    //    if (!kingPos.InBounds()) return false; // should not happen

    //    PlayerColor opp = kingColor == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
    //    // for each opponent piece, generate its pseudo moves and see if any capture king
    //    for (int x = 0; x < 8; x++)
    //        for (int y = 0; y < 8; y++)
    //        {
    //            var p = pieces[x, y];
    //            if (p != null && p.color == opp)
    //            {
    //                var pseudo = p.GeneratePseudoLegalMoves();
    //                foreach (var m in pseudo)
    //                {
    //                    if (m.to == kingPos) return true;
    //                }
    //            }
    //        }
    //    return false;
    //}

    //// Is a given square attacked by side (used in castling)
    //public bool IsSquareAttacked(BoardPosition square, PlayerColor attacker)
    //{
    //    for (int x = 0; x < 8; x++)
    //        for (int y = 0; y < 8; y++)
    //        {
    //            var p = pieces[x, y];
    //            if (p != null && p.color == attacker)
    //            {
    //                var pseudo = p.GeneratePseudoLegalMoves();
    //                foreach (var m in pseudo)
    //                    if (m.to == square) return true;
    //            }
    //        }
    //    return false;
    //}

    //// Apply move publicly (after legality checked)
    //public void ApplyMove(Move m)
    //{
    //    var mover = GetPieceAt(m.from);
    //    if (mover == null) return;

    //    // handle captured object's destruction inside Piece.DoMove
    //    mover.DoMove(m);

    //    lastMove = m;
    //    currentTurn = currentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
    //}

    //// Promotion helper: replace pawn with given promotion piece (type indicated)
    //public void PromotePawn(Piece pawn, Piece promotionPrototype)
    //{
    //    // instantiate new piece of prototype type and set at pawn.pos
    //    var protoType = promotionPrototype.GetType();
    //    GameObject prefab = null;
    //    if (protoType == typeof(Queen)) prefab = queenPrefab;
    //    else if (protoType == typeof(Rook)) prefab = rookPrefab;
    //    else if (protoType == typeof(Bishop)) prefab = bishopPrefab;
    //    else if (protoType == typeof(Knight)) prefab = knightPrefab;
    //    else prefab = queenPrefab;

    //    var go = Instantiate(prefab, TileToWorld(pawn.pos), Quaternion.identity, piecesParent);
    //    var newPiece = go.GetComponent<Piece>();
    //    newPiece.Initialize(this, pawn.color, pawn.pos);
    //    SetPieceAt(pawn.pos, newPiece);
    //    Destroy(pawn.gameObject);
    //}

    //// Helper to find king position
    //public BoardPosition FindKing(PlayerColor color)
    //{
    //    for (int x = 0; x < 8; x++)
    //        for (int y = 0; y < 8; y++)
    //        {
    //            var p = pieces[x, y];
    //            if (p is King && p.color == color) return new BoardPosition(x, y);
    //        }
    //    return new BoardPosition(-1, -1);
    //}*/
    #region Singleton
    public static BoardManager Instance { get; private set; }

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

    [SerializeField] Piece[,] pieces = new Piece[8, 8];
    Piece pickingPiece;

    #region Getter setter

    public Piece PickingPiece
    {
        get { return pickingPiece; }
        set { pickingPiece = value; }
    }

    #endregion

    public void HandleMove(Tile targetTile, Move move)
    {
        if (pickingPiece == null) return;

        pickingPiece.MoveTo(targetTile);
        SetPieceAt(targetTile.GetBoardPotition(), pickingPiece);

        pickingPiece.Tile.Highlight(false);
        pickingPiece = null;

    }    

    public void SetPieceAt(BoardPosition pos, Piece p)
    {
        if (!pos.InBounds()) return;
        pieces[pos.x, pos.y] = p;
    }

    public Piece GetPieceAt(BoardPosition pos)
    {
        if (!pos.InBounds()) return null;
        return pieces[pos.x, pos.y];
    }
}
