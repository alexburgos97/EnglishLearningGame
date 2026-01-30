using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultipleChoiceQuestion : MonoBehaviour
{
    // ---------------------------------------------------------
    // BLOQUE DE PREGUNTA 1
    // ---------------------------------------------------------
    [Header("--- OPCIÓN DE PREGUNTA 1 ---")]
    [SerializeField][TextArea] private string _questionString1;
    [SerializeField] private CorrectAnswer _correctAnswer1; // Respuesta correcta para la opción 1
    [Space(5)]
    [SerializeField] private string _answerStringA1;
    [SerializeField] private string _answerStringB1;
    [SerializeField] private string _answerStringC1;
    [SerializeField] private string _answerStringD1;

    // ---------------------------------------------------------
    // BLOQUE DE PREGUNTA 2
    // ---------------------------------------------------------
    [Header("--- OPCIÓN DE PREGUNTA 2 ---")]
    [SerializeField][TextArea] private string _questionString2;
    [SerializeField] private CorrectAnswer _correctAnswer2; // Respuesta correcta para la opción 2
    [Space(5)]
    [SerializeField] private string _answerStringA2;
    [SerializeField] private string _answerStringB2;
    [SerializeField] private string _answerStringC2;
    [SerializeField] private string _answerStringD2;

    // ---------------------------------------------------------
    // CONFIGURACIÓN GENERAL
    // ---------------------------------------------------------
    [Header("Configuración General")]
    [SerializeField] private bool _hasColoredButton;
    [SerializeField] private Color _correctAnswerColor = Color.green;
    [SerializeField] private Color _IncorrectAnswerColor = Color.red;

    [Space(10)]
    [SerializeField] private int _maxAttempt = 1;
    private int _maxSavedAttempt;
    [SerializeField] private TMP_Text _attempsTextMeshPro;
    [SerializeField] private string _attempConcept = "Attempt";
    private bool _canAnswer = true;

    [Space(10)]
    [Header("Referencias UI (Objetos de la Escena)")]
    [SerializeField] private TMP_Text _questionTextMeshPro;

    [SerializeField] private Button _answerButtonA;
    [SerializeField] private TMP_Text _answerStringATextMeshPro;

    [SerializeField] private Button _answerButtonB;
    [SerializeField] private TMP_Text _answerStringBTextMeshPro;

    [SerializeField] private Button _answerButtonC;
    [SerializeField] private TMP_Text _answerStringCTextMeshPro;

    [SerializeField] private Button _answerButtonD;
    [SerializeField] private TMP_Text _answerStringDTextMeshPro;

    [Space(10)]
    [SerializeField] private UnityEvent _correctAnswerEvent;
    [SerializeField] private UnityEvent _incorrectOneAnswerEvent;
    [SerializeField] private UnityEvent _incorrectAllAnswerEvent;

    private Color originalColor;
    private Button incorrectButton;

    // Esta variable guardará cual es la correcta ACTUALMENTE (dependiendo de si salió la 1 o la 2)
    private CorrectAnswer _activeCorrectAnswer;

    private void Start()
    {
        if (_answerButtonA != null)
            originalColor = _answerButtonA.GetComponent<Image>().color;

        _maxSavedAttempt = _maxAttempt;
        _attempsTextMeshPro.text = $"{_attempConcept}: {_maxAttempt}";

        DisableButtons();
        incorrectButton = null;

        // Asignar funciones a los botones
        _answerButtonA.onClick.AddListener(() => VerifyAnswer(CorrectAnswer.A, _answerButtonA));
        _answerButtonB.onClick.AddListener(() => VerifyAnswer(CorrectAnswer.B, _answerButtonB));
        _answerButtonC.onClick.AddListener(() => VerifyAnswer(CorrectAnswer.C, _answerButtonC));
        _answerButtonD.onClick.AddListener(() => VerifyAnswer(CorrectAnswer.D, _answerButtonD));

        // INICIAR SELECCIÓN ALEATORIA
        SetupRandomQuestion();
    }

    private void SetupRandomQuestion()
    {
        // Random.Range(0, 2) devolverá 0 o 1.
        int coinFlip = Random.Range(0, 2);

        if (coinFlip == 0)
        {
            // CARGAMOS LA OPCIÓN 1
            _questionTextMeshPro.text = _questionString1;
            _answerStringATextMeshPro.text = _answerStringA1;
            _answerStringBTextMeshPro.text = _answerStringB1;
            _answerStringCTextMeshPro.text = _answerStringC1;
            _answerStringDTextMeshPro.text = _answerStringD1;

            // Definimos que la respuesta correcta es la configurada en el bloque 1
            _activeCorrectAnswer = _correctAnswer1;
        }
        else
        {
            // CARGAMOS LA OPCIÓN 2
            _questionTextMeshPro.text = _questionString2;
            _answerStringATextMeshPro.text = _answerStringA2;
            _answerStringBTextMeshPro.text = _answerStringB2;
            _answerStringCTextMeshPro.text = _answerStringC2;
            _answerStringDTextMeshPro.text = _answerStringD2;

            // Definimos que la respuesta correcta es la configurada en el bloque 2
            _activeCorrectAnswer = _correctAnswer2;
        }

        // Habilitar botones si se puede responder
        if (_canAnswer) EnableQuestionInitial();
    }

    private void EnableQuestionInitial()
    {
        _answerButtonA.interactable = true;
        _answerButtonB.interactable = true;
        _answerButtonC.interactable = true;
        _answerButtonD.interactable = true;
    }

    private void VerifyAnswer(CorrectAnswer selectedAnswer, Button button)
    {
        // AQUI COMPARAMOS CON LA VARIABLE DINÁMICA _activeCorrectAnswer
        if (selectedAnswer == _activeCorrectAnswer)
        {
            if (_hasColoredButton)
                button.GetComponent<Image>().color = _correctAnswerColor;

            DisableButtons();
            _canAnswer = false;
            _correctAnswerEvent.Invoke();
        }
        else
        {
            if (_hasColoredButton)
                button.GetComponent<Image>().color = _IncorrectAnswerColor;

            incorrectButton = button;
            incorrectButton.interactable = false;
            _maxAttempt--;
            _attempsTextMeshPro.text = $"{_attempConcept}: {_maxAttempt}";
            _incorrectOneAnswerEvent.Invoke();

            if (_maxAttempt <= 0)
            {
                DisableButtons();
                _canAnswer = false;
                _incorrectAllAnswerEvent.Invoke();
            }
        }
    }

    public void DisableButtons()
    {
        _answerButtonA.interactable = false;
        _answerButtonB.interactable = false;
        _answerButtonC.interactable = false;
        _answerButtonD.interactable = false;
    }

    public void ResetQuestion()
    {
        _maxAttempt = _maxSavedAttempt;
        _attempsTextMeshPro.text = $"{_attempConcept}: {_maxAttempt}";
        incorrectButton = null;

        _answerButtonA.GetComponent<Image>().color = originalColor;
        _answerButtonB.GetComponent<Image>().color = originalColor;
        _answerButtonC.GetComponent<Image>().color = originalColor;
        _answerButtonD.GetComponent<Image>().color = originalColor;

        EnableQuestionInitial();
        _canAnswer = true;

        // OPCIONAL: Si quieres que al resetear vuelva a elegir al azar entre la 1 y la 2,
        // descomenta la siguiente línea:
        // SetupRandomQuestion(); 
    }

    public void EnableQuestion()
    {
        if (_canAnswer)
        {
            if (incorrectButton != _answerButtonA) _answerButtonA.interactable = true;
            if (incorrectButton != _answerButtonB) _answerButtonB.interactable = true;
            if (incorrectButton != _answerButtonC) _answerButtonC.interactable = true;
            if (incorrectButton != _answerButtonD) _answerButtonD.interactable = true;
        }
    }

    public void CanAnswer(bool canAnswer)
    {
        _canAnswer = canAnswer;
        _maxAttempt = _maxSavedAttempt;
    }
}

public enum CorrectAnswer
{
    A,
    B,
    C,
    D
}