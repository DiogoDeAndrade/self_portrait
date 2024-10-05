using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LegoPlayfield : LegoPiece
{
    public bool CanDrop(LegoPiece piece)
    {
        for (int y = 0; y < piece.size.y; y++)
        {
            for (int x = 0; x < piece.size.x; x++)
            {
                if (piece.HasTile(x, y))
                {
                    var worldPos = piece.TileToWorld(x, y);
                    var tilePos = WorldToTile(worldPos);

                    if ((tilePos.x < 0) || (tilePos.x >= size.x) || (tilePos.y < 0) || (tilePos.y >= size.y))
                    {
                        return false;
                    }

                    if (tiles[tilePos.x + tilePos.y * size.x])
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void Set(LegoPiece piece, bool set)
    {
        for (int y = 0; y < piece.size.y; y++)
        {
            for (int x = 0; x < piece.size.x; x++)
            {
                if (piece.HasTile(x, y))
                {
                    var worldPos = piece.TileToWorld(x, y);
                    var tilePos = WorldToTile(worldPos);

                    tiles[tilePos.x + tilePos.y * size.x] = set;
                }
            }
        }
    }

}
