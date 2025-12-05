using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    public override void GeneratePseudoLegalMoves(List<Move> moves)
    {
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        GetSlidingMoves(moves, dx, dy);
    }
}
