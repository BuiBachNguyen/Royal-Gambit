using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpriteDatabase", menuName = "Database/Item Sprite Database")]
public class ItemSpriteDatabase : ScriptableObject
{
    public ItemSpriteData[] items;

    /// <summary>
    /// Trả về Sprite tương ứng với TileStatus và TileColor đã cho.
    /// </summary>
    /// <param name="status">Trạng thái của ô cờ.</param>
    /// <param name="color">Màu của ô cờ (White/Black).</param>
    /// <returns>Sprite tương ứng, hoặc null nếu không tìm thấy.</returns>
    public Sprite GetSprite(TileStatus status, TileColor color)
    {
        foreach (var item in items)
        {
            // Kiểm tra cả trạng thái VÀ màu
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

// Enum mới cho màu ô cờ
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
    public TileColor color; // Thêm trường màu
    public Sprite sprite;
}