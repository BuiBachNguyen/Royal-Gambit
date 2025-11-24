using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Move> GeneratePseudoLegalMoves()
    {
        List<Move> result = new List<Move>();

        int[] dx = { -1, -1, 1, 1 };
        int[] dy = { -1, 1, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            for (int step = 1; step < 8; step++)
            {
                BoardPosition targetPos = new BoardPosition(pos.x + dx[i] * step, pos.y + dy[i] * step);
                
                if (!targetPos.InBounds()) break;

                Piece targetPiece = boardManager.GetPieceAt(targetPos);
                
                if (targetPiece == null)
                {
                    result.Add(new Move(this.Pos, targetPos, MoveType.Normal));
                }
                else
                {
                    if (targetPiece.Color != this.color)
                    {
                        result.Add(new Move(this.Pos, targetPos, MoveType.Capture, targetPiece));
                    }
                    break;
                }
            }
        }

        return result;
    }
}
