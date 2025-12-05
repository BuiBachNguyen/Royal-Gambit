using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override void GeneratePseudoLegalMoves(List<Move> moves)
    {
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        GetSlidingMoves(moves, dx, dy);
    }
}
