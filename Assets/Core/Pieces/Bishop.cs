using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override void GeneratePseudoLegalMoves(List<Move> moves)
    {
        int[] dx = { -1, -1, 1, 1 };
        int[] dy = { -1, 1, -1, 1 };

        GetSlidingMoves(moves, dx, dy);
    }
}
