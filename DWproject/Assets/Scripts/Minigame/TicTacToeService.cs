using System;
using System.Collections.Generic;
using Naninovel;

[InitializeAtRuntime]
public class TicTacToeService : IEngineService
{
    private const int PlayerMark = 1;
    private const int AiMark = 2;
    private const float BlockChance = 0.5f;
    
    public event Action<int[]> OnBoardChanged;
    public event Action<GameResult> OnGameEnded;
    
    private readonly int[][] WinLines =
    {
        new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8},
        new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8},
        new[] {0,4,8}, new[] {2,4,6}
    };

    private int[] board = new int[9];
    private Random random = new();
    
    public bool GameActive { get; private set; }
    public GameResult Result { get; private set; } = GameResult.None;
    
    public void StartNewGame()
    {
        Array.Clear(board, 0, board.Length);
        Result = GameResult.None;
        GameActive = true;
        OnBoardChanged?.Invoke(board);
    }
    
    public void MakePlayerMove(int cellIndex)
    {
        if (!GameActive || cellIndex is < 0 or > 8 || board[cellIndex] != 0)
        {
            return;
        }
        board[cellIndex] = PlayerMark;
        OnBoardChanged?.Invoke(board);
        if (TryFinishGame(PlayerMark, GameResult.PlayerWon))
        {
            return;
        }
        ContinuePlayingAfterPlayerMove();
    }

    private void ContinuePlayingAfterPlayerMove()
    {
        MakeAiMove();
        OnBoardChanged?.Invoke(board);
        TryFinishGame(AiMark, GameResult.PlayerLost);
    }
    
    private void MakeAiMove()
    {
        int? move = FindWinningMove(AiMark);

        if (move == null && random.NextDouble() < BlockChance)
        {
            move = FindWinningMove(PlayerMark);
        }

        move ??= FindCenterFreeCell(4)                       // strict center check
                 ?? FindFirstFreeAmong(0, 2, 6, 8) // strict corner check
                 ?? FindRandomFreeCell();

        if (move.HasValue)
        {
            board[move.Value] = AiMark;
        }
    }

    private int? FindWinningMove(int mark)
    {
        foreach (var line in WinLines)
        {
            int emptyIdx = -1, markCount = 0;
            foreach (var idx in line)
            {
                if (board[idx] == mark)
                {
                    markCount++;
                }
                else if (board[idx] == 0)
                {
                    emptyIdx = idx;
                }
            }
            if (markCount == 2 && emptyIdx != -1)
            {
                return emptyIdx;
            }
        }
        return null;
    }

    private int? FindCenterFreeCell(int index) => board[index] == 0 ? index : null;

    private int? FindFirstFreeAmong(params int[] candidates)
    {
        foreach (var idx in candidates)
        {
            if (board[idx] == 0)
            {
                return idx;
            }
        }
        return null;
    }

    private int? FindRandomFreeCell()
    {
        var free = new List<int>();
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == 0)
            {
                free.Add(i);
            }
        }
        if (free.Count == 0)
        {
            return null;
        }
        return free[random.Next(free.Count)];
    }
    
    private bool TryFinishGame(int mark, GameResult resultIfWon)
    {
        if (HasWinner(mark))
        {
            GameActive = false;
            Result = resultIfWon;
            OnGameEnded?.Invoke(Result);
            return true;
        }

        if (IsBoardFull())
        {
            GameActive = false;
            Result = GameResult.Draw;
            OnGameEnded?.Invoke(Result);
            return true;
        }

        return false;
    }

    private bool HasWinner(int mark)
    {
        foreach (var line in WinLines)
        {
            if (board[line[0]] == mark && board[line[1]] == mark && board[line[2]] == mark)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsBoardFull()
    {
        foreach (var cell in board)
        {
            if (cell == 0)
            {
                return false;
            }
        }
        return true;
    }

    #region IEngineService
    UniTask IEngineService.InitializeServiceAsync() => UniTask.CompletedTask;

    public void ResetService()
    {
        Array.Clear(board, 0, board.Length);
        GameActive = false;
        Result = GameResult.None;
    }

    public void DestroyService() { }
    #endregion
}
