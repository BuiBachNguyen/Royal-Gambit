using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct BoardPosition
{
    public int x;
    public int y;

    public BoardPosition(int x, int y) { this.x = x; this.y = y; }

    public bool InBounds() => x >= 0 && x < 8 && y >= 0 && y < 8; // board 8x8 right?

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
    private Image img;

    public virtual void Initialize(BoardManager boardManager, BoardPosition pos, Piece occupying = null, Color? baseColor = null)
    {
        this.boardManager = boardManager;
        this.pos = pos;
        this.occupying = occupying;
        img = GetComponent<Image>();

        if (img != null)
        {
            img.color = baseColor ?? GetDefaultColor(pos);
        }
    }
    void Awake()
    {
        if (img == null)
        {
            img = GetComponent<Image>();
        }
        img.color = GetDefaultColor(pos);
    }
    //This is hardcode color, đont do like this
    private Color GetDefaultColor(BoardPosition pos)
    {
        return ((pos.x + pos.y) % 2 == 0)
            ? new Color(0.9f, 0.9f, 0.9f)   // WhiteClay
            : new Color(0.2f, 0.2f, 0.2f);  // DarkGrey
    }

    public void Highlight(bool on)
    {
        if (img == null)
            img = GetComponent<Image>();

        if (on)
            img.color = Color.green; // the same too
        else
            img.color = GetDefaultColor(pos);
    }
    public void HighLightCheckmate(bool on)
    {
        if (img == null)
            img = GetComponent<Image>();

        if (on)
            img.color = Color.red; //Another 
        else
            img.color = GetDefaultColor(pos);
    }

    public BoardPosition GetBoardPotition() => pos;

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
        pieceRect.anchorMin = new Vector2(0.5f, 0.5f);
        pieceRect.anchorMax = new Vector2(0.5f, 0.5f);
        pieceRect.anchoredPosition = Vector2.zero;
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

        List<Move> legalMoves = boardManager.PickingPiece.GeneratePseudoLegalMoves();

        foreach (Move move in legalMoves)
        {
            if (move.from == boardManager.PickingPiece.Pos && this.pos == move.to)
            {
                boardManager.HandleMove(this, move);
                return;
            }
        }
    }
}
