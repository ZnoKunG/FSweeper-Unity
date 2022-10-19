using System.Collections.Generic;
using UnityEngine;

public class Sweeper
{
    private int width;
    private int height;
    private Vector3 originPosition;
    private int bombCount;
    private Grid<BombNode> grid;
    private List<BombNode> bombList;

    private const int SWEEPER_SIZE = 100;
    public Sweeper(int width, int height, Vector3 originPosition, int bombCount)
    {
        int cellSize = Mathf.RoundToInt(SWEEPER_SIZE / width);
        grid = new Grid<BombNode>(width, height, cellSize, originPosition, (Grid<BombNode> g, int x, int y) => new BombNode(g, x, y));
        this.width = width;
        this.height = height;
        this.originPosition = originPosition;
        this.bombCount = bombCount;
        RandomBomb(bombCount);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BombNode bombNode = grid.GetGridObject(x, y);
                IncrementNeighbourBombCount(bombNode);
                grid.TriggerGridObjectChanged(x, y);
            }
        }
    }

    private void RandomBomb(int bombCount)
    {
        bombList = new List<BombNode>();
        for (int i = 0; i < bombCount; i++)
        {
            BombNode randomBomb = randomBombNode();
            bombList.Add(randomBomb);
        }
    }

    private BombNode randomBombNode()
    {
        int randX = Random.Range(0, grid.GetWidth());
        int randY = Random.Range(0, grid.GetHeight());
        BombNode bombNode = GetNode(randX, randY);
        bombNode.SetBomb();
        return bombNode;
    }

    private void IncrementNeighbourBombCount(BombNode currentNode)
    {
        if (currentNode.isBombed()) return;
        List<BombNode> neighbourBombNodeList = grid.GetAllNeighbourGridObjects(currentNode.x, currentNode.y);
        foreach (BombNode neighbourBombNode in neighbourBombNodeList)
        {
            if (neighbourBombNode.isBombed()) currentNode.IncrementBombCount();
        }
    }
    public BombNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public Grid<BombNode> GetGrid()
    {
        return grid;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public Vector3 GetOriginPosition()
    {
        return originPosition;
    }

    public int GetBombCount()
    {
        return bombCount;
    }

    public bool IsAllNodesCleared()
    {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (GetNode(x, y).isBombed()) continue;
                if (!GetNode(x, y).isRevealed()) return false;
            }
        }
        return true;
    }
    
    public void ClearGridDebugger()
    {
        grid.ClearGridDebugger();
    }
}
