using Naninovel;
using Naninovel.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeUI : CustomUI
{
    [SerializeField] private Button[] cellButtons = new Button[9];
    [SerializeField] private TextMeshProUGUI[] cellLabels = new TextMeshProUGUI[9];

    private TicTacToeService minigame => Engine.GetService<TicTacToeService>();

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < cellButtons.Length; i++)
        {
            int cellIndex = i;
            cellButtons[i].onClick.AddListener(() => OnCellClicked(cellIndex));
        }
        minigame.OnBoardChanged += RenderBoard;
    }

    private void OnDestroy()
    {
        if (minigame != null)
        {
            minigame.OnBoardChanged -= RenderBoard;
        }
    }

    private void OnCellClicked(int cellIndex)
    {
        minigame.MakePlayerMove(cellIndex);
    }

    private void RenderBoard(int[] board)
    {
        for (int i = 0; i < board.Length; i++)
        {
            cellLabels[i].text = board[i] switch
            {
                1 => "X",
                2 => "O",
                _ => ""
            };
            cellButtons[i].interactable = board[i] == 0;
        }
    }
}
