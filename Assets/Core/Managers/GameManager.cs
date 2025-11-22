using System;
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


[Flags]
public enum MoveType
{
    Normal = 0,
    Capture = 1 << 0,
    DoubleStep = 1 << 1,  // tốt đi 2 ô
    EnPassant = 1 << 2,
    Castling = 1 << 3,
    Promotion = 1 << 4
}

public class Move
{
    public BoardPosition from;
    public BoardPosition to;
    public MoveType moveType;
    public Piece capturedPiece;    // nếu có quân bị ăn
    public Piece promotionPiece;   // nếu là promotion

    public Move(BoardPosition from, BoardPosition to, MoveType moveType = MoveType.Normal, Piece captured = null, Piece promotion = null)
    {
        this.from = from;
        this.to = to;
        this.moveType = moveType;
        this.capturedPiece = captured;
        this.promotionPiece = promotion;
    }

    // Helper methods để check kiểu nước đi
    public bool IsCapture() => moveType.HasFlag(MoveType.Capture);
    public bool IsDoubleStep() => moveType.HasFlag(MoveType.DoubleStep);
    public bool IsEnPassant() => moveType.HasFlag(MoveType.EnPassant);
    public bool IsCastling() => moveType.HasFlag(MoveType.Castling);
    public bool IsPromotion() => moveType.HasFlag(MoveType.Promotion);
}