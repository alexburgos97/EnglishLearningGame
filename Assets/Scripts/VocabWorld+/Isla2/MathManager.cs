using UnityEngine;
using TMPro;
using SpatialSys.UnitySDK;

public class MathManager : MonoBehaviour
{
    public static MathManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject syntaxSprite;
    public AudioSource audioSource;
    public GameObject puente_Isla3;

    [Header("UI")]
    public GameObject mathPanel;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI feedbackText;

    [Header("Slots por fila")]
    public DroppableSlot[] slotsNum1 = new DroppableSlot[5];
    public DroppableSlot[] slotsSign = new DroppableSlot[5];
    public DroppableSlot[] slotsNum2 = new DroppableSlot[5];
    public DroppableSlot[] slotsResult = new DroppableSlot[5];

    private string[] signWords = {"", "PLUS", "MINUS", "TIMES", "DIVIDED BY"};
    private string[] signSymbols = {"", "+", "-", "×", "÷"};

    private int[][,] phases = new int[][,]
    {
        new int[,] { {9,1,2,11}, {7,1,5,12}, {8,1,5,13}, {6,1,8,14}, {7,1,8,15} },
        new int[,] { {20,2,3,17}, {19,2,1,18}, {20,2,1,19}, {20,2,4,16}, {18,2,2,16} },
        new int[,] { {4,3,3,12}, {5,3,3,15}, {4,3,4,16}, {3,3,6,18}, {4,3,5,20} },
        new int[,] { {22,4,2,11}, {26,4,2,13}, {28,4,2,14}, {34,4,2,17}, {38,4,2,19} }
    };

    private int currentPhase = 0;
    private int[] rowOrder = new int[5];
    private int[] filledNum1 = new int[5];
    private string[] filledSign = new string[5];
    private int[] filledNum2 = new int[5];
    private bool[] rowCompleted = new bool[5];
    private int completedRows = 0;
    private bool isTalking = false;
    private Vector3 originalScale;
    private float pulseTimer = 0f;
    private int currentInstructionRow = 0;
    private int rowToClear = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (puente_Isla3 != null)
            puente_Isla3.SetActive(false);
        mathPanel.SetActive(false);
        originalScale = syntaxSprite.transform.localScale;
        audioSource = syntaxSprite.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = syntaxSprite.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!isTalking) return;
        pulseTimer += Time.deltaTime * 5f;
        float pulse = 1f + Mathf.Sin(pulseTimer) * 0.1f;
        syntaxSprite.transform.localScale = originalScale * pulse;
        if (!audioSource.isPlaying)
        {
            isTalking = false;
            syntaxSprite.transform.localScale = originalScale;
        }
    }

    public void StartMath()
    {
        currentPhase = 0;
        mathPanel.SetActive(true);
        StartPhase();
    }

    private void StartPhase()
    {
        completedRows = 0;
        currentInstructionRow = 0;

        rowOrder = new int[] {0, 1, 2, 3, 4};
        for (int i = 4; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = rowOrder[i];
            rowOrder[i] = rowOrder[j];
            rowOrder[j] = temp;
        }

        for (int i = 0; i < 5; i++)
        {
            filledNum1[i] = -1;
            filledSign[i] = "";
            filledNum2[i] = -1;
            rowCompleted[i] = false;
            slotsNum1[i].ClearSlot();
            slotsSign[i].ClearSlot();
            slotsNum2[i].ClearSlot();
            slotsResult[i].ClearSlot();
        }

        feedbackText.text = "";
        ShowCurrentInstruction();
    }

    private void ShowCurrentInstruction()
    {
        if (currentInstructionRow >= 5) return;

        int eq = rowOrder[currentInstructionRow];
        int n1 = phases[currentPhase][eq, 0];
        int sign = phases[currentPhase][eq, 1];
        int n2 = phases[currentPhase][eq, 2];

        instructionText.text = GetNumberWord(n1) + " " +
            signWords[sign] + " " +
            GetNumberWord(n2) + " equals ?";
    }

    private string GetNumberWord(int number)
    {
        string[] words = new string[]
        {
            "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE",
            "SIX", "SEVEN", "EIGHT", "NINE", "TEN",
            "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN",
            "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN", "TWENTY",
            "TWENTY-ONE", "TWENTY-TWO", "TWENTY-THREE", "TWENTY-FOUR",
            "TWENTY-FIVE", "TWENTY-SIX", "TWENTY-SEVEN", "TWENTY-EIGHT",
            "TWENTY-NINE", "THIRTY", "THIRTY-ONE", "THIRTY-TWO",
            "THIRTY-THREE", "THIRTY-FOUR", "THIRTY-FIVE", "THIRTY-SIX",
            "THIRTY-SEVEN", "THIRTY-EIGHT", "THIRTY-NINE"
        };
        if (number >= 0 && number < words.Length)
            return words[number];
        return number.ToString();
    }

    public void SetFilledNum1(int row, int value)
    {
        if (row >= 0 && row < 5)
        {
            filledNum1[row] = value;
            CheckRow(row);
        }
    }

    public void SetFilledSign(int row, string sign)
    {
        if (row >= 0 && row < 5)
        {
            filledSign[row] = sign;
            CheckRow(row);
        }
    }

    public void SetFilledNum2(int row, int value)
    {
        if (row >= 0 && row < 5)
        {
            filledNum2[row] = value;
            CheckRow(row);
        }
    }

    private void CheckRow(int row)
    {
        if (rowCompleted[row]) return;
        if (filledNum1[row] == -1 || filledSign[row] == "" || filledNum2[row] == -1) return;

        int eq = rowOrder[row];
        int expectedN1 = phases[currentPhase][eq, 0];
        int expectedSign = phases[currentPhase][eq, 1];
        int expectedN2 = phases[currentPhase][eq, 2];
        int expectedResult = phases[currentPhase][eq, 3];

        if (filledNum1[row] == expectedN1 &&
            filledSign[row] == signSymbols[expectedSign] &&
            filledNum2[row] == expectedN2)
        {
            rowCompleted[row] = true;
            completedRows++;
            slotsResult[row].ShowResult(expectedResult, GetNumberWord(expectedResult));
            feedbackText.text = "Well done!";
            feedbackText.color = Color.green;

            currentInstructionRow++;
            if (currentInstructionRow < 5)
                ShowCurrentInstruction();

            if (completedRows >= 5)
                Invoke(nameof(OnPhaseComplete), 1.5f);
        }
        else
        {
            feedbackText.text = "Try again!";
            feedbackText.color = Color.red;
            rowToClear = row;
            Invoke(nameof(ClearRowDelayed), 1.5f);
        }
    }

    private void ClearRowDelayed()
    {
        ClearRow(rowToClear);
    }

    private void ClearRow(int row)
    {
        filledNum1[row] = -1;
        filledSign[row] = "";
        filledNum2[row] = -1;
        slotsNum1[row].ClearSlot();
        slotsSign[row].ClearSlot();
        slotsNum2[row].ClearSlot();
        feedbackText.text = "";
    }

    private void OnPhaseComplete()
    {
        currentPhase++;
        if (currentPhase >= 4)
        {
            OnMathComplete();
            return;
        }
        feedbackText.text = "Great! Next operation!";
        feedbackText.color = Color.white;
        Invoke(nameof(StartPhase), 2f);
    }

    private void OnMathComplete()
    {
        mathPanel.SetActive(false);
        if (puente_Isla3 != null)
            puente_Isla3.SetActive(true);
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Amazing! You mastered the numbers!");
        GameProgressManager.Instance.AwardNumberCruncherMedal();
    }
    public bool IsRowActive(int row)
{
    return row == currentInstructionRow;
}
}