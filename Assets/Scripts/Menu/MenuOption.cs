using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuOption : MonoBehaviour
{
    public TextMeshProUGUI optionName;

    public Button btn;

    internal void Deselect()
    {
        optionName.color = UIManager.Instance.defaultTextColor;
    }

    internal void SetData(string name, MenuOptions option, bool enableButton)
    {
        gameObject.name = name;
        optionName.text = name;
        Deselect();

        btn.onClick.AddListener(() => ActivePanel(option));
        btn.enabled = enableButton;
        if (!enableButton) optionName.color = UIManager.Instance.disabledColor;
    }

    private void ActivePanel(MenuOptions option)
    {
        MenuManager.Instance.SetActiveAllRightPanels(false);
        MenuManager.Instance.SetActive(option, true);
        MenuManager.Instance.DeselectAllOptions();
        optionName.color = UIManager.Instance.selectedColor;
    }

}
