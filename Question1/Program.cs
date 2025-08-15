using System;
using System.Collections.Generic;

// a. Transaction record
public record Transaction(
    int Id,
    DateTime Date,
    decimal Amount,
    string Category
);

// b. Interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// c. Concrete processors
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Sent {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Transferred {transaction.Amount:C} for {transaction.Category}");
    }
}

// d. Base Account class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// e. Sealed SavingsAccount class
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }
}

// f. FinanceApp
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        Console.Write("Enter account number: ");
        string accNum = Console.ReadLine();

        Console.Write("Enter initial balance: ");
        decimal initBalance = decimal.Parse(Console.ReadLine());

        var account = new SavingsAccount(accNum, initBalance);

        for (int i = 1; i <= 3; i++)
        {
            Console.WriteLine($"\n--- Enter details for Transaction {i} ---");

            Console.Write("Amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            Console.Write("Category: ");
            string category = Console.ReadLine();

            var transaction = new Transaction(i, DateTime.Now, amount, category);

            // Choose processor based on transaction number
            ITransactionProcessor processor = i switch
            {
                1 => new MobileMoneyProcessor(),
                2 => new BankTransferProcessor(),
                3 => new CryptoWalletProcessor(),
                _ => throw new InvalidOperationException()
            };

            processor.Process(transaction);
            account.ApplyTransaction(transaction);
            _transactions.Add(transaction);
        }
    }
}

// Main entry point
public class Program
{
    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
