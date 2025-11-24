using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override List<Move> GeneratePseudoLegalMoves()
    {
        List<Move> result = new List<Move>();

        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int i = 0; i < 8; i++)
        {
            BoardPosition targetPos = new BoardPosition(pos.x + dx[i], pos.y + dy[i]);
            
            if (!targetPos.InBounds()) continue;

            Piece targetPiece = boardManager.GetPieceAt(targetPos);
            
            if (targetPiece == null)
            {
                result.Add(new Move(this.Pos, targetPos, MoveType.Normal));
            }
            else if (targetPiece.Color != this.color)
            {
                result.Add(new Move(this.Pos, targetPos, MoveType.Capture, targetPiece));
            }
        }

        if (!hasMoved && !boardManager.IsKingInCheck(this.color))
        {
            AddCastlingMoves(result);
        }

        return result;
    }

    private void AddCastlingMoves(List<Move> result)
    {
        int kingRow = pos.x;
        int kingCol = pos.y;

        if (kingRow != 0 && kingRow != 7) return;
        if (kingCol != 4) return;

        bool kingSideAvailable = CheckKingSideCastling(kingRow);
        bool queenSideAvailable = CheckQueenSideCastling(kingRow);

        if (kingSideAvailable)
        {
            result.Add(new Move(this.Pos, new BoardPosition(kingRow, 6), MoveType.Castling));
        }

        if (queenSideAvailable)
        {
            result.Add(new Move(this.Pos, new BoardPosition(kingRow, 2), MoveType.Castling));
        }
    }

    private bool CheckKingSideCastling(int row)
    {
        Piece rook = boardManager.GetPieceAt(new BoardPosition(row, 7));
        if (rook == null || rook is not Rock || rook.Color != this.color || rook.hasMoved)
            return false;

        for (int col = 5; col <= 6; col++)
        {
            if (boardManager.GetPieceAt(new BoardPosition(row, col)) != null)
                return false;
        }

        if (IsSquareAttacked(new BoardPosition(row, 5)) || IsSquareAttacked(new BoardPosition(row, 6)))
            return false;

        return true;
    }

    private bool CheckQueenSideCastling(int row)
    {
        Piece rook = boardManager.GetPieceAt(new BoardPosition(row, 0));
        if (rook == null || rook is not Rock || rook.Color != this.color || rook.hasMoved)
            return false;

        for (int col = 1; col <= 3; col++)
        {
            if (boardManager.GetPieceAt(new BoardPosition(row, col)) != null)
                return false;
        }

        if (IsSquareAttacked(new BoardPosition(row, 2)) || IsSquareAttacked(new BoardPosition(row, 3)))
            return false;

        return true;
    }

    private bool IsSquareAttacked(BoardPosition square)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece piece = boardManager.GetPieceAt(new BoardPosition(row, col));
                if (piece == null || piece.Color == this.color) continue;

                List<Move> moves = piece.GeneratePseudoLegalMoves();
                foreach (Move move in moves)
                {
                    if (move.to.x == square.x && move.to.y == square.y)
                        return true;
                }
            }
        }
        return false;
    }
}
