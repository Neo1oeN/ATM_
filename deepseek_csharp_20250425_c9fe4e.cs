using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject homeMenu;
    public GameObject actionSelect;
    public GameObject withdrawPage;
    public GameObject transferPage;
    public GameObject changePinPage;
    public GameObject balancePage;
    public GameObject depositPage;

    public Button button;

    private GameObject[] allPanels;
    private string currentPin = "1234";  // Default PIN
    private string newPin;
    public InputField newPinInput;  // InputField for new PIN
    public Text statusText;  // Text to show status message

    void Start()
    {
        allPanels = new GameObject[]
        {
            homeMenu,
            actionSelect,
            withdrawPage,
            transferPage,
            changePinPage,
            balancePage,
            depositPage,
        };

        // Show the homepage first
        foreach (GameObject panel in allPanels)
        {
            panel.SetActive(panel == homeMenu);
        }
    }

    public void SetActivePanel(GameObject panelActivate)
    {
        foreach (GameObject panel in allPanels)
        {
            panel.SetActive(panel == panelActivate);
        }
    }

    // Button click handler to change the PIN
    public void OnConfirmChangePinButtonClick()
    {
        newPin = newPinInput.text;

        if (IsValidPin(newPin))
        {
            currentPin = newPin;
            statusText.text = "PIN changed successfully!";
        }
        else
        {
            statusText.text = "Invalid PIN. Please try again.";
        }
    }

    // Validate the new PIN (for example, it must be 4 digits)
    private bool IsValidPin(string pin)
    {
        return pin.Length == 4;
    }

    // Button click handlers for navigating between panels
    public void OnLoginButtonClick()
    {
        SetActivePanel(actionSelect);
    }

    public void OnWithdrawButtonClick()
    {
        SetActivePanel(withdrawPage);
    }

    public void OnDepositButtonClick()
    {
        SetActivePanel(depositPage);
    }

    public void OnTransferButtonClick()
    {
        SetActivePanel(transferPage);
    }

    public void OnChangePinButtonClick()
    {
        SetActivePanel(changePinPage);  // Navigate to the change PIN page
    }

    public void OnCheckBalanceButtonClick()
    {
        SetActivePanel(balancePage);
    }

    public void OnBackButtonClick()
    {
        // Going back to action select page
        SetActivePanel(actionSelect);
    }

    public void OnHomeButtonClick()
    {
        SetActivePanel(homeMenu);
    }
}
