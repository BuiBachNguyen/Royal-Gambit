using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override List<Move> GeneratePseudoLegalMoves()
    {
        List<Move> result = new List<Move>();

        int[] dx = { 2, 2, -2, -2, 1, 1, -1, -1 };
        int[] dy = { 1, -1, 1, -1, 2, -2, 2, -2 };

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

        return result;
    }
}
