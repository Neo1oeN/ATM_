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

    public TMP_InputField enterPinInput;
    public TMP_InputField enterAccNumInput;
    public TMP_InputField depositAmountInput;
    public TMP_InputField withdrawAmountInput;
    
    private GameObject[] allPanels;
    private string currentPin;
    private string newPin;
    public TMP_InputField newPinInput;
    public TMP_Text statusText;
    public TMP_Text balanceOutput;
    public TMP_Text transferStatusText;
    public TMP_Text withdrawStatusText;
    public TMP_Text depositStatusText;

    public TMP_Text createAccStatusText;
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField pinInput;
    public TMP_InputField transferAccNumInput;
    public TMP_InputField transferAmountInput;
    public TMP_InputField accountDetailOutput;
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

        ClearAllInputFields(panelActivate);
    }

    public void OnLoginButtonClick()
    {
        string enteredAccNum = enterAccNumInput.text.Trim();
        string enteredPin = enterPinInput.text.Trim();

        Debug.Log($"🔐 Attempting login - Account: {enteredAccNum}, PIN: {enteredPin}");

        AccountManager.AccountData account = AccountManager.Instance.LoadAccountFromPrefs(enteredAccNum);

        if (account != null)
        {
            Debug.Log($"🔎 Found account - PIN on file: {account.pin}");

            if (account.pin == enteredPin)
            {
                SetActivePanel(actionSelect);
                loginStatusText.text = "";

                AccountManager.Instance.CurrentLoggedInAccount = enteredAccNum;

                Debug.Log("✅ Login successful!");
            }
            else
            {
                loginStatusText.text = "❌ Invalid PIN.";
                Debug.Log("❌ Login failed: Incorrect PIN.");
            }
        }
        else
        {
            loginStatusText.text = "❌ Account not found.";
            Debug.Log("❌ Login failed: Account not found.");
        }
    }

    public void OnCreateAccountButtonClick()
    {
        string fname = firstNameInput.text.Trim();
        string lname = lastNameInput.text.Trim();
        string pin = pinInput.text.Trim();

        Debug.Log($"🟡 Creating Account - First: {fname}, Last: {lname}, PIN: {pin}");

        if (fname == "" || lname == "" || pin == "" || pin.Length != 4)
        {
            createAccStatusText.text = "Please enter all fields. PIN must be 4 digits.";
            Debug.Log("❌ Failed to create account: Missing fields or invalid PIN.");
            return;
        }

        float initialBalance = 0f;
        AccountManager.Instance.SaveAccountdata(fname, lname, pin, initialBalance);

        string accountNumber = AccountManager.Instance.accountNumber;
        Debug.Log($"✅ Account Created - Number: {accountNumber}");

        if (string.IsNullOrEmpty(accountNumber))
        {
            Debug.LogWarning("⚠️ Account number is empty or null!");
            accountDetailOutput.text = "❌ Error: Account number not generated.";
            return;
        }

        SetActivePanel(displayAccountDetailPage);
        accountDetailOutput.text =
            $"✅ Account Created!\n" +
            $"Name: {fname} {lname}\n" +
            $"Account Number: {accountNumber}";

        firstNameInput.text = "";
        lastNameInput.text = "";
        pinInput.text = "";
    }

    public void OnDepositAmountButtonClick()
    {   
        string depositAmount = depositAmountInput.text.Trim();
        float amount;

        if (!float.TryParse(depositAmount, out amount) || amount <= 0)
        {
            depositStatusText.text = "❌ Enter a valid amount.";
            Debug.Log("❌ Invalid deposit amount entered.");
            return;
        }

        AccountManager.Instance.Deposit(amount);
        depositStatusText.text = $"✅ ₦{amount} deposited successfully!";
        Debug.Log($"✅ Deposit of ₦{amount} completed.");
    }

    public void OnWithdrawAmountButtonClick()
    {
        string withdrawAmount = withdrawAmountInput.text.Trim();
        float amount;

        if (!float.TryParse(withdrawAmount, out amount) || amount <= 0)
        {
            withdrawStatusText.text = "❌ Enter a valid amount.";
            Debug.Log("❌ Invalid withdraw amount entered.");
            return;
        }

        bool success = AccountManager.Instance.Withdraw(amount);
        if (success)
        {
            withdrawStatusText.text = $"✅ ₦{amount} withdrawn successfully!";
            Debug.Log($"✅ Withdrawal of ₦{amount} completed.");
        }
        else
        {
            withdrawStatusText.text = "❌ Withdrawal failed: Insufficient funds.";
            Debug.Log("❌ Withdrawal failed.");
        }
    }

    public void UpdateBalanceDisplay()
    {
        if (balanceOutput != null && AccountManager.Instance != null)
        {
            balanceOutput.text = AccountManager.Instance.GetBalanceFormatted();
        }
    }

    public void OnConfirmChangePinButtonClick()
    {
        string newPin = newPinInput.text.Trim();

        if (!IsValidPin(newPin))
        {
            statusText.text = "Invalid PIN. Must be 4 digits.";
            Debug.Log("❌ Invalid PIN: Must be 4 digits.");
            return;
        }

        string accNum = AccountManager.Instance.CurrentLoggedInAccount;
        if (string.IsNullOrEmpty(accNum))
        {
            statusText.text = "No account logged in.";
            Debug.Log("❌ No account logged in for PIN change.");
            return;
        }

        AccountManager.AccountData account = AccountManager.Instance.LoadAccountFromPrefs(accNum);
        if (account == null)
        {
            statusText.text = "Account not found.";
            Debug.Log($"❌ Account {accNum} not found for PIN change.");
            return;
        }

        account.pin = newPin;
        AccountManager.Instance.SaveAccountToPrefs(account);

        statusText.text = "PIN changed successfully!";
        Debug.Log($"✅ PIN changed for account {accNum} to {newPin}");

        newPinInput.text = "";
    }

    private bool IsValidPin(string pin)
    {
        return pin.Length == 4;
    }

    public void OnFiveKButtonClick()
    {
        float cash = 5000;
        bool success = AccountManager.Instance.Withdraw(cash);
        if (success)
        {
            withdrawStatusText.text = $"✅ ₦{cash} withdrawn successfully!";
            Debug.Log($"✅ Withdrawal of ₦{cash} completed.");
        }
        else
        {
            withdrawStatusText.text = "❌ Withdrawal failed: Insufficient funds.";
            Debug.Log("❌ Withdrawal failed.");
        }
    }

    public void OnTenKButtonClick()
    {
        float cash = 10000;
        bool success = AccountManager.Instance.Withdraw(cash);
        if (success)
        {
            withdrawStatusText.text = $"✅ ₦{cash} withdrawn successfully!";
            Debug.Log($"✅ Withdrawal of ₦{cash} completed.");
        }
        else
        {
            withdrawStatusText.text = "❌ Withdrawal failed: Insufficient funds.";
            Debug.Log("❌ Withdrawal failed.");
        }
    }

    public void OnTwentyKButtonClick()
    {
        float cash = 20000;
        bool success = AccountManager.Instance.Withdraw(cash);
        if (success)
        {
            withdrawStatusText.text = $"✅ ₦{cash} withdrawn successfully!";
            Debug.Log($"✅ Withdrawal of ₦{cash} completed.");
        }
        else
        {
            withdrawStatusText.text = "❌ Withdrawal failed: Insufficient funds.";
            Debug.Log("❌ Withdrawal failed.");
        }
    }

    public void OnFiftyKButtonClick()
    {
        float cash = 50000;
        bool success = AccountManager.Instance.Withdraw(cash);
        if (success)
        {
            withdrawStatusText.text = $"✅ ₦{cash} withdrawn successfully!";
            Debug.Log($"✅ Withdrawal of ₦{cash} completed.");
        }
        else
        {
            withdrawStatusText.text = "❌ Withdrawal failed: Insufficient funds.";
            Debug.Log("❌ Withdrawal failed.");
        }
    }

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
        SetActivePanel(changePinPage);
    }

    public void OnCheckBalanceButtonClick()
    {
        SetActivePanel(balancePage);
        UpdateBalanceDisplay();
    }

    public void OnBackButtonClick()
    {
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

    public void OnConfirmTransferButtonClick()
    {
        string recipientAcc = transferAccNumInput.text.Trim();
        string amountText = transferAmountInput.text.Trim();
        float amount;

        if (string.IsNullOrEmpty(recipientAcc))
        {
            transferStatusText.text = "❌ Enter a valid recipient account number.";
            Debug.Log("❌ Invalid recipient account number.");
            return;
        }

        if (!float.TryParse(amountText, out amount) || amount <= 0)
        {
            transferStatusText.text = "❌ Enter a valid amount.";
            Debug.Log("❌ Invalid amount entered for transfer.");
            return;
        }

        string senderAcc = AccountManager.Instance.CurrentLoggedInAccount; // Use logged-in account

        bool success = AccountManager.Instance.Transfer(senderAcc, recipientAcc, amount);

        if (success)
        {
            transferStatusText.text = $"✅ ₦{amount} transferred successfully to {recipientAcc}!";
            Debug.Log($"✅ Transfer of ₦{amount} to {recipientAcc} completed.");
        }
        else
        {
            transferStatusText.text = "❌ Transfer failed: Check balance or account.";
            Debug.Log("❌ Transfer failed.");
        }
    }

    public void ClearAllInputFields(GameObject targetPanel = null)
    {
        if (targetPanel == homeMenu)
        {
            if (enterAccNumInput != null) enterAccNumInput.text = "";
            if (enterPinInput != null) enterPinInput.text = "";
            if (loginStatusText != null) loginStatusText.text = "";
        }

        if (depositAmountInput != null) depositAmountInput.text = "";
        if (withdrawAmountInput != null) withdrawAmountInput.text = "";
        if (transferAmountInput != null) transferAmountInput.text = "";
        if (transferAccNumInput != null) transferAccNumInput.text = "";
        if (transferStatusText != null) transferStatusText.text = "";
        if (firstNameInput != null) firstNameInput.text = "";
        if (lastNameInput != null) lastNameInput.text = "";
        if (pinInput != null) pinInput.text = "";
        if (accountDetailOutput != null) accountDetailOutput.text = "";
        if (newPinInput != null) newPinInput.text = "";
        if (statusText != null) statusText.text = "";
        if (withdrawStatusText != null) withdrawStatusText.text = "";
        if (depositStatusText != null) depositStatusText.text = "";

        Debug.Log("🧽 Input fields cleared (selectively).");
    }
}