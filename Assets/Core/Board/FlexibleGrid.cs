using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup), typeof(RectTransform))]
//if you read this file, You bị con mèo
public class FlexibleGrid : MonoBehaviour
{
    void Update()
    {
        var rect = GetComponent<RectTransform>().rect;
        var grid = GetComponent<GridLayoutGroup>();

        float cellSize = rect.width / 8f; 
        grid.cellSize = new Vector2(cellSize, cellSize);
    }
}