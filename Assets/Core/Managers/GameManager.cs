using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// Represents a move. Contains origin, destination, optional flags for special moves
public class Move
{
    public BoardPosition from;
    public BoardPosition to;
    public Piece captured; // may be null
    public bool isEnPassant;
    public bool isCastling;
    public bool isPromotion;
    public Piece promotionPiece; // if used

    public Move(BoardPosition f, BoardPosition t)
    {
        from = f; to = t;
    }
}
