using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpriteDatabase", menuName = "Database/Item Sprite Database")]
public class ItemSpriteDatabase : ScriptableObject
{
    public ItemSpriteData[] items;

    /// <summary>
    /// Return matching Sprite with TileStatus and TileCorlor
    /// </summary>
    /// <param name="status">Status of tile</param>
    /// <param name="color">Color of pieces (White/Black).</param>
    /// <returns>Matched Sprite or null if not found .</returns>
    public Sprite GetSprite(TileStatus status, TileColor color)
    {
        foreach (var item in items)
        {
            if (item.status == status && item.color == color)
                return item.sprite;
        }
        return null;
    }
}

// -------------------------------------------------------------------

public enum TileStatus
{
    NoAct,
    Act,
    CheckMate
}

public enum TileColor
{
    White,
    Black
}

// -------------------------------------------------------------------

[System.Serializable]
public class ItemSpriteData
{
    public TileStatus status;
    public TileColor color; 
    public Sprite sprite;
}