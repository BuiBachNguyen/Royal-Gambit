using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[System.Serializable]
public struct BoardPosition
{
    public int x;
    public int y;

    public BoardPosition(int x, int y) { this.x = x; this.y = y; }

    public bool InBounds() => x >= 0 && x < BoardManager.BOARD_SIZE && y >= 0 && y < BoardManager.BOARD_SIZE;

    public static bool operator ==(BoardPosition a, BoardPosition b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(BoardPosition a, BoardPosition b) => !(a == b);

    public override bool Equals(object obj)
    {
        if (!(obj is BoardPosition)) return false;
        var o = (BoardPosition)obj;
        return this == o;
    }

    public override int GetHashCode() => x * 31 + y;
}


public class Tile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] BoardManager boardManager;
    [SerializeField] BoardPosition pos;
    [SerializeField] Piece occupying;

    [SerializeField] ItemSpriteDatabase data; // return right img with Status and corlor
    [SerializeField] TileStatus status = TileStatus.NoAct;
    [SerializeField] TileColor tileColor;
    [SerializeField] Image img;


    public virtual void Initialize(
        BoardManager boardManager, 
        BoardPosition pos, 
        Piece occupying = null, 
        TileStatus status = TileStatus.NoAct)
    {
        this.boardManager = boardManager;
        this.pos = pos;
        this.occupying = occupying;
        this.status = status;
    }
    void Awake()
    {
        img = GetComponent<Image>();
    }
    void Start()
    {
        this.tileColor = GetTileColor();
        if (img != null)
            img.sprite = data.GetSprite(status, tileColor);
    }

    public TileColor GetTileColor()
    {
        int sum = pos.x + pos.y;
        if(sum % 2 == 0)
            return TileColor.Black;
        else
            return TileColor.White;
    }    
    public BoardPosition GetBoardPotition() => pos;


    public void ChangeStatus(TileStatus newtatus)
    {
        this.status = newtatus;
        img.sprite = data.GetSprite(newtatus, GetTileColor());
    }    

    public void PlacePiece(Piece piece)
    {
        //Update ref two way
        this.occupying = piece;
        piece.Tile = this;
        piece.Pos = this.pos;

        //Set child and let them alind center
        piece.transform.SetParent(this.transform, false);
        piece.transform.SetAsLastSibling();
        RectTransform pieceRect = piece.GetComponent<RectTransform>();
        pieceRect.anchorMin = new Vector2(0, 0);
        pieceRect.anchorMax = new Vector2(1, 1);
        pieceRect.anchoredPosition = Vector2.zero;
        pieceRect.sizeDelta = Vector2.zero;

    }

    public void RemovePiece()
    {
        if (occupying != null)
        {
            occupying.Tile = null;
            occupying = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (boardManager == null) return;

        if (boardManager.PickingPiece == null) return;

        if(this.status == TileStatus.Act)
            boardManager.HandleMove(this, boardManager.PickingPiece);
    }
}
