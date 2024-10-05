using System.Collections;
using UnityEngine;

public class LegoGame : Minigame
{
    [Header("Lego Game"), SerializeField] 
    private LayerMask      pieceMask;
    [SerializeField] private LegoPlayfield  playfield;
    [SerializeField] private AudioClip      legoSound;
    [SerializeField] private AudioClip      buzzerSound;
    [SerializeField] private ParticleSystem effectPS;

    LegoPiece grabbedPiece;
    int       currentRotation;

    Quaternion[]    predefRotations = new Quaternion[4]
    {
        Quaternion.identity,
        Quaternion.Euler(0, 0, 90),
        Quaternion.Euler(0, 0, 180),
        Quaternion.Euler(0, 0, 270),
    };

    protected override void Start()
    {
        base.Start();

        Invoke(nameof(StartPlaying), 2.0f);
    }

    protected override void Update()
    {
        base.Update();

        if (!isPlaying) return;

        if (Input.GetButtonDown("Fire1") || (playTime > 1.0f))
        {
            DisablePrompt();
        }

        Cursor.visible = (grabbedPiece  == null);

        if (grabbedPiece == null)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // Grab a piece
                GrabPiece();
            }
        }
        else
        {
            Vector2 cursorPos = GetCursorPos();
            if (playfield.IsOnRect(cursorPos))
            {
                // Get tiled pos
                var tilePos = playfield.WorldToTile(cursorPos);

                grabbedPiece.transform.position = playfield.TileToWorld(tilePos);
            }
            else
            {
                grabbedPiece.transform.position = cursorPos;
            }

            grabbedPiece.transform.rotation = Quaternion.RotateTowards(grabbedPiece.transform.rotation, predefRotations[currentRotation], 720.0f * Time.deltaTime);


            if (Input.GetButtonDown("Fire1"))
            {
                // Check if we're on the playfield
                if (playfield.IsOnRect(cursorPos))
                {
                    if (playfield.CanDrop(grabbedPiece))
                    {
                        PlaySound();
                        playfield.Set(grabbedPiece, true);
                        grabbedPiece.SetColliders(true);
                        grabbedPiece.SetOrder(grabbedPiece.originalOrder);
                        grabbedPiece = null;
                    }
                    else
                    {
                        if (buzzerSound) SoundManager.PlaySound(buzzerSound, 0.5f, Random.Range(0.8f, 1.2f));
                    }
                }
                else
                {
                    ReleasePiece();
                    PlaySound();
                }
            }

            if (Input.GetButtonDown("Fire2"))
            {
                currentRotation = (currentRotation + 1) % predefRotations.Length;
                PlaySound();
            }
        }

        if (playfield.isFull)
        {
            Win();
            effectPS.Play();
        }
    }

    void GrabPiece()
    {
        Vector2 cursorPos = GetCursorPos();

        DrawPoint(cursorPos, Color.red);

        Collider2D[] results = Physics2D.OverlapCircleAll(cursorPos, 5, pieceMask);
        if (results.Length > 0)
        {
            // Use the first valid one
            foreach (var result in results)
            {
                LegoPiece legoPiece = result.GetComponentInParent<LegoPiece>();
                if ((legoPiece != null) && (legoPiece.canMove))
                {
                    grabbedPiece = legoPiece;

                    if (playfield.IsOnRect(cursorPos))
                    {
                        // Remove piece from playfield
                        playfield.Set(grabbedPiece, false);
                    }

                    grabbedPiece.SetOrder(5);
                    grabbedPiece.SetColliders(false);

                    currentRotation = GetClosestRotation(grabbedPiece.originalRotation);

                    PlaySound();

                    return;
                }
            }
        }
    }

    void ReleasePiece()
    {
        StartCoroutine(BackToPlaceCR(grabbedPiece, grabbedPiece.originalPosition, grabbedPiece.originalRotation, grabbedPiece.originalOrder));

        grabbedPiece = null;
    }

    IEnumerator BackToPlaceCR(LegoPiece piece,  Vector3 originalPosition, Quaternion originalRotation, int originalOrder)
    {
        while ((Vector3.Distance(piece.transform.position, originalPosition) > 1.0f) ||
               (Quaternion.Angle(piece.transform.rotation, originalRotation) > 1.0f))
        {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, originalPosition, 2000.0f * Time.deltaTime);
            piece.transform.rotation = Quaternion.RotateTowards(piece.transform.rotation, originalRotation, 360.0f * Time.deltaTime);

            yield return null;
        }

        piece.transform.position = originalPosition;
        piece.transform.rotation = originalRotation;

        piece.SetOrder(originalOrder);
        piece.SetColliders(true);

        PlaySound();
    }

    Vector2 GetCursorPos()
    {
        Vector2 mouseCursor = Input.mousePosition;
        Vector2 ret;

        if ((minigameCamera) && (minigameCamera.targetTexture))
        {
            Vector3 viewportPos = new Vector3(mouseCursor.x / Screen.width, mouseCursor.y / Screen.height, minigameCamera.nearClipPlane);

            ret = minigameCamera.ViewportToWorldPoint(viewportPos);
        }
        else
        {
            ret = minigameCamera.ScreenToWorldPoint(mouseCursor);
        }

        return ret;
    }

    int GetClosestRotation(Quaternion rotation)
    {
        float minAngle = float.MaxValue;
        int   ret = -1;

        for (int i = 0; i < predefRotations.Length; i++)
        {
            float angle = Quaternion.Angle(predefRotations[i], rotation);
            if (angle < minAngle)
            {
                minAngle = angle;
                ret = i;
            }
        }

        return ret;
    }

    void PlaySound()
    {
        if (legoSound) SoundManager.PlaySound(legoSound, 0.5f, Random.Range(0.8f, 1.2f));
    }


    static void DrawPoint(Vector3 p, Color c, float t = 1.0f)
    {
        Debug.DrawLine(p + Vector3.up * 5, p + Vector3.down * 5, c, t);
        Debug.DrawLine(p + Vector3.left * 5, p + Vector3.right * 5, c, t);
    }
}
