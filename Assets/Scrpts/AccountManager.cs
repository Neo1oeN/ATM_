using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AccountManager : MonoBehaviour
{   
    public static AccountManager Instance { get; private set; }
    public string accountNumber {get; private set;}
    public string AccountPin { get; private set; }  // default pin
    public string CurrentLoggedInAccount { get; set; }


    public float Balance;
    public TMP_Text balanceOutput;


    private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // accountNumber = GenerateAccountNumber();
                // Debug.Log("Generated Account Number: " + accountNumber);
                Balance = 0f;

            }
            else
            {
                Destroy(gameObject);
            }
        }



    public string GenerateAccountNumber()
    {
        System.Random random = new System.Random();
        //first digit (1-9), account numbers dont usually start with 0
        string firstDigit = "210";

        //Remaining 9 digits(0-9)
        string remainDigits = "";
        for (int i = 0; i < 7; i++)
        {
            remainDigits += random.Next(0,10).ToString();
        }


        string fullAccountNum = firstDigit + remainDigits;
        // Debug.Log(fullAccountNum);
        return fullAccountNum;


    }
    public void SetPin(string pin)
    {
        AccountPin = pin;
    }


    public void Deposit(float amount)
{
    if (amount <= 0f)
    {
        Debug.Log("❌ Invalid deposit amount.");
        return;
    }

    var accNum = CurrentLoggedInAccount;
    var account = LoadAccountFromPrefs(accNum);

    if (account != null)
    {
        account.balance += amount;
        SaveAccountToPrefs(account);
        Debug.Log($"💰 Deposited ₦{amount} to {accNum}. New Balance: ₦{account.balance}");
    }
    else
    {
        Debug.Log("❌ Failed to deposit: Account not found.");
    }
}

    

    public bool Withdraw(float amount)
{
    if (amount <= 0f)
    {
        Debug.Log("❌ Invalid withdrawal amount.");
        return false;
    }

    var accNum = CurrentLoggedInAccount;
    var account = LoadAccountFromPrefs(accNum);

    if (account != null && account.balance >= amount)
    {
        account.balance -= amount;
        SaveAccountToPrefs(account);
        Debug.Log($"🏧 Withdrew ₦{amount} from {accNum}. New Balance: ₦{account.balance}");
        return true;
    }
    else
    {
        Debug.Log("❌ Withdraw failed: insufficient funds or account missing.");
        return false;
    }
}

    

    public float GetBalance()
{
    var accNum = CurrentLoggedInAccount;
    var account = LoadAccountFromPrefs(accNum);
    return account != null ? account.balance : 0f;
}

public string GetBalanceFormatted()
{
    return "₦" + GetBalance().ToString("N2");
}


    // private void OnEnable()
    // {
    //     UpdateBalanceDisplay();
    // }

    // public void UpdateBalanceDisplay()
    // {
    //     if
    //     (
    //         balanceOutput != null && AccountManager.Instance != null
    //     )
    //     {
    //         balanceOutput.text = AccountManager.Instance.GetBalance()
    //     }
    // }
    


    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         GenerateAccountNumber();
    //     }
    //     if (Input.GetKeyDown(KeyCode.B))
    //     {
    //         Deposit(100);
    //     }
    //     if (Input.GetKeyDown(KeyCode.W))
    //     {
    //         Withdraw(50);
    //     }
    // }
    
   [System.Serializable]
public class AccountData
{
    public string accountNumber;
    public string firstName;
    public string lastName;
    public string pin;
    public float balance;

    public AccountData(string acc, string fn, string ln, string p, float b)
    {
        accountNumber = acc;
        firstName = fn;
        lastName = ln;
        pin = p;
        balance = b;
    }
}


    // To save newly created account usinf PlayerPrefs
public void SaveAccountdata(string fname, string lname, string pin, float balance)
{
    string accNumber = GenerateAccountNumber();
    accountNumber = accNumber; // Store the generated account number

    AccountData newAccount = new AccountData(accNumber, fname, lname, pin, balance);
    SaveAccountToPrefs(newAccount);

    // Optional: store the most recent account for quick access
    PlayerPrefs.SetString("last_account", accNumber);
    PlayerPrefs.Save();

    Debug.Log($"New account number created: {accNumber}");
}

    public void SaveAccount(string accNumber, string pin)
    {
        
        //name, account number, pin, .....
        PlayerPrefs.SetString("accountNumber", accNumber);
        PlayerPrefs.SetString("pin", pin);
        PlayerPrefs.Save();
   
        Debug.Log("Account and Pin saved to PlayerPrefs");
    }

    public AccountData GetAccountbyNumber(string accNumber)
    {
        string data = PlayerPrefs.GetString("account_" + accNumber, "");

        if(string.IsNullOrEmpty(data)) return null;


        string[] split = data.Split(','); 

        return new AccountData(split[0], split[1], split[2], split[3], float.Parse(split[4]));
    }

    public void SaveAccountToPrefs(AccountData account)
{
    string key = "account_" + account.accountNumber;
    string json = JsonUtility.ToJson(account);
    PlayerPrefs.SetString(key, json);
    PlayerPrefs.Save();

    Debug.Log("📦 Account saved to PlayerPrefs:");
    Debug.Log($"Key: {key}");
    Debug.Log($"Data: {json}");
}


   public AccountData LoadAccountFromPrefs(string accountNumber)
{
    string key = "account_" + accountNumber;
    string json = PlayerPrefs.GetString(key, "");

    if (string.IsNullOrEmpty(json))
    {
        Debug.Log($"❌ No account found with key: {key}");
        return null;
    }

    Debug.Log($"📥 Loaded account JSON for {accountNumber}: {json}");
    return JsonUtility.FromJson<AccountData>(json);
}

public bool Transfer(string senderAccNum, string recipientAccNum, float amount)
{
    if (amount <= 0f)
    {
        Debug.Log("❌ Invalid transfer amount.");
        return false;
    }

    AccountData sender = LoadAccountFromPrefs(senderAccNum);
    AccountData recipient = LoadAccountFromPrefs(recipientAccNum);

    if (sender == null)
    {
        Debug.Log("❌ Sender account not found.");
        return false;
    }

    if (recipient == null)
    {
        Debug.Log("❌ Recipient account not found.");
        return false;
    }

    if (sender.balance < amount)
    {
        Debug.Log("❌ Not enough balance to transfer.");
        return false;
    }

    // Perform transfer
    sender.balance -= amount;
    recipient.balance += amount;

    // Save both accounts
    SaveAccountToPrefs(sender);
    SaveAccountToPrefs(recipient);

    Debug.Log($"✅ ₦{amount} transferred from {senderAccNum} to {recipientAccNum}");
    return true;
}









}

    

 