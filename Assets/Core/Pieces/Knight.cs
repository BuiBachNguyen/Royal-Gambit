using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override void GeneratePseudoLegalMoves(List<Move> moves)
    {
        BoardPosition currentPos = this.Pos;
        PlayerColor pieceColor = this.color;

        int[] dx = { 2, 2, -2, -2, 1, 1, -1, -1 };
        int[] dy = { 1, -1, 1, -1, 2, -2, 2, -2 };

        for (int i = 0; i < 8; i++)
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
    }
}
