using System;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerColor
{
    White,
    Black
}


// Base class for all pieces. Not MonoBehaviour if you prefer pure data,
// but making MonoBehaviour easier to attach to prefab.
public abstract class Piece : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected PlayerColor color;
    [SerializeField] protected Tile tile;
    [SerializeField] protected BoardPosition pos;
    [SerializeField] protected bool hasMoved = false;
    [SerializeField] protected BoardManager board;
    protected bool isOnClick;

    #region Getter Setter
    public Tile Tile
    {
        get { return tile; }
        set { tile = value; }
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
    #endregion

    private void Start()
    {
        board = FindAnyObjectByType<BoardManager>();
    }

    //public virtual void Initialize(BoardManager boardManager, PlayerColor color, BoardPosition pos)
    //{
    //    this.board = boardManager;
    //    this.color = color;
    //    this.pos = pos;
    //    hasMoved = false;
    //}

    //// Return all pseudo-legal moves (not filtering out leaving own king in check)
    public abstract List<Move> GeneratePseudoLegalMoves();

    //// helper: ensure move inside board
    //protected bool InBounds(int x, int y) => x >= 0 && x < 8 && y >= 0 && y < 8;

    //// helper to add capture/free moves for sliding pieces etc.
    //protected void AddIfValid(List<Move> list, int tx, int ty)
    //{
    //    var targetPos = new BoardPosition(tx, ty);
    //    if (!targetPos.InBounds()) return;
    //    var target = board.GetPieceAt(targetPos);
    //    if (target == null || target.color != this.color)
    //    {
    //        list.Add(new Move(pos, targetPos) { captured = target });
    //    }
    //}

    //// Move piece (updates board data); does not check legality - caller must ensure
    //public virtual void DoMove(Move m)
    //{
    //    board.SetPieceAt(pos, null);
    //    var destPiece = board.GetPieceAt(m.to);
    //    if (m.isEnPassant && destPiece == null)
    //    {
    //        // remove the pawn that was captured en-passant (behind to)
    //        var epPos = new BoardPosition(m.to.x, m.from.y);
    //        board.SetPieceAt(epPos, null);
    //        var epObj = board.GetPieceObjectAt(epPos);
    //        if (epObj) Destroy(epObj.gameObject);
    //    }

    //    // if capturing normal, destroy captured piece object
    //    if (destPiece != null)
    //    {
    //        var obj = board.GetPieceObjectAt(m.to);
    //        if (obj) Destroy(obj.gameObject);
    //    }

    //    // for castling, move rook too
    //    if (m.isCastling)
    //    {
    //        int dir = (m.to.x - m.from.x) > 0 ? 1 : -1;
    //        var rookFromX = dir > 0 ? 7 : 0;
    //        var rookToX = m.to.x - dir;
    //        var rookPos = new BoardPosition(rookFromX, m.from.y);
    //        var rook = board.GetPieceAt(rookPos);
    //        if (rook != null)
    //        {
    //            var rookObj = board.GetPieceObjectAt(rookPos);
    //            board.SetPieceAt(rookPos, null);
    //            board.SetPieceAt(new BoardPosition(rookToX, rookPos.y), rook);
    //            rook.pos = new BoardPosition(rookToX, rookPos.y);
    //            rook.hasMoved = true;
    //            if (rookObj) rookObj.transform.position = board.TileToWorld(new BoardPosition(rookToX, rookPos.y));
    //        }
    //    }

    //    // move this piece's gameobject to target tile position
    //    var thisObj = board.GetPieceObjectAt(pos);
    //    board.SetPieceAt(m.to, this);
    //    pos = m.to;
    //    hasMoved = true;

    //    if (thisObj) thisObj.transform.position = board.TileToWorld(m.to);

    //    // Promotion handling (default promote to Queen)
    //    if (m.isPromotion)
    //    {
    //        // destroy this object and instantiate the promoted prefab
    //        var obj = board.GetPieceObjectAt(pos);
    //        if (obj) Destroy(obj.gameObject);
    //        board.PromotePawn(this, m.promotionPiece);
    //    }
    //}

    //Click on Tile has piece
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK r");
        if (tile == null || board == null) return;
        if (board.PickingPiece == null)
        {
            this.isOnClick = true;
            this.Tile.Highlight(isOnClick);
            board.PickingPiece = this;
        }
        else if (board.PickingPiece == this)
        {
            this.isOnClick = !isOnClick;
            this.Tile.Highlight(isOnClick);
            board.PickingPiece = isOnClick ? this : null;
        }
        else if (board.PickingPiece != this)
        {
            //clear old piece and tile
            board.PickingPiece.Tile.Highlight(false);
            board.PickingPiece.isOnClick = false;
            //Set for new piece
            this.isOnClick = true;
            this.Tile.Highlight(true);
            board.PickingPiece = this;
        }
    }

    public void MoveTo(Tile targetTile)
    {
        if (tile != null)
            tile.RemovePiece();

        targetTile.PlacePiece(this);
    }

    internal static King Instantiate(BoardManager boardManager, PlayerColor black, BoardPosition boardPosition)
    {
        throw new NotImplementedException();
    }
}
