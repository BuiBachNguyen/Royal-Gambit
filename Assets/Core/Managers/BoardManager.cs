//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class BoardManager : MonoBehaviour
//{
//    #region Singleton
//    public static BoardManager Instance { get; private set; }
//    private void Awake()
//    {
//        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }
//    #endregion

//    [SerializeField] private Piece[,] pieces = new Piece[8, 8];

//    private Piece pickingPiece;
//    private PlayerColor currentTurn = PlayerColor.White;
//    private Move lastMove;

//    public Piece PickingPiece { get => pickingPiece; set => pickingPiece = value; }
//    public PlayerColor CurrentTurn { get => currentTurn; set => currentTurn = value; }

//    private void OnEnable() => Piece.OnAnyPieceClicked += HandlePieceClick;
//    private void OnDisable() => Piece.OnAnyPieceClicked -= HandlePieceClick;

//    private void HandlePieceClick(Piece clickedPiece, PointerEventData eventData)
//    {
//        if (clickedPiece.Color != currentTurn) return;

//        pickingPiece = clickedPiece;
//        var pseudo = clickedPiece.GeneratePseudoLegalMoves();
//        var legal = FilterLegalMoves(clickedPiece, pseudo);

//        Board.Instance.ChangeStatusTiles(legal, TileStatus.Act);
//    }

//    public void HandleMove(Tile targetTile, Piece piece)
//    {
//        if (pickingPiece == null) return;

//        var pseudo = pickingPiece.GeneratePseudoLegalMoves();
//        var legal = FilterLegalMoves(pickingPiece, pseudo);

//        BoardPosition targetPos = targetTile.GetBoardPotition();
//        bool isLegal = legal.Exists(m => m.to.x == targetPos.x && m.to.y == targetPos.y);
//        if (!isLegal) return;

//        Move selectedMove = legal.Find(m => m.to.x == targetPos.x && m.to.y == targetPos.y);

//        // --- EN PASSANT ---
//        if (selectedMove.moveType == MoveType.EnPassant)
//        {
//            int dir = pickingPiece.Color == PlayerColor.White ? 1 : -1;
//            BoardPosition pawnPos = new BoardPosition(selectedMove.to.x - dir, selectedMove.to.y);
//            Piece enPassantPawn = GetPieceAt(pawnPos);
//            if (enPassantPawn != null) { Destroy(enPassantPawn.gameObject); SetPieceAt(pawnPos, null); }
//        }


//        // --- CASTLING ---
//        if (selectedMove.moveType == MoveType.Castling) HandleCastling(selectedMove);

//        // --- CAPTURE ---
//        Piece targetPiece = GetPieceAt(targetPos);
//        if (targetPiece != null && targetPiece.Color != pickingPiece.Color)
//            Destroy(targetPiece.gameObject);

//        // --- MOVE ---
//        pickingPiece.MoveTo(targetTile);
//        SetPieceAt(targetPos, pickingPiece);

//        lastMove = selectedMove;

//        // --- PROMOTION ---
//        if (pickingPiece is Pawn && (targetPos.x == 0 || targetPos.x == 7))
//            PromotePawn(pickingPiece);

//        pickingPiece = null;
//        Board.Instance.ChangeStatusTiles(null, TileStatus.NoAct);
//        currentTurn = currentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
//    }

//    // --- LEGAL MOVE FILTER ---
//    public List<Move> FilterLegalMoves(Piece piece, List<Move> pseudoMoves)
//    {
//        List<Move> legal = new List<Move>();
//        foreach (var mv in pseudoMoves)
//        {
//            Piece captured = SimulateMove(piece, mv);
//            if (!IsKingInCheck(piece.Color)) legal.Add(mv);
//            UndoSimulateMove(piece, mv, captured);
//        }
//        return legal;
//    }

//    private Piece SimulateMove(Piece piece, Move mv)
//    {
//        Piece captured = GetPieceAt(mv.to);
//        SetPieceAt(mv.from, null);
//        SetPieceAt(mv.to, piece);
//        return captured;
//    }

//    private void UndoSimulateMove(Piece piece, Move mv, Piece captured)
//    {
//        SetPieceAt(mv.from, piece);
//        SetPieceAt(mv.to, captured);
//    }

//    public bool IsKingInCheck(PlayerColor color)
//    {
//        Piece king = FindKing(color);
//        if (king == null) return false;

//        for (int row = 0; row < 8; row++)
//        {
//            for (int col = 0; col < 8; col++)
//            {
//                Piece p = pieces[row, col];
//                if (p == null || p.Color == color) continue;

//                foreach (var mv in p.GeneratePseudoLegalMoves())
//                {
//                    if (mv.to.x == king.Pos.x && mv.to.y == king.Pos.y)
//                        return true;
//                }
//            }
//        }
//        return false;
//    }

//    private Piece FindKing(PlayerColor color)
//    {
//        for (int row = 0; row < 8; row++)
//            for (int col = 0; col < 8; col++)
//            {
//                Piece p = pieces[row, col];
//                if (p != null && p.Color == color && p is King)
//                    return p;
//            }
//        return null;
//    }

//    private void HandleCastling(Move mv)
//    {
//        int row = mv.from.x; // x = hàng
//        // King side
//        if (mv.to.y == 6)
//        {
//            Piece rook = GetPieceAt(new BoardPosition(row, 7));
//            rook.MoveTo(Board.Instance.GetTile(row, 5));
//            SetPieceAt(new BoardPosition(row, 5), rook);
//            SetPieceAt(new BoardPosition(row, 7), null);
//        }
//        // Queen side
//        else if (mv.to.y == 2)
//        {
//            Piece rook = GetPieceAt(new BoardPosition(row, 0));
//            rook.MoveTo(Board.Instance.GetTile(row, 3));
//            SetPieceAt(new BoardPosition(row, 3), rook);
//            SetPieceAt(new BoardPosition(row, 0), null);
//        }
//    }

//    private void PromotePawn(Piece pawn)
//    {
//        Destroy(pawn.gameObject);
//        Piece promotePawn = Instantiate(Board.Instance.PrefabLibrary.GetPrefab(PieceType.Queen, pawn.Color));
//        promotePawn.Color = pawn.Color;
//        promotePawn.MoveTo(pawn.Tile);
//        SetPieceAt(pawn.Pos, promotePawn);
//    }

//    public void SetPieceAt(BoardPosition pos, Piece p)
//    {
//        if (!pos.InBounds()) return;
//        pieces[pos.x, pos.y] = p; // x = row, y = col
//    }

//    public Piece GetPieceAt(BoardPosition pos)
//    {
//        if (!pos.InBounds()) return null;
//        return pieces[pos.x, pos.y]; // x = row, y = col
//    }
//}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    #region Singleton
    public static BoardManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private Piece[,] pieces = new Piece[8, 8];

    private Piece pickingPiece;
    private PlayerColor currentTurn = PlayerColor.White;
    private Move lastMove;

    public Piece PickingPiece { get => pickingPiece; set => pickingPiece = value; }
    public PlayerColor CurrentTurn { get => currentTurn; set => currentTurn = value; }
    public Move LastMove => lastMove; // expose LastMove

    private void OnEnable() => Piece.OnAnyPieceClicked += HandlePieceClick;
    private void OnDisable() => Piece.OnAnyPieceClicked -= HandlePieceClick;

    private void HandlePieceClick(Piece clickedPiece, PointerEventData eventData)
    {
        if (clickedPiece.Color != currentTurn) return;

        pickingPiece = clickedPiece;
        var pseudo = clickedPiece.GeneratePseudoLegalMoves();
        var legal = FilterLegalMoves(clickedPiece, pseudo);

        Board.Instance.ChangeStatusTiles(legal, TileStatus.Act);
    }

    public void HandleMove(Tile targetTile, Piece piece)
    {
        if (pickingPiece == null) return;

        var pseudo = pickingPiece.GeneratePseudoLegalMoves();
        var legal = FilterLegalMoves(pickingPiece, pseudo);

        BoardPosition targetPos = targetTile.GetBoardPotition();
        Move selectedMove = legal.Find(m => m.to.x == targetPos.x && m.to.y == targetPos.y);
        if (selectedMove == null) return;

        // --- EN PASSANT ---
        if (selectedMove.IsEnPassant())
        {
            int dir = pickingPiece.Color == PlayerColor.White ? 1 : -1;
            BoardPosition pawnPos = new BoardPosition(selectedMove.to.x + dir, selectedMove.to.y);
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
            Debug.Log("Destroy");
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
        if (pickingPiece is Pawn && (targetPos.x == 0 || targetPos.x == 7))
            PromotePawn(pickingPiece);

        pickingPiece = null;

        Board.Instance.ChangeStatusTiles(null, TileStatus.NoAct);

        currentTurn = currentTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
    }

    // --- LEGAL MOVE FILTER ---
    public List<Move> FilterLegalMoves(Piece piece, List<Move> pseudoMoves)
    {
        List<Move> legal = new List<Move>();
        foreach (var mv in pseudoMoves)
        {
            Piece captured = SimulateMove(piece, mv);
            if (!IsKingInCheck(piece.Color)) legal.Add(mv);
            UndoSimulateMove(piece, mv, captured);
        }
        return legal;
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
        Piece king = FindKing(color);
        if (king == null) return false;

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece p = pieces[row, col];
                if (p == null || p.Color == color) continue;

                foreach (var mv in p.GeneratePseudoLegalMoves())
                {
                    if (mv.to.x == king.Pos.x && mv.to.y == king.Pos.y)
                        return true;
                }
            }
        }
        return false;
    }

    private Piece FindKing(PlayerColor color)
    {
        for (int row = 0; row < 8; row++)
            for (int col = 0; col < 8; col++)
            {
                Piece p = pieces[row, col];
                if (p != null && p.Color == color && p is King)
                    return p;
            }
        return null;
    }

    private void HandleCastling(Move mv)
    {
        int row = mv.from.x; // x = hàng
        // King side
        if (mv.to.y == 6)
        {
            Piece rook = GetPieceAt(new BoardPosition(row, 7));
            rook.MoveTo(Board.Instance.GetTile(row, 5));
            SetPieceAt(new BoardPosition(row, 5), rook);
            SetPieceAt(new BoardPosition(row, 7), null);
        }
        // Queen side
        else if (mv.to.y == 2)
        {
            Piece rook = GetPieceAt(new BoardPosition(row, 0));
            rook.MoveTo(Board.Instance.GetTile(row, 3));
            SetPieceAt(new BoardPosition(row, 3), rook);
            SetPieceAt(new BoardPosition(row, 0), null);
        }
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
        pieces[pos.x, pos.y] = p; // x = row, y = col
    }

    public Piece GetPieceAt(BoardPosition pos)
    {
        if (!pos.InBounds()) return null;
        return pieces[pos.x, pos.y]; // x = row, y = col
    }

    // --- HELPER LAST MOVE CHECK ---
    public bool LastMoveWasDoubleStep()
    {
        return lastMove != null && lastMove.IsDoubleStep();
    }

    public bool LastMoveWasEnPassantablePawn()
    {
        return lastMove != null && lastMove.IsDoubleStep();
    }
}

