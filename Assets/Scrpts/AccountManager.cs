using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AccountManager : MonoBehaviour
{   
    public static AccountManager Instance { get; private set; }
    public string accountNumber {get; private set;}
    public float Balance;


    private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // accountNumber = GenerateAccountNumber();
                // Balance = 0f;

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
        Debug.Log(fullAccountNum);
        return fullAccountNum;


    }


    public void Deposit(float amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            Debug.Log($"Deposited: {amount}. New Balance: {Balance}");
        }
    }
    

    public bool Withdraw(float amount)
    {
        if (amount > 0 && Balance >= amount)
        {
            Balance -= amount;
            Debug.Log($"Withdrawn: {amount}. New Balance: {Balance}");
            return true;
        }
        return false;
    }
    

    public float GetBalance()
    {
        return Balance;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateAccountNumber();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Deposit(100);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Withdraw(50);
        }
    }




}

    

 