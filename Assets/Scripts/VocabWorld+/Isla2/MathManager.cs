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

    // Solo num1, signo, num2 - el resultado se calcula automaticamente
    private int[][,] phases = new int[][,]
    {
        // PLUS: num1, signo, num2
        new int[,] { {9,1,2}, {7,1,5}, {8,1,5}, {6,1,8}, {7,1,8} },
        // MINUS
        new int[,] { {20,2,3}, {19,2,1}, {20,2,1}, {20,2,4}, {18,2,2} },
        // TIMES
        new int[,] { {4,3,3}, {5,3,3}, {4,3,4}, {3,3,6}, {4,3,5} },
        // DIVIDED BY
        new int[,] { {20,4,4}, {18,4,3}, {15,4,5}, {14,4,7}, {12,4,6} }
    };

    private int currentPhase = 0;
    private int activeRow = 0;
    private int[] filledNum1 = new int[5];
    private string[] filledSign = new string[5];
    private int[] filledNum2 = new int[5];
    private bool[] rowCompleted = new bool[5];
    private bool isTalking = false;
    private Vector3 originalScale;
    private float pulseTimer = 0f;
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
        activeRow = 0;

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
        ShowInstruction();
    }

    private void ShowInstruction()
    {
        if (activeRow >= 5) return;

        int n1 = phases[currentPhase][activeRow, 0];
        int sign = phases[currentPhase][activeRow, 1];
        int n2 = phases[currentPhase][activeRow, 2];

        instructionText.text = GetNumberWord(n1) + " " +
            signWords[sign] + " " +
            GetNumberWord(n2) + " equals ?";
    }

    public bool IsRowActive(int row)
    {
        return row == activeRow;
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

    private int CalculateResult(int num1, string sign, int num2)
    {
        switch (sign)
        {
            case "+": return num1 + num2;
            case "-": return num1 - num2;
            case "×": return num1 * num2;
            case "÷": return num2 != 0 ? num1 / num2 : 0;
            default: return 0;
        }
    }

    public void SetFilledNum1(int row, int value)
    {
        if (row == activeRow)
        {
            filledNum1[row] = value;
            CheckRow(row);
        }
    }

    public void SetFilledSign(int row, string sign)
    {
        if (row == activeRow)
        {
            filledSign[row] = sign;
            CheckRow(row);
        }
    }

    public void SetFilledNum2(int row, int value)
    {
        if (row == activeRow)
        {
            filledNum2[row] = value;
            CheckRow(row);
        }
    }

    private void CheckRow(int row)
    {
        if (filledNum1[row] == -1 || filledSign[row] == "" || filledNum2[row] == -1) return;

        int expectedN1 = phases[currentPhase][row, 0];
        int expectedSign = phases[currentPhase][row, 1];
        int expectedN2 = phases[currentPhase][row, 2];

        if (filledNum1[row] == expectedN1 &&
            filledSign[row] == signSymbols[expectedSign] &&
            filledNum2[row] == expectedN2)
        {
            // Calcular resultado automaticamente
            int result = CalculateResult(filledNum1[row], filledSign[row], filledNum2[row]);

            rowCompleted[row] = true;
            slotsResult[row].ShowResult(result);
            feedbackText.text = "Well done!";
            feedbackText.color = Color.green;

            activeRow++;

            if (activeRow < 5)
                ShowInstruction();
            else
                Invoke(nameof(OnPhaseComplete), 1.5f);
        }
        else
        {
            feedbackText.text = "Try again! Follow the instruction.";
            feedbackText.color = Color.red;
            rowToClear = row;
            Invoke(nameof(ClearRowDelayed), 1.5f);
        }
    }

    private void ClearRowDelayed()
    {
        filledNum1[rowToClear] = -1;
        filledSign[rowToClear] = "";
        filledNum2[rowToClear] = -1;
        slotsNum1[rowToClear].ClearSlot();
        slotsSign[rowToClear].ClearSlot();
        slotsNum2[rowToClear].ClearSlot();
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
}