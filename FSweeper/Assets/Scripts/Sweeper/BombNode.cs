using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombNode
{
    private Grid<BombNode> grid;
    public int x;
    public int y;
    private int bombSurroundCount;
    private bool isBomb;
    private bool isReveal;
    private bool isFlag;
    public BombNode(Grid<BombNode> grid, int x, int y)
    {
        this.x = x;
        this.y = y;
        this.grid = grid;
        isBomb = false;
        isReveal = false;
        isFlag = false;
        bombSurroundCount = 0;
    }

    public Vector2 GetNodeCenterPosition()
    {
        return grid.GetCenterWorldPosition(x, y);
    }

    public Grid<BombNode> GetGrid()
    {
        return grid;
    }
    public override string ToString()
    {
        if (!isBomb) return bombSurroundCount.ToString();
        return "Bomb";
    }

    public int GetBombCount()
    {
        return bombSurroundCount;
    }

    public List<BombNode> GetNeighbourBombNodes()
    {
        return grid.GetAllNeighbourGridObjects(x, y);
    }

    public void SetBomb()
    {
        isBomb = true;
        bombSurroundCount = 0;
    }

    public void UnsetBomb()
    {
        isBomb = false;
    }

    public bool isBombed()
    {
        return isBomb;
    }

    public bool isRevealed()
    {
        return isReveal;
    }
    public void Reveal()
    {
        isReveal = true;
    }

    public bool isFlagged()
    {
        return isFlag;
    }
    public void Flag()
    {
        isFlag = true;
    }

    public void UnFlag()
    {
        isFlag = false;
    }
    public void IncrementBombCount()
    {
        bombSurroundCount++;
    }
}
