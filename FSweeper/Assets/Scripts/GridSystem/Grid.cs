using System.Collections.Generic;
using System;
using UnityEngine;
using ZnoKunG.Utils;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;

    public bool showDebug = false;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.originPosition = GetGridCenterPosition();


        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        if (showDebug)
        {
            debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetCenterWorldPosition(x, y), 20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);


            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }

    public void ClearGridDebugger()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                GameObject textMeshGameObject = debugTextArray[x, y].gameObject;
                if (textMeshGameObject != null)
                {
                    GameObject.Destroy(textMeshGameObject);
                }
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public Vector3 GetCenterWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition + Vector3.one * (cellSize / 2);
    }

    public void GetXY(Vector3 WorldPosition, out int x, out int y) { 
        x = Mathf.FloorToInt((WorldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((WorldPosition - originPosition).y / cellSize);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetGridCenterPosition()
    {
        return new Vector3(originPosition.x - (width * cellSize / 2), originPosition.y - (height * cellSize / 2), 0);
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return; // x, y is range from 0 to width - 1 or height - 1

        gridArray[x, y] = value;
        TriggerGridObjectChanged(x, y);
    }

    // Set Value using WorldPosition
    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return default(TGridObject); // If the value is invalid, then return 0

        return gridArray[x, y];
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public List<TGridObject> GetAllGridObjectArray()
    {
        List<TGridObject> gridObjectArray = new List<TGridObject>();
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridObjectArray.Add(GetGridObject(x, y));
            }
        }

        return gridObjectArray;
    }

    public List<TGridObject> GetAllNeighbourGridObjects(int x, int y)
    {
        List<TGridObject> neighbourObjectArray = new List<TGridObject>();
        if (x - 1 >= 0) // Left
        {
            neighbourObjectArray.Add(GetGridObject(x - 1, y));
            if (y - 1 >= 0) // Left Down
            {
                neighbourObjectArray.Add(GetGridObject(x - 1, y - 1));
            }
            if (y + 1 < height) // Left Up
            {
                neighbourObjectArray.Add(GetGridObject(x - 1, y + 1));
            }
        }
        if (x + 1 < width) // Right
        {
            neighbourObjectArray.Add(GetGridObject(x + 1, y));
            if (y - 1 >= 0) // Right Down
            {
                neighbourObjectArray.Add(GetGridObject(x + 1, y - 1));
            }
            if (y + 1 < height) // Right Up
            {
                neighbourObjectArray.Add(GetGridObject(x + 1, y + 1));
            }
        }
        if (y - 1 >= 0) // Down
        {
            neighbourObjectArray.Add(GetGridObject(x, y - 1));
        }
        if (y + 1 < height) // Up
        {
            neighbourObjectArray.Add(GetGridObject(x, y + 1));
        }

        return neighbourObjectArray;
    }

    public List<TGridObject> GetDiagonalNeighbourGridObjects(int x, int y)
    {
        List<TGridObject> neighbourObjectArray = new List<TGridObject>();
        if (x - 1 >= 0) // Left
        {
            if (y - 1 >= 0) // Left Down
            {
                neighbourObjectArray.Add(GetGridObject(x - 1, y - 1));
            }
            if (y + 1 < height) // Left Up
            {
                neighbourObjectArray.Add(GetGridObject(x - 1, y + 1));
            }
        }
        if (x + 1 < width) // Right
        {
            if (y - 1 >= 0) // Right Down
            {
                neighbourObjectArray.Add(GetGridObject(x + 1, y - 1));
            }
            if (y + 1 < height) // Right Up
            {
                neighbourObjectArray.Add(GetGridObject(x + 1, y + 1));
            }
        }

        return neighbourObjectArray;
    }

    public List<TGridObject> GetAxisNeighbourGridObjects(int x, int y)
    {
        List<TGridObject> neighbourObjectArray = new List<TGridObject>();
        if (x - 1 >= 0) // Left
        {
            neighbourObjectArray.Add(GetGridObject(x - 1, y));
        }
        if (x + 1 < width) // Right
        {
            neighbourObjectArray.Add(GetGridObject(x + 1, y));
        }
        if (y - 1 >= 0) // Down
        {
            neighbourObjectArray.Add(GetGridObject(x, y - 1));
        }
        if (y + 1 < height) // Up
        {
            neighbourObjectArray.Add(GetGridObject(x, y + 1));
        }

        return neighbourObjectArray;
    }
}

