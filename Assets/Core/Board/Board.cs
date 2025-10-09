using NUnit.Framework;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] Tile tilePrefab;
    [SerializeField] BoardManager boardManager;

    void Start()
    {
        if (boardManager == null)
            boardManager = FindAnyObjectByType<BoardManager>();
        GenerateTile(boardManager);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void GenerateTile(BoardManager bm)
    {

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //if (i == j && j == 0) continue;
                Tile tile = Instantiate(tilePrefab, transform);
                tile.Initialize(bm, new BoardPosition(i, j), null);
            }
        }

    }

}
