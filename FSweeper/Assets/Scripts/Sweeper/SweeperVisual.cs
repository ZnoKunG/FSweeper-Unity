using System;
using System.Collections.Generic;
using UnityEngine;
using ZnoKunG.Utils;

public class SweeperVisual : MonoBehaviour
{
    private Grid<BombNode> grid;

    [SerializeField] private Material bombNodeMaterial;
    public static SweeperVisual Instance;

    private GameObject sweeperVisualGameObject;
    private Mesh sweeperMesh;

    private int sweeperWidth;
    private int sweeperHeight;
    private Vector3 sweeperOriginPosition;
    private int sweeperBombCount;

    private bool isTextureUVSetted;
    private const int coverUVIndex = 5;
    private const int normalBombUVIndex = 6;
    private const int redBombUVIndex = 7;
    private const int flagUVIndex = 8;
    private const float textureWidth = 4500f;
    private const float oneDivisionWidth = 500f;
    private Vector3 quadSize;
    private Vector2 redBombTextureUV00;
    private Vector2 redBombTextureUV11;
    private Vector2 normalBombTextureUV00;
    private Vector2 normalBombTextureUV11;
    private Vector2 coverTextureUV00;
    private Vector2 coverTextureUV11;
    private Vector2 flagTextureUV00;
    private Vector2 flagTextureUV11;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    private void SubscribeNodeInteractionEvents()
    {
        GameManager.OnNormalNodeClicked += Sweeper_OnNormalNodeClicked;
        GameManager.OnBombNodeClicked += Sweeper_OnBombNodeClicked;
        GameManager.OnNodeFlagged += Sweeper_OnNodeFlagged;
    }
    public void InitializeSweeperGrid(Sweeper sweeper)
    {
        grid = sweeper.GetGrid();
        if (!isTextureUVSetted) SetBombNodeTextureUVs();
        SubscribeNodeInteractionEvents();
        CreateInitialSweeperVisual();
    }

    public Sweeper CreateSweeperWithVisual(int width, int height, Vector3 originPosition, int bombCount)
    {
        Sweeper sweeper = new Sweeper(width, height, originPosition, bombCount);
        InitializeSweeperGrid(sweeper);
        SaveSweeperProperty(sweeper);
        return sweeper;
    }

    public Sweeper CreateSavedSweeperWithVisual()
    {
        return CreateSweeperWithVisual(sweeperWidth, sweeperHeight, sweeperOriginPosition, sweeperBombCount);
    }

    private void SaveSweeperProperty(Sweeper sweeper)
    {
        sweeperHeight = sweeper.GetHeight();
        sweeperWidth = sweeper.GetWidth();
        sweeperOriginPosition = sweeper.GetOriginPosition();
        sweeperBombCount = sweeper.GetBombCount();
    }

    private void SetBombNodeTextureUVs()
    {
        quadSize = Vector3.one * grid.GetCellSize();

        redBombTextureUV00 = new Vector2(oneDivisionWidth * redBombUVIndex / textureWidth, 0);
        redBombTextureUV11 = new Vector2(oneDivisionWidth * (redBombUVIndex + 1) / textureWidth, 1);
        normalBombTextureUV00 = new Vector2(oneDivisionWidth * normalBombUVIndex / textureWidth, 0);
        normalBombTextureUV11 = new Vector2(oneDivisionWidth * (normalBombUVIndex + 1) / textureWidth, 1);
        coverTextureUV00 = new Vector2(oneDivisionWidth * coverUVIndex / textureWidth, 0);
        coverTextureUV11 = new Vector2(oneDivisionWidth * (coverUVIndex + 1) / textureWidth, 1);
        flagTextureUV00 = new Vector2(oneDivisionWidth * flagUVIndex / textureWidth, 0);
        flagTextureUV11 = new Vector2(oneDivisionWidth * (flagUVIndex + 1) / textureWidth, 1);

        isTextureUVSetted = true;
    }

    private void Sweeper_OnNormalNodeClicked(object sender, GameManager.OnSweeperNodeChangedEventArgs eventArgs)
    {
        BombNode clickedBombNode = eventArgs.clickedSweeperNode;
        RevealCurrentBombNodeAndNeighbour(clickedBombNode);
        UpdateSweeperVisual(clickedBombNode);
    }

    private void Sweeper_OnBombNodeClicked(object sender, GameManager.OnSweeperNodeChangedEventArgs eventArgs)
    {
        BombNode clickedBombNode = eventArgs.clickedSweeperNode;
        HandleClickOnBombNode();
        UpdateSweeperVisual(clickedBombNode);
    }

    private void Sweeper_OnNodeFlagged(object sender, GameManager.OnSweeperNodeChangedEventArgs eventArgs)
    {
        BombNode clickedSweeperNode = eventArgs.clickedSweeperNode;
        UpdateSweeperVisual(clickedSweeperNode);
    }

    private void UpdateSweeperVisual(BombNode clickedSweeperNode)
    {
        GetMeshComponents(out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        List<BombNode> AllSweeperNodeList = grid.GetAllGridObjectArray();
        foreach (BombNode sweeperNode in AllSweeperNodeList)
        {
            int index = sweeperNode.x * grid.GetHeight() + sweeperNode.y;

            if (!sweeperNode.isRevealed() && sweeperNode == clickedSweeperNode) // ClickedNode (maybe) change UV to Flag Texture
            {
                if (sweeperNode.isFlagged()) {
                    World_Mesh.SetMeshComponentsWithIndex(vertices, uv, triangles, index, clickedSweeperNode.GetNodeCenterPosition(), 0f, quadSize, flagTextureUV00, flagTextureUV11);
                } else {
                    World_Mesh.SetMeshComponentsWithIndex(vertices, uv, triangles, index, clickedSweeperNode.GetNodeCenterPosition(), 0f, quadSize, coverTextureUV00, coverTextureUV11);
                }
                continue;
            }
            if (!sweeperNode.isRevealed()) continue; // Other nodes no change applied

            if (sweeperNode.isBombed())
            {
                if (sweeperNode == clickedSweeperNode) // Clicked BombNode
                {
                    World_Mesh.SetMeshComponentsWithIndex(vertices, uv, triangles, index, sweeperNode.GetNodeCenterPosition(), 0f, quadSize, redBombTextureUV00, redBombTextureUV11);
                }

                if (sweeperNode != clickedSweeperNode) // Unclicked BombNode
                {
                    World_Mesh.SetMeshComponentsWithIndex(vertices, uv, triangles, index, sweeperNode.GetNodeCenterPosition(), 0f, quadSize, normalBombTextureUV00, normalBombTextureUV11);
                }
            }

            if (!sweeperNode.isBombed()) // Normal BombCount Node
            {
                GetBombCountTextureUV(sweeperNode, out Vector2 numberTextureUV00, out Vector2 numberTextureUV11);
                World_Mesh.SetMeshComponentsWithIndex(vertices, uv, triangles, index, sweeperNode.GetNodeCenterPosition(), 0f, quadSize, numberTextureUV00, numberTextureUV11);
            }
        }

        UpdateMeshComponents(vertices, uv, triangles);
    }
    private void CreateInitialSweeperVisual()
    {
        if (sweeperVisualGameObject == null)
        {
            sweeperVisualGameObject = new GameObject("Sweeper Visual", typeof(MeshFilter), typeof(MeshRenderer));
        }
        sweeperMesh = sweeperVisualGameObject.GetComponent<MeshFilter>().mesh;
        World_Mesh.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                BombNode bombNode = grid.GetGridObject(x, y);
                World_Mesh.SetMeshComponentsWithIndex(vertices, uv, triangles, index, bombNode.GetNodeCenterPosition(), 0f, quadSize, coverTextureUV00, coverTextureUV11);
            }
        }

        UpdateMeshComponents(vertices, uv, triangles, bombNodeMaterial);
    }
    private void GetBombCountTextureUV(BombNode bombNode, out Vector2 uv00, out Vector2 uv11)
    {
        int bombCount = bombNode.GetBombCount();
        uv00 = new Vector2(oneDivisionWidth * bombCount / textureWidth, 0);
        uv11 = new Vector2(oneDivisionWidth * (bombCount + 1) / textureWidth, 1);
        
    }
    private void GetMeshComponents(out Vector3[] vertices, out Vector2[] uv, out int[] triangles)
    {
        vertices = sweeperMesh.vertices;
        uv = sweeperMesh.uv;
        triangles = sweeperMesh.triangles;
    }

    private void UpdateMeshComponents(Vector3[] vertices, Vector2[] uv, int[] triangles, Material material = null)
    {
        sweeperMesh.vertices = vertices;
        sweeperMesh.uv = uv;
        sweeperMesh.triangles = triangles;
        sweeperMesh.RecalculateBounds();
        if (material != null) sweeperVisualGameObject.GetComponent<MeshRenderer>().material = material;
    }

    private void RevealCurrentBombNodeAndNeighbour(BombNode clickedBombNode)
    {
        clickedBombNode.Reveal();
        RevealNeighbourBombNodesRecursive(clickedBombNode);
    }
    private void RevealNeighbourBombNodesRecursive(BombNode currentBombNode)
    {
        List<BombNode> neighbourNodeList = currentBombNode.GetNeighbourBombNodes();
        foreach (BombNode neighbourBombNode in neighbourNodeList)
        {
            if (neighbourBombNode.isRevealed() || neighbourBombNode.isBombed()) continue;

            if (currentBombNode.GetBombCount() == 0)
            {
                if (neighbourBombNode.GetBombCount() > 0) neighbourBombNode.Reveal();
                else
                {
                    neighbourBombNode.Reveal();
                    RevealNeighbourBombNodesRecursive(neighbourBombNode);
                }
            }
        }
    }
    private void RevealAllRealBombNodes()
    {
        List<BombNode> bombNodesArray = grid.GetAllGridObjectArray();

        foreach (BombNode bombNode in bombNodesArray)
        {
            if (bombNode.isBombed()) bombNode.Reveal();
        }
    }

    private void HandleClickOnBombNode()
    {
        RevealAllRealBombNodes();
        Debug.Log("You Lose !");
    }
}
