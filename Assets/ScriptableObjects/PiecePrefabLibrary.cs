using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Piece Prefab Library")]
public class PiecePrefabLibrary : ScriptableObject
{
    public Piece whitePawn;
    public Piece whiteKnight;
    public Piece whiteBishop;
    public Piece whiteRook;
    public Piece whiteQueen;
    public Piece whiteKing;

    public Piece blackPawn;
    public Piece blackKnight;
    public Piece blackBishop;
    public Piece blackRook;
    public Piece blackQueen;
    public Piece blackKing;

    public Piece GetPrefab(PieceType type, PlayerColor color)
    {
        switch (color)
        {
            case PlayerColor.White:
                return type switch
                {
                    PieceType.Pawn => whitePawn,
                    PieceType.Knight => whiteKnight,
                    PieceType.Bishop => whiteBishop,
                    PieceType.Rook => whiteRook,
                    PieceType.Queen => whiteQueen,
                    PieceType.King => whiteKing,
                    _ => null
                };

            case PlayerColor.Black:
                return type switch
                {
                    PieceType.Pawn => blackPawn,
                    PieceType.Knight => blackKnight,
                    PieceType.Bishop => blackBishop,
                    PieceType.Rook => blackRook,
                    PieceType.Queen => blackQueen,
                    PieceType.King => blackKing,
                    _ => null
                };
        }

        return null;
    }
}
