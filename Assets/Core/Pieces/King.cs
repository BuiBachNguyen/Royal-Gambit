using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override List<Move> GeneratePseudoLegalMoves()
    {
        List<Move> result = new List<Move>();
        result.Add(new Move(this.Pos, new BoardPosition(pos.x+1, pos.y)));
        result.Add(new Move(this.Pos, new BoardPosition(pos.x-1, pos.y)));
        result.Add(new Move(this.Pos, new BoardPosition(pos.x,  pos.y+1)));
        result.Add(new Move(this.Pos, new BoardPosition(pos.x, pos.y-1)));
        return result;
    }
}
