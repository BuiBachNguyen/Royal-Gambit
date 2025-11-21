using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Move> GeneratePseudoLegalMoves()
    {
        List<Move> result = new List<Move>();
        int dir = (color == PlayerColor.Black) ? 1 : -1;

        if (hasMoved == false)
        {
            result.Add(new Move(this.Pos, new BoardPosition(pos.x - 2 * dir, pos.y)));
        }
        result.Add(new Move(this.Pos, new BoardPosition(pos.x - 1 * dir, pos.y)));

        return result;
    }
}
