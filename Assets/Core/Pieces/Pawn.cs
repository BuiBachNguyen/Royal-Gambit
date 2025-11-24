using System;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Move> GeneratePseudoLegalMoves()
    {
        List<Move> result = new List<Move>();

        int dir = (color == PlayerColor.White) ? 1 : -1;

        // Di chuyển thẳng
        BoardPosition forward = new BoardPosition(pos.x + dir, pos.y);
        if (forward.InBounds() && boardManager.GetPieceAt(forward) == null)
        { 
            result.Add(new Move(this.Pos, forward, MoveType.Normal));
        }    

        // Nếu chưa đi → di chuyển 2 ô
        if (!hasMoved)
        {
            BoardPosition twoStep = new BoardPosition(pos.x + 2 * dir, pos.y);
            if (twoStep.InBounds() && boardManager.GetPieceAt(forward) == null && boardManager.GetPieceAt(twoStep) == null)
                result.Add(new Move(this.Pos, twoStep, MoveType.DoubleStep));
        }

        // Ăn chéo
        BoardPosition captureLeft = new BoardPosition(pos.x + dir, pos.y + 1);
        BoardPosition captureRight = new BoardPosition(pos.x + dir, pos.y - 1);

        if (captureLeft.InBounds())
        {
            Piece target = boardManager.GetPieceAt(captureLeft);
            if (target != null && target.Color != color)
            {
                result.Add(new Move(this.Pos, captureLeft, MoveType.Capture, target));
            }    
        }

        if (captureRight.InBounds())
        {
            Piece target = boardManager.GetPieceAt(captureRight);
            if (target != null && target.Color != color)
            {
                result.Add(new Move(this.Pos, captureRight, MoveType.Capture, target));
            }    
        }
        
        // Bắt tốt qua đường (En Passant)
        if (!((color == PlayerColor.White && pos.x == 4) || (color == PlayerColor.Black && pos.x == 3)))
            return result; 

        BoardPosition left = new BoardPosition(pos.x, pos.y + 1);
        BoardPosition right = new BoardPosition(pos.x, pos.y - 1);

        if (left.InBounds())
        {
            Piece leftPiece = boardManager.GetPieceAt(left);
            if (leftPiece is Pawn && leftPiece.Color != color && boardManager.LastMove != null)
            {
                Move last = boardManager.LastMove;
                if (last.to.Equals(left) && last.moveType == MoveType.DoubleStep)
                {
                    BoardPosition enPassantTarget = new BoardPosition(pos.x + dir, pos.y + 1);
                    result.Add(new Move(this.Pos, enPassantTarget, MoveType.EnPassant));
                }
            }
        }
        if (right.InBounds())
        {
            Piece rightPiece = boardManager.GetPieceAt(right);
            if (rightPiece is Pawn && rightPiece.Color != color && boardManager.LastMove != null)
            {
                Move last = boardManager.LastMove;
                if (last.to.Equals(right) && last.moveType == MoveType.DoubleStep)
                {
                    BoardPosition enPassantTarget = new BoardPosition(pos.x + dir, pos.y - 1);
                    result.Add(new Move(this.Pos, enPassantTarget, MoveType.EnPassant));
                }
            }
        }
        return result;
    }
}
