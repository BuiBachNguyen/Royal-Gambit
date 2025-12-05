using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Base class for all pieces.
public abstract class Piece : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected PlayerColor color;
    [SerializeField] protected PieceType pieceType;
    [SerializeField] protected Tile tile;
    [SerializeField] protected BoardPosition pos;
    [SerializeField] protected bool hasMoved = false;
    [SerializeField] protected BoardManager boardManager;

    protected bool isOnClick;

    #region Getter Setter
    public Tile Tile
    {
        get { return tile; }
        set { tile = value; }
    }
    public PieceType PieceType => pieceType;
    public PlayerColor Color
    {
        get { return color; }
        set { color = value; }
    }    
    public BoardPosition Pos
    {
        get { return pos; }
        set { pos = value; }
    }
    public bool IsOnClick
    {
        get { return isOnClick; }
        set { isOnClick = value; }
    }
    public bool HasMoved => hasMoved;
    #endregion


    public static event Action<Piece, PointerEventData> OnAnyPieceClicked;

    private void Start()
    {
        if (boardManager == null)
            boardManager = BoardManager.Instance;
    }

    // Optimization: Pass list to avoid GC allocation
    public virtual void GeneratePseudoLegalMoves(List<Move> moves)
    {
        if (DEBUG.isLogicDebuging || DEBUG.overviewDebug) 
            Debug.Log("Generate moves base called");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnAnyPieceClicked?.Invoke(this, eventData);
        if (this.tile != null)
            this.tile.OnPointerClick(eventData);
    }

    public void MoveTo(Tile targetTile, bool hasMove = true)
    {
        if (tile != null)
            tile.RemovePiece();
        this.hasMoved = hasMove;
        targetTile.PlacePiece(this);
    }

    protected void GetSlidingMoves(List<Move> moves, int[] dx, int[] dy)
    {
        BoardPosition currentPos = this.Pos;

        for (int i = 0; i < dx.Length; i++)
        {
            for (int step = 1; step < BoardManager.BOARD_SIZE; step++)
            {
                BoardPosition targetPos = new BoardPosition(currentPos.x + dx[i] * step, currentPos.y + dy[i] * step);

                if (!targetPos.InBounds()) break;

                Piece targetPiece = boardManager.GetPieceAt(targetPos);

                if (targetPiece == null)
                {
                    moves.Add(new Move(currentPos, targetPos, MoveType.Normal));
                }
                else
                {
                    if (targetPiece.Color != this.Color)
                    {
                        moves.Add(new Move(currentPos, targetPos, MoveType.Capture, targetPiece));
                    }
                    break;
                }
            }
        }
    }
}
