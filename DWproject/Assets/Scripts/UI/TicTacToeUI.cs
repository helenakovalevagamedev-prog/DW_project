using Naninovel;
using Naninovel.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Простейший UI крестиков-ноликов. Наследуется от CustomUI, поэтому
/// автоматически реализует IManagedUI и доступен через
/// Engine.GetService<IUIManager>().GetUI<TicTacToeUI>() — без своего Instance.
///
/// ВАЖНО: чтобы это заработало, префаб с этим компонентом на корне нужно
/// зарегистрировать через Naninovel -> Resources -> UI (аналогично тому,
/// как регистрируются Characters/Backgrounds). На корневом объекте также
/// должен быть Canvas Group (требование CustomUI для управления видимостью/фейдами).
///
/// Ничего не решает сама — только:
///  - рисует состояние поля, когда сервис сообщает об изменении (OnBoardChanged);
///  - пересылает клик игрока в сервис (TryPlayerMove).
/// Вся игровая логика (правила, ИИ, определение победителя) — в MinigameService.
///
/// Разместите 9 кнопок (Cells[0..8], слева направо, сверху вниз) с Text/TMP_Text
/// компонентами для отображения "X"/"O".
/// </summary>
public class TicTacToeUI : CustomUI
{
    [Tooltip("9 кнопок поля по порядку: 0,1,2 / 3,4,5 / 6,7,8")]
    [SerializeField] private Button[] cellButtons = new Button[9];
    [SerializeField] private Text[] cellLabels = new Text[9]; // замените на TMP_Text, если используете TextMeshPro

    private TicTacToeService minigame => Engine.GetService<TicTacToeService>();

    protected override void Awake()
    {
        base.Awake(); // важно вызвать база-реализацию CustomUI

        for (int i = 0; i < cellButtons.Length; i++)
        {
            int cellIndex = i; // важно скопировать в локальную переменную для замыкания
            cellButtons[i].onClick.AddListener(() => OnCellClicked(cellIndex));
        }

        // Подписываемся в Awake, а не в OnEnable: CustomUI может скрывать панель
        // через альфу CanvasGroup (фейд), а не через SetActive(false) — в этом
        // случае OnEnable/OnDisable не сработают надёжно.
        minigame.OnBoardChanged += RenderBoard;
    }

    private void OnDestroy()
    {
        if (minigame != null)
            minigame.OnBoardChanged -= RenderBoard;
    }

    private void OnCellClicked(int cellIndex)
    {
        minigame.TryPlayerMove(cellIndex);
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
// using Naninovel;
// using UnityEngine;
// using UnityEngine.UI;
//
// /// <summary>
// /// Простейший UI крестиков-ноликов. Ничего не решает сам — только:
// ///  - рисует состояние поля, когда сервис сообщает об изменении (OnBoardChanged);
// ///  - пересылает клик игрока в сервис (TryPlayerMove).
// /// Вся игровая логика (правила, ИИ, определение победителя) — в MinigameService.
// ///
// /// Разместите на сцене Canvas с 9 кнопками (Cells[0..8], слева направо, сверху вниз),
// /// у каждой кнопки — Text/TMP_Text компонент для отображения "X"/"O".
// /// Изначально Canvas/панель должна быть выключена (SetActive(false)) — включает
// /// её сам StartMinigameCommand через Show()/Hide().
// /// </summary>
// public class TicTacToeUI : MonoBehaviour
// {
//     public static TicTacToeUI Instance { get; private set; }
//
//     [Tooltip("9 кнопок поля по порядку: 0,1,2 / 3,4,5 / 6,7,8")]
//     [SerializeField] private Button[] cellButtons = new Button[9];
//     [SerializeField] private Text[] cellLabels = new Text[9]; // замените на TMP_Text, если используете TextMeshPro
//
//     [SerializeField] private GameObject root; // корневой объект панели (Canvas или её дочерний контейнер)
//
//     private TicTacToeService minigame => Engine.GetService<TicTacToeService>();
//
//     private void Awake()
//     {
//         Instance = this;
//
//         for (int i = 0; i < cellButtons.Length; i++)
//         {
//             int cellIndex = i; // важно скопировать в локальную переменную для замыкания
//             cellButtons[i].onClick.AddListener(() => OnCellClicked(cellIndex));
//         }
//     }
//
//     private void OnEnable()
//     {
//         if (minigame != null)
//             minigame.OnBoardChanged += RenderBoard;
//     }
//
//     private void OnDisable()
//     {
//         if (minigame != null)
//             minigame.OnBoardChanged -= RenderBoard;
//     }
//
//     public void Show()
//     {
//         root.SetActive(true);
//         RenderBoard(new int[9]); // очистить визуал перед стартом новой партии
//     }
//
//     public void Hide()
//     {
//         root.SetActive(false);
//     }
//
//     private void OnCellClicked(int cellIndex)
//     {
//         minigame.TryPlayerMove(cellIndex);
//     }
//
//     private void RenderBoard(int[] board)
//     {
//         for (int i = 0; i < board.Length; i++)
//         {
//             cellLabels[i].text = board[i] switch
//             {
//                 1 => "X",
//                 2 => "O",
//                 _ => ""
//             };
//             cellButtons[i].interactable = board[i] == 0;
//         }
//     }
// }
