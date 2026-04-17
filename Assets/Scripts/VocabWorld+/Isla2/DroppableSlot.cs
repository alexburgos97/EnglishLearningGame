using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DroppableSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;
    public int rowIndex;
    public bool isResultSlot = false;

    private TextMeshProUGUI slotText;
    private Image slotImage;
    private bool isFilled = false;

    void Start()
    {
        slotText = GetComponentInChildren<TextMeshProUGUI>();
        slotImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isResultSlot) return;
        if (isFilled) return;

        if (!MathManager.Instance.IsRowActive(rowIndex)) return;

        DraggableNumber selected = DraggableNumber.selectedNumber;
        if (selected == null) return;

        if (slotIndex == 2 && !selected.isSign) return;
        if (slotIndex != 2 && selected.isSign) return;

        isFilled = true;

        if (slotText != null)
        {
            if (selected.isSign)
                slotText.text = selected.signSymbol;
            else
                slotText.text = selected.numberValue.ToString();
        }

        if (selected.isSign)
            MathManager.Instance.SetFilledSign(rowIndex, selected.signSymbol);
        else if (slotIndex == 1)
            MathManager.Instance.SetFilledNum1(rowIndex, selected.numberValue);
        else if (slotIndex == 3)
            MathManager.Instance.SetFilledNum2(rowIndex, selected.numberValue);

        selected.Deselect();
    }

    public void ClearSlot()
    {
        isFilled = false;
        if (slotText != null)
        {
            slotText.text = "";
            slotText.color = Color.black;
        }
        if (slotImage != null)
            slotImage.color = Color.white;
    }

    public void ShowResult(int result, string resultWord)
    {
        isFilled = true;
        if (slotText != null)
        {
            slotText.text = result + "\n" + resultWord;
            slotText.color = Color.green;
        }
    }

    public void ShowError()
    {
        if (slotText != null)
        {
            slotText.text = "!";
            slotText.color = Color.red;
        }
    }
}