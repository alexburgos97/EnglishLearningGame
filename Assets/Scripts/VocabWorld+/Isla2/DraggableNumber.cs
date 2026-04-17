using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using SpatialSys.UnitySDK;

public class DraggableNumber : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int numberValue;
    public string displayText;
    public bool isSign = false;
    public string signSymbol = "";
    public AudioClip audioClip;

    private AudioSource audioSource;
    private Image buttonImage;
    private Color originalColor;

    public static DraggableNumber selectedNumber = null;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
            originalColor = buttonImage.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayAudio();
        SelectThis();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSign)
            SpatialBridge.coreGUIService.DisplayToastMessage(
                GetNumberWord(numberValue));
        else
            SpatialBridge.coreGUIService.DisplayToastMessage(
                GetSignWord(signSymbol));
    }

    public void OnPointerExit(PointerEventData eventData) { }

    private void SelectThis()
    {
        // Deseleccionar el anterior
        if (selectedNumber != null && selectedNumber != this)
            selectedNumber.Deselect();

        selectedNumber = this;

        // Resaltar en amarillo
        if (buttonImage != null)
            buttonImage.color = Color.yellow;
    }

    public void Deselect()
    {
        if (buttonImage != null)
            buttonImage.color = originalColor;
        if (selectedNumber == this)
            selectedNumber = null;
    }

    private void PlayAudio()
    {
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    private string GetNumberWord(int number)
    {
        string[] words = new string[]
        {
            "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE",
            "SIX", "SEVEN", "EIGHT", "NINE", "TEN",
            "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN",
            "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN", "TWENTY"
        };
        if (number >= 0 && number < words.Length)
            return words[number];
        return number.ToString();
    }

    private string GetSignWord(string sign)
    {
        switch (sign)
        {
            case "+": return "PLUS";
            case "-": return "MINUS";
            case "×": return "TIMES";
            case "÷": return "DIVIDED BY";
            default: return sign;
        }
    }
}