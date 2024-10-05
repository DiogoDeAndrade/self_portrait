using System.Collections.Generic;
using UnityEngine;

public class LegoPiece : MonoBehaviour
{
    [SerializeField] public     bool       canMove = true;
    [SerializeField] public     Vector2Int size = new Vector2Int(5, 5);
    [SerializeField] protected  Vector2Int pivot = new Vector2Int(2, 2);
    [SerializeField] protected  bool[]     tiles = new bool[25];
    [SerializeField] protected  Vector2Int prevSize = new Vector2Int(5, 5);
    [SerializeField] protected  Color      color = Color.red;
    [SerializeField] protected  Sprite     baseSprite;
    [SerializeField] protected  int        defaultSpriteOrder = 0;
    

    protected Vector3       _originalPosition;
    protected Quaternion    _originalRotation;
    protected int           _originalOrder;
    protected Vector2       tileSize;
    protected Vector2       pivotPos;
    protected Bounds        bounds;
    protected Bounds        localBounds;

    public Vector3 originalPosition => _originalPosition;
    public Quaternion originalRotation => _originalRotation;
    public int originalOrder => _originalOrder;

    void Start()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _originalOrder = transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;

        tileSize = new Vector2(baseSprite.rect.width / baseSprite.pixelsPerUnit, baseSprite.rect.height / baseSprite.pixelsPerUnit);
        pivotPos = new Vector2(pivot.x * tileSize.x, (size.y - pivot.y - 1) * tileSize.y);

        ComputeBoundingBox();
    }

    void Update()
    {
        
    }

    private void OnValidate()
    {
        // Force size to be odd
        size.x += (size.x % 2 == 0) ? (1) : (0);
        size.y += (size.y % 2 == 0) ? (1) : (0);

        if ((tiles.Length != size.x * size.y) && (size.x > 0) && (size.y > 0))
        {
            var newTiles = new bool[size.x * size.y];
            if (pivot.x >= size.x) pivot.x = 0;
            if (pivot.y >= size.y) pivot.x = 0;

            for (int y = 0; y < size.y; y++)
            {
                if (y >= prevSize.y) break;
                for (int x = 0; x < size.x; x++)
                {
                    if (x >= prevSize.x) break;

                    newTiles[y * size.x + x] = tiles[y * prevSize.x + x];
                }
            }

            tiles = newTiles;
            prevSize = size;
        }
    }

    public void Rebuild()
    {
        // Delete all children
        List<GameObject> toDestroy = new();
        foreach (Transform t in transform)
        {
            toDestroy.Add(t.gameObject);
        }

        foreach (var obj in toDestroy)
        { 
            DestroyImmediate(obj);
        }

        tileSize = new Vector2(baseSprite.rect.width / baseSprite.pixelsPerUnit, baseSprite.rect.height / baseSprite.pixelsPerUnit);
        pivotPos = new Vector2(pivot.x * tileSize.x, (size.y - pivot.y - 1) * tileSize.y);

        // Build new pieces
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (tiles[y * size.x + x])
                {
                    float px = x * tileSize.x - pivotPos.x;
                    float py = (size.y - y - 1) * tileSize.y - pivotPos.y;

                    GameObject go = new GameObject();
                    go.transform.parent = transform;
                    go.transform.localPosition = new Vector3(px, py, 0);
                    go.transform.localRotation = Quaternion.identity;
                    go.name = $"Piece ({x}, {y})";
                    go.layer = gameObject.layer;
                    SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = baseSprite;
                    sr.color = color;
                    sr.sortingOrder = defaultSpriteOrder;
                    BoxCollider2D bx = go.AddComponent<BoxCollider2D>();
                    bx.isTrigger = true;
                    bx.size = new Vector2(tileSize.x, tileSize.y);
                }
            }
        }
    }

    public void SetOrder(int order)
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in spriteRenderers)
        {
            sr.sortingOrder = order;
        }
    }

    public void SetColliders(bool enableCollider)
    {
        var boxColliders = GetComponentsInChildren<BoxCollider2D>();

        foreach (var bx in boxColliders)
        {
            bx.enabled = enableCollider;
        }
    }

    void ComputeBoundingBox()
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        bounds = new Bounds(transform.position, Vector3.one);
        foreach (var sr in spriteRenderers)
        {
            bounds.Encapsulate(sr.bounds);
        }
        localBounds = new Bounds(Vector3.zero, Vector3.one);
        foreach (var sr in spriteRenderers)
        {
            var b = sr.localBounds;
            b.center += sr.transform.position;
            localBounds.Encapsulate(b);
        }
    }

    public bool IsOnRect(Vector2 pos)
    {
        return bounds.Contains(pos);
    }

    public Vector2Int WorldToTile(Vector2 worldPos)
    {
        var localPos = transform.InverseTransformPoint(worldPos);

        return new Vector2Int(pivot.x + Mathf.FloorToInt(0.5f + localPos.x / tileSize.x), (size.y - Mathf.FloorToInt(0.5f + localPos.y / tileSize.y) - 1) - (size.y - pivot.y - 1));
    }

    public Vector2 TileToWorld(int x, int y)
    {
        var worldPos = new Vector2((x - pivot.x) * tileSize.x, ((size.y - y - 1) - (size.y - pivot.y - 1)) * tileSize.y);

        return transform.TransformPoint(worldPos);
    }

    public Vector2 TileToWorld(Vector2Int tilePos) => TileToWorld(tilePos.x, tilePos.y);

    public bool HasTile(int x, int y) => tiles[x + size.x * y];

    public bool isFull
    {
        get
        {
            foreach (var tile in tiles)
            {
                if (!tile) return false;
            }

            return true;
        }
    }
}
