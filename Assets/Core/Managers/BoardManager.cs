using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    public const int BOARD_SIZE = 8;

    #region Singleton
    public static BoardManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private Piece[,] pieces = new Piece[BOARD_SIZE, BOARD_SIZE];

    private Piece pickingPiece;
    private PlayerColor currentTurn = PlayerColor.White;
    private Move lastMove;
    
    private Piece whiteKing;
    private Piece blackKing;

    // Cache list to avoid GC allocation
    private List<Move> cachedLegalMoves = new List<Move>(64);
    private List<Move> cachedPseudoMoves = new List<Move>(64);

    public Piece PickingPiece { get => pickingPiece; set => pickingPiece = value; }
    public PlayerColor CurrentTurn { get => currentTurn; set => currentTurn = value; }
    public Move LastMove => lastMove;

    private void OnEnable() => Piece.OnAnyPieceClicked += HandlePieceClick;
    private void OnDisable() => Piece.OnAnyPieceClicked -= HandlePieceClick;

    private void HandlePieceClick(Piece clickedPiece, PointerEventData eventData)
    {
        if (clickedPiece.Color != currentTurn) return;

        pickingPiece = clickedPiece;
        
        // Use cached lists
        cachedPseudoMoves.Clear();
        clickedPiece.GeneratePseudoLegalMoves(cachedPseudoMoves);
        
        cachedLegalMoves.Clear();
        FilterLegalMoves(clickedPiece, cachedPseudoMoves, cachedLegalMoves);

        Board.Instance.ChangeStatusTiles(cachedLegalMoves, TileStatus.Act);
    }

    public void HandleMove(Tile targetTile, Piece piece)
    {
        if (pickingPiece == null) return;

        cachedPseudoMoves.Clear();
        pickingPiece.GeneratePseudoLegalMoves(cachedPseudoMoves);
        
        cachedLegalMoves.Clear();
        FilterLegalMoves(pickingPiece, cachedPseudoMoves, cachedLegalMoves);

        BoardPosition targetPos = targetTile.GetBoardPotition();
        Move selectedMove = cachedLegalMoves.Find(m => m.to.x == targetPos.x && m.to.y == targetPos.y);
        
        if (selectedMove == null) return;

        // --- EN PASSANT ---
        if (selectedMove.IsEnPassant())
        {
            int dir = pickingPiece.Color == PlayerColor.White ? 1 : -1;
            BoardPosition pawnPos = new BoardPosition(selectedMove.to.x - dir, selectedMove.to.y);
            Piece enPassantPawn = GetPieceAt(pawnPos);
            if (enPassantPawn != null)
            {
                Destroy(enPassantPawn.gameObject);
                SetPieceAt(pawnPos, null);
            }
        }

        // --- CASTLING ---
        if (selectedMove.IsCastling()) HandleCastling(selectedMove);

        // --- CAPTURE ---
        if (selectedMove.IsCapture())
        {
            Piece targetPiece = GetPieceAt(targetPos);
            if (targetPiece != null && targetPiece.Color != pickingPiece.Color)
            {
                Destroy(targetPiece.gameObject);
                SetPieceAt(targetPos, null);
            }
        }

        // --- MOVE ---
        pickingPiece.MoveTo(targetTile);
        SetPieceAt(targetPos, pickingPiece);

        lastMove = selectedMove;

        // --- PROMOTION ---
        if (pickingPiece is Pawn && (targetPos.x == 0 || targetPos.x == BOARD_SIZE - 1))
            PromotePawn(pickingPiece);

        pickingPiece = null;

        Board.Instance.ChangeStatusTiles(null, TileStatus.NoAct);

        currentTurn = currentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
    }

    // --- LEGAL MOVE FILTER ---
    // Modified to be non-allocating
    public void FilterLegalMoves(Piece piece, List<Move> pseudoMoves, List<Move> resultMoves)
    {
        resultMoves.Clear();
        foreach (var mv in pseudoMoves)
        {
            Piece captured = SimulateMove(piece, mv);
            if (!IsKingInCheck(piece.Color)) 
            {
                resultMoves.Add(mv);
            }
            UndoSimulateMove(piece, mv, captured);
        }
    }

    private Piece SimulateMove(Piece piece, Move mv)
    {
        Piece captured = GetPieceAt(mv.to);
        SetPieceAt(mv.from, null);
        SetPieceAt(mv.to, piece);
        return captured;
    }

    private void UndoSimulateMove(Piece piece, Move mv, Piece captured)
    {
        SetPieceAt(mv.from, piece);
        SetPieceAt(mv.to, captured);
    }

    public bool IsKingInCheck(PlayerColor color)
    {
        Piece king = GetKing(color);
        if (king == null) return false;

        PlayerColor enemyColor = (color == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
        return IsSquareAttacked(king.Pos, enemyColor);
    }

    /// <summary>
    /// Checks if a specific square is under attack by the opponent using Raycasting (Reverse Check).
    /// This is O(1) relative to board size (constant checks), much faster than iterating all pieces O(N).
    /// </summary>
    public bool IsSquareAttacked(BoardPosition pos, PlayerColor attackerColor)
    {
        // 1. Check for Sliding Pieces (Rook, Queen) - Vertical/Horizontal
        if (CheckSlidingAttack(pos, attackerColor, new int[] { 1, -1, 0, 0 }, new int[] { 0, 0, 1, -1 }, PieceType.Rook))
            return true;

        // 2. Check for Sliding Pieces (Bishop, Queen) - Diagonal
        if (CheckSlidingAttack(pos, attackerColor, new int[] { 1, 1, -1, -1 }, new int[] { 1, -1, 1, -1 }, PieceType.Bishop))
            return true;

        // 3. Check for Knights
        int[] knightDx = { 1, 1, 2, 2, -1, -1, -2, -2 };
        int[] knightDy = { 2, -2, 1, -1, 2, -2, 1, -1 };
        for (int i = 0; i < 8; i++)
        {
            if (CheckSinglePieceAttack(pos, attackerColor, knightDx[i], knightDy[i], PieceType.Knight))
                return true;
        }

        // 4. Check for Pawns
        // White pawns move +1 x, so they attack from x-1. Black pawns move -1 x, so they attack from x+1.
        int pawnAttackRowDir = (attackerColor == PlayerColor.White) ? -1 : 1; 
        if (CheckSinglePieceAttack(pos, attackerColor, pawnAttackRowDir, 1, PieceType.Pawn) ||
            CheckSinglePieceAttack(pos, attackerColor, pawnAttackRowDir, -1, PieceType.Pawn))
            return true;

        // 5. Check for King (enemy king logic, kings cannot stand next to each other)
        int[] kingDx = { 1, 1, 1, 0, 0, -1, -1, -1 };
        int[] kingDy = { 1, 0, -1, 1, -1, 1, 0, -1 };
        for (int i = 0; i < 8; i++)
        {
            if (CheckSinglePieceAttack(pos, attackerColor, kingDx[i], kingDy[i], PieceType.King))
                return true;
        }

        return false;
    }

    private bool CheckSlidingAttack(BoardPosition startPos, PlayerColor attackerColor, int[] dx, int[] dy, PieceType straightType)
    {
        for (int i = 0; i < dx.Length; i++)
        {
            for (int dist = 1; dist < BOARD_SIZE; dist++)
            {
                int x = startPos.x + dx[i] * dist;
                int y = startPos.y + dy[i] * dist;

                if (x < 0 || x >= BOARD_SIZE || y < 0 || y >= BOARD_SIZE) break;

                Piece p = pieces[x, y];
                if (p != null)
                {
                    if (p.Color == attackerColor && (p.PieceType == straightType || p.PieceType == PieceType.Queen))
                    {
                        return true;
                    }
                    // Blocked by any piece (friend or foe that isn't the attacker we looked for)
                    break;
                }
            }
        }
        return false;
    }

    private bool CheckSinglePieceAttack(BoardPosition startPos, PlayerColor attackerColor, int dx, int dy, PieceType targetType)
    {
        int x = startPos.x + dx;
        int y = startPos.y + dy;

        if (x < 0 || x >= BOARD_SIZE || y < 0 || y >= BOARD_SIZE) return false;

        Piece p = pieces[x, y];
        return p != null && p.Color == attackerColor && p.PieceType == targetType;
    }

    private Piece GetKing(PlayerColor color)
    {
        Piece king = color == PlayerColor.White ? whiteKing : blackKing;
        if (king != null && king is King && king.Color == color)
            return king;
        
        king = FindKing(color);
        if (color == PlayerColor.White)
            whiteKing = king;
        else
            blackKing = king;
        
        return king;
    }

    private Piece FindKing(PlayerColor color)
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                Piece p = pieces[row, col];
                if (p != null && p.Color == color && p is King)
                    return p;
            }
        }
        return null;
    }

    private void HandleCastling(Move mv)
    {
        int row = mv.from.x;
        Piece rook = null;
        BoardPosition rookFromPos;
        BoardPosition rookToPos;

        // King side castling
        if (mv.to.y == 6)
        {
            rookFromPos = new BoardPosition(row, BOARD_SIZE - 1);
            rookToPos = new BoardPosition(row, 5);
        }
        // Queen side castling
        else if (mv.to.y == 2)
        {
            rookFromPos = new BoardPosition(row, 0);
            rookToPos = new BoardPosition(row, 3);
        }
        else
        {
            return;
        }

        rook = GetPieceAt(rookFromPos);
        if (rook == null) return;

        SetPieceAt(rookFromPos, null);
        SetPieceAt(rookToPos, rook);
        rook.MoveTo(Board.Instance.GetTile(rookToPos.x, rookToPos.y), true);
    }

    private void PromotePawn(Piece pawn)
    {
        Destroy(pawn.gameObject);
        Piece promotePawn = Instantiate(Board.Instance.PrefabLibrary.GetPrefab(PieceType.Queen, pawn.Color));
        promotePawn.Color = pawn.Color;
        promotePawn.MoveTo(pawn.Tile);
        SetPieceAt(pawn.Pos, promotePawn);
    }

    public void SetPieceAt(BoardPosition pos, Piece p)
    {
        if (!pos.InBounds()) return;
        
        Piece oldPiece = pieces[pos.x, pos.y];
        pieces[pos.x, pos.y] = p;
        
        if (oldPiece is King)
        {
            if (oldPiece.Color == PlayerColor.White)
                whiteKing = null;
            else
                blackKing = null;
        }
        
        if (p is King)
        {
            if (p.Color == PlayerColor.White)
                whiteKing = p;
            else
                blackKing = p;
        }
    }

    public Piece GetPieceAt(BoardPosition pos)
    {
        if (!pos.InBounds()) return null;
        return pieces[pos.x, pos.y]; // x = row, y = col
    }

    public bool LastMoveWasDoubleStep()
    {
        return lastMove != null && lastMove.IsDoubleStep();
    }

    public bool LastMoveWasEnPassantablePawn()
    {
        if (lastMove == null || !lastMove.IsDoubleStep()) return false;
        
        BoardPosition lastMoveTo = lastMove.to;
        Piece lastMovedPiece = GetPieceAt(lastMoveTo);
        return lastMovedPiece != null && lastMovedPiece is Pawn;
    }
}
