using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override void GeneratePseudoLegalMoves(List<Move> moves)
    {
        BoardPosition currentPos = this.Pos;
        PlayerColor pieceColor = this.color;

        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int i = 0; i < dx.Length; i++)
        {
            BoardPosition targetPos = new BoardPosition(pos.x + dx[i], pos.y + dy[i]);
            
            if (!targetPos.InBounds()) continue;

            Piece targetPiece = boardManager.GetPieceAt(targetPos);
            
            if (targetPiece == null)
            {
                moves.Add(new Move(currentPos, targetPos, MoveType.Normal));
            }
            else if (targetPiece.Color != pieceColor)
            {
                moves.Add(new Move(currentPos, targetPos, MoveType.Capture, targetPiece));
            }
        }

        if (!hasMoved && !boardManager.IsKingInCheck(pieceColor))
        {
            AddCastlingMoves(moves, currentPos, pieceColor);
        }
    }

    private void AddCastlingMoves(List<Move> moves, BoardPosition currentPos, PlayerColor pieceColor)
    {
        int kingRow = currentPos.x;
        int kingCol = currentPos.y;

        if (kingRow != 0 && kingRow != BoardManager.BOARD_SIZE - 1) return;
        if (kingCol != 4) return;

        bool kingSideAvailable = CheckKingSideCastling(kingRow, pieceColor);
        bool queenSideAvailable = CheckQueenSideCastling(kingRow, pieceColor);

        if (kingSideAvailable)
        {
            moves.Add(new Move(currentPos, new BoardPosition(kingRow, 6), MoveType.Castling));
        }

        if (queenSideAvailable)
        {
            moves.Add(new Move(currentPos, new BoardPosition(kingRow, 2), MoveType.Castling));
        }
    }

    private bool CheckKingSideCastling(int row, PlayerColor pieceColor)
    {
        Piece rook = boardManager.GetPieceAt(new BoardPosition(row, BoardManager.BOARD_SIZE - 1));
        if (rook == null || rook is not Rook || rook.Color != pieceColor || rook.HasMoved)
            return false;

        for (int col = 5; col <= 6; col++)
        {
            if (boardManager.GetPieceAt(new BoardPosition(row, col)) != null)
                return false;
        }

        PlayerColor enemyColor = (pieceColor == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
        
        // Positions: 4 (start), 5 (through), 6 (dest)
        if (boardManager.IsSquareAttacked(new BoardPosition(row, 5), enemyColor) || 
            boardManager.IsSquareAttacked(new BoardPosition(row, 6), enemyColor))
            return false;

        return true;
    }

    private bool CheckQueenSideCastling(int row, PlayerColor pieceColor)
    {
        Piece rook = boardManager.GetPieceAt(new BoardPosition(row, 0));

        if (rook == null || rook is not Rook || rook.Color != pieceColor || rook.HasMoved)
            return false;

        for (int col = 1; col <= 3; col++)
        {
            if (boardManager.GetPieceAt(new BoardPosition(row, col)) != null)
                return false;
        }

        PlayerColor enemyColor = (pieceColor == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;

        // Check path (King moves 2 squares: 4 -> 3 -> 2)
        if (boardManager.IsSquareAttacked(new BoardPosition(row, 3), enemyColor) || 
            boardManager.IsSquareAttacked(new BoardPosition(row, 2), enemyColor))
            return false;

        return true;
    }
}
