using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{


    public Piece piecePrefab;
    Piece activePiece;
    public Tilemap tilemap;
    public TetrisManager tetrisManager;

    public TetronimoData[] tetronimos;
    public Vector2Int boardSize;
    public Vector2Int startPostion;

    public float dropInterval = 0.5f;

    float dropTime = 0.0f;

    // Maps tilemap postion to a piece gameObject
    Dictionary<Vector3Int, Piece> pieces = new Dictionary<Vector3Int, Piece>();

    int left
    {
        get { return -boardSize.x / 2; }
    }

    int right
    {
        get { return boardSize.x / 2; }
    }

    int top
    {
        get { return boardSize.y / 2; }
    }

    int bottom
    {
        get { return -boardSize.y / 2; }
    }

    private void Update()
    {
        if (tetrisManager.gameOver) return;

        dropTime += Time.deltaTime;

        if (dropTime >= dropInterval)
        {
            dropTime = 0.0f;

            Clear(activePiece);
            bool moveResult = activePiece.Move(Vector2Int.down);
            Set(activePiece);

            if (!moveResult)
            {
                activePiece.freeze = true;

                CheckBoard();
                SpawnPiece();
            }
        }
    }

    public void SpawnPiece()
    {
        activePiece = Instantiate(piecePrefab);

        Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);

        activePiece.Initialize(this, t);

        CheckEndGame();

        Set(activePiece);
    }

    void CheckEndGame()
    {
        if (!IsPositionValid(activePiece, activePiece.position))
        {
            tetrisManager.SetGameOver(true);
        }
    }

    public void UpdateGameOver()
    {
        if (!tetrisManager.gameOver)
        {
            ResetBoard();
        }
    }

    void ResetBoard()
    {
        Piece[] foundPieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);

        foreach (Piece piece in foundPieces) Destroy(piece.gameObject);

        activePiece = null;

        tilemap.ClearAllTiles();

        pieces.Clear();

        SpawnPiece();
    }

    void SetTile(Vector3Int cellPosition, Piece piece)
    {
        if (piece == null)
        {
            tilemap.SetTile(cellPosition, null);

            pieces.Remove(cellPosition);
        }
        else
        {
            tilemap.SetTile(cellPosition, piece.data.tile);
            pieces[cellPosition] = piece;
        }
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, piece);
        }
    }
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, null);
        }
    }
    public bool IsPositionValid(Piece piece, Vector2Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + position);


            if (cellPosition.x < left || cellPosition.x >= right ||

              cellPosition.y < bottom || cellPosition.y >= top) return false;

            if (tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    public int CheckBoard()
        //Changed the Void to be an Int so we can return the Cleared lines
    {
        //checks if line is full and clears it
        List<int> destroyedLines = new List<int>();
        for (int y = bottom; y < top; y++)
        {
            if (IsLineFull(y))
            {
                DestroyLine(y);
                destroyedLines.Add(y);
            }
        }

        int rowsShiftedDown = 0;
        foreach (int y in destroyedLines)
        {
            ShiftRowsDown(y - rowsShiftedDown);
            rowsShiftedDown++;
        }
        int linesCleared = destroyedLines.Count;
        //Counts if a line was cleared
        if (linesCleared > 0)
        {
            //if a line was cleared and the active piece was Y slows drop intreval then after the next piece is placed then reset
            if (activePiece != null && activePiece.data.tetronimo == Tetronimo.Y)
            {

            }
        }
        int score = tetrisManager.CalculateScore(destroyedLines.Count);
        tetrisManager.ChangeScore(score);

        return linesCleared;
    }

    void ShiftRowsDown(int clearedRow)
    {
        for (int y = clearedRow + 1; y < top; y++)
        {
            for (int x = left; x < right; x++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);

                if (pieces.ContainsKey(cellPosition))
                {
                    Piece currentPiece = pieces[cellPosition];
                    SetTile(cellPosition, null);


                    cellPosition.y -= 1;
                    SetTile(cellPosition, currentPiece);
                }
            }
        }
    }

    bool IsLineFull(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);
            if (!tilemap.HasTile(cellPosition)) return false;
        }

        return true;
    }
    void DestroyLine(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);


            if (pieces.ContainsKey(cellPosition))
            {
                Piece piece = pieces[cellPosition];

                piece.ReduceActiveCount();

                SetTile(cellPosition, null);
            }
        }
    }
}
