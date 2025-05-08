using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{   
    public class AccountData
    {
        public string accountNumber;
        public string pin;
        public string firstnamen;
        public string lastnamen;
    }
    public GameObject homeMenu;
    public GameObject actionSelect;
    public GameObject withdrawPage;
    public GameObject transferPage;
    public GameObject changePinPage;
    public GameObject balancePage;
    public GameObject depositPage;
    public GameObject newAccount;
    public GameObject withdraw_otherPage;
    public GameObject displayAccountDetailPage;

    // public Button button;
    public TMP_InputField enterPinInput;
    public TMP_InputField enterAccNumInput;
    public TMP_InputField depositAmountInput;
    public TMP_InputField withdrawAmountInput;
    
    private GameObject[] allPanels;
    private string currentPin;  // Default PIN
    private string newPin;
    public TMP_InputField newPinInput;  // InputField for new PIN
    public TMP_Text statusText;  // Text to show status message
    public TMP_Text balanceOutput;

    // for the create new account page
    public TMP_Text createAccStatusText;
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField pinInput;
    public TMP_Text accountDetailOutput;
    public TMP_Text loginStatusText;

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
            newAccount,
            withdraw_otherPage,
            displayAccountDetailPage,
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

    

    public void OnLoginButtonClick()
    {   
        string enteredAccNum = enterAccNumInput.text.Trim();
        string enteredPin = enterPinInput.text.Trim();

        string savedAccNum = PlayerPrefs.GetString("accountNumber", "");
        string savedPin = PlayerPrefs.GetString("pin", "");
         
         if (enteredAccNum == savedAccNum && enteredPin == savedPin)
         {
            SetActivePanel(actionSelect);
            loginStatusText.text = "";
         }
         else
         {
            loginStatusText.text = "Invalid account number or PIN";
         }
        

        // Debug.Log(acctDets);

        // if (enteredPin == currentPin)
        // {
        //     SetActivePanel(actionSelect);
        //     enterPinInput.text = "";
        // }
    }


     public void OnCreateAccountButtonClick()
    {
        string fname = firstNameInput.text.Trim();
        string lname = lastNameInput.text.Trim();
        string pin = pinInput.text.Trim();

        if (fname == "" || lname == "" || pin == "" || pin.Length != 4)
        {
            createAccStatusText.text = "Please enter all fields. PIN must be 4 digits.";
            return;
        }
        float initialbalance = 0f;
        AccountManager.Instance.SaveAccountdata(fname, lname, pin, initialbalance);

        // createAccStatusText.text = "Account created successfully!";

        string accountNumber = AccountManager.Instance.GenerateAccountNumber();
        AccountManager.Instance.SaveAccount(accountNumber, pin);
    

        //clear fields
        firstNameInput.text = "";
        lastNameInput.text = "";
        pinInput.text = "";

        SetActivePanel(displayAccountDetailPage);

        accountDetailOutput.text = 
        $"Account Created!\n" +
        $"Name: {fname} {lname}\n" +
        $"Account Number: {AccountManager.Instance.GenerateAccountNumber()}";

    }

   
    public void OnDepositAmountButtonClick()
    {   
        string depositAmount = depositAmountInput.text;
        float amount;

        if (float.TryParse(depositAmount, out amount))
        {
            // Successfully parsed
            Debug.Log("Deposit amount: " + amount);
            
            AccountManager.Instance.Deposit(amount);
        }
        else
        {
            // Invalid input
            Debug.LogWarning("Invalid deposit amount entered.");
        }

    }

    public void OnWithdrawAmountButtonClick()
    {
        string withdrawAmount = withdrawAmountInput.text;
        float amountWithdraw;

         if (float.TryParse(withdrawAmount, out amountWithdraw))
        {
            // Successfully parsed
            Debug.Log("Deposit amount: " + amountWithdraw);
            
            AccountManager.Instance.Withdraw(amountWithdraw);
        }
        else
        {
            // Invalid input
            Debug.LogWarning("Invalid deposit amount entered.");
        }

    }

    public void UpdateBalanceDisplay()
    {
        if (balanceOutput != null && AccountManager.Instance != null)
    {
        balanceOutput.text = AccountManager.Instance.GetBalanceFormatted();
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
    


    

    public void OnFiveKButtonClick()
    {
        float cash = 5000;
        AccountManager.Instance.Withdraw(cash);
        
    }
    public void OnTenKButtonClick()
    {
        float cash = 10000;
        AccountManager.Instance.Withdraw(cash);
        
    }
    public void OnTwentyKButtonClick()
    {
        float cash = 20000;
        AccountManager.Instance.Withdraw(cash);
        
    }
    public void OnFiftyKButtonClick()
    {
        float cash = 50000;
        AccountManager.Instance.Withdraw(cash);
        
    }
    // Button click handlers for navigating between panels
    // public void OnLoginButtonClick()
    // {
    //     SetActivePanel(actionSelect);
    // }

   
    

    

    public void OnWithdrawOtherButtonClick()
    {
        SetActivePanel(withdraw_otherPage);
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
        UpdateBalanceDisplay();
    }

    public void OnBackButtonClick()
    {
        // Going back to action select page
        SetActivePanel(actionSelect);
    }

    public void OnNewAccountButtonClick()
    {
        SetActivePanel(newAccount);
    }

    public void OnHomeButtonClick()
    {
        SetActivePanel(homeMenu);
    }

    
}
