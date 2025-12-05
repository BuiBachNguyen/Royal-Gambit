using System;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override void GeneratePseudoLegalMoves(List<Move> moves)
    {
        BoardPosition currentPos = this.Pos;
        PlayerColor pieceColor = this.color;

        int dir = (pieceColor == PlayerColor.White) ? 1 : -1;

        BoardPosition forward = new BoardPosition(currentPos.x + dir, currentPos.y);
        if (forward.InBounds() && boardManager.GetPieceAt(forward) == null)
        { 
            moves.Add(new Move(currentPos, forward, MoveType.Normal));
        }    

        if (!hasMoved)
        {
            BoardPosition twoStep = new BoardPosition(currentPos.x + 2 * dir, currentPos.y);
            if (twoStep.InBounds() && boardManager.GetPieceAt(forward) == null && boardManager.GetPieceAt(twoStep) == null)
                moves.Add(new Move(currentPos, twoStep, MoveType.DoubleStep));
        }

        BoardPosition captureLeft = new BoardPosition(currentPos.x + dir, currentPos.y + 1);
        BoardPosition captureRight = new BoardPosition(currentPos.x + dir, currentPos.y - 1);

        if (captureLeft.InBounds())
        {
            Piece target = boardManager.GetPieceAt(captureLeft);
            if (target != null && target.Color != pieceColor)
            {
                moves.Add(new Move(currentPos, captureLeft, MoveType.Capture, target));
            }    
        }

        if (captureRight.InBounds())
        {
            Piece target = boardManager.GetPieceAt(captureRight);
            if (target != null && target.Color != pieceColor)
            {
                moves.Add(new Move(currentPos, captureRight, MoveType.Capture, target));
            }    
        }
        
        // En Passant Logic
        if (!((pieceColor == PlayerColor.White && currentPos.x == 4) || (pieceColor == PlayerColor.Black && currentPos.x == 3)))
            return; 

        BoardPosition left = new BoardPosition(currentPos.x, currentPos.y + 1);
        BoardPosition right = new BoardPosition(currentPos.x, currentPos.y - 1);

        CheckEnPassant(left, dir, moves);
        CheckEnPassant(right, dir, moves);
    }

    private void CheckEnPassant(BoardPosition neighborPos, int dir, List<Move> moves)
    {
        if (neighborPos.InBounds())
        {
            Piece neighborPiece = boardManager.GetPieceAt(neighborPos);
            if (neighborPiece is Pawn && neighborPiece.Color != this.color && boardManager.LastMove != null)
            {
                Move last = boardManager.LastMove;
                if (last.to.Equals(neighborPos) && last.moveType == MoveType.DoubleStep)
                {
                    BoardPosition enPassantTarget = new BoardPosition(this.Pos.x + dir, neighborPos.y);
                    moves.Add(new Move(this.Pos, enPassantTarget, MoveType.EnPassant));
                }
            }
        }
    }
}
