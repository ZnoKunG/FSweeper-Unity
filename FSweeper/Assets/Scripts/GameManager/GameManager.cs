using UnityEngine;
using System;
using ZnoKunG.Utils;

public class GameManager : MonoBehaviour
{
    public static Sweeper bombSweeper;
    [SerializeField] private Transform originTransform;
    private int sweeperDimension;

    public static event EventHandler<OnSweeperNodeChangedEventArgs> OnNormalNodeClicked;
    public static event EventHandler<OnSweeperNodeChangedEventArgs> OnBombNodeClicked;
    public static event EventHandler<OnSweeperNodeChangedEventArgs> OnNodeFlagged;
    public static event EventHandler OnWinSweeper;

    public class OnSweeperNodeChangedEventArgs : EventArgs
    {
        public BombNode clickedSweeperNode;
    }
    
    private void Awake()
    {
        sweeperDimension = UIController.dimension;
        DatabaseAccess.SetUrl("https://fsweeperapi-znokung.azurewebsites.net/api/leaderboard");
        int bombCount = Mathf.RoundToInt(sweeperDimension * 1.2f);

        if (sweeperDimension > 0)
        {
            bombSweeper = SweeperVisual.Instance.CreateSweeperWithVisual(sweeperDimension, sweeperDimension, originTransform.position, bombCount);
        }
        else
        {
            bombSweeper = SweeperVisual.Instance.CreateSweeperWithVisual(10, 10, originTransform.position, 12);
        }
        SoundManager.Instance.PlayOneShot("Background Music");
    }
    private void Update()
    {
        if (GameSceneUI.IsPaused()) return;

        if (Input.GetMouseButtonDown(0))
        {
            BombNode bombNode = GetBombNodeFromMousePosition();
            if (bombNode == null || bombNode.isFlagged() || bombNode.isRevealed()) return;

            if (bombNode.isBombed())
            {
                OnBombNodeClicked?.Invoke(this, new OnSweeperNodeChangedEventArgs { clickedSweeperNode = bombNode });
            }
            else
            {
                OnNormalNodeClicked?.Invoke(this, new OnSweeperNodeChangedEventArgs { clickedSweeperNode = bombNode });
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            BombNode bombNode = GetBombNodeFromMousePosition();
            if (bombNode == null) return;

            if (bombNode.isFlagged()) {
                bombNode.UnFlag();
            } else {
                bombNode.Flag();
            }
            OnNodeFlagged?.Invoke(this, new OnSweeperNodeChangedEventArgs { clickedSweeperNode = bombNode });
        }

        if (bombSweeper.IsAllNodesCleared())
        {
            UpdateScoreToLeaderboard((Mathf.Round(GameSceneUI.timer * 100f) / 100f));
            OnWinSweeper?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UpdateScoreToLeaderboard(float score)
    {
        string currentPlayerName = PlayerPrefs.GetString("playerName");
        DatabaseAccess.UpdatePlayer(new Player { name = currentPlayerName, score = score },
            (string error) => Debug.Log(error), (string success) => Debug.Log(success));
    }
    private BombNode GetBombNodeFromMousePosition()
    {
        Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition2D();
        BombNode bombNode = bombSweeper.GetGrid().GetGridObject(mouseWorldPosition);
        return bombNode;
    }

    public static void ReloadSweeper()
    {
        bombSweeper = SweeperVisual.Instance.CreateSavedSweeperWithVisual();
    }
}
