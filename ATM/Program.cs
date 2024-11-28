using System;
using System.IO;

class ATMSystem
{
    static string usersDirectory = @"C:\Users\marina.kochiashvili\Desktop\HomeworksC#\ATM_Users";

    static void Main(string[] args)
    {
        // Ensure the directory exists
        if (!Directory.Exists(usersDirectory))
        {
            Directory.CreateDirectory(usersDirectory);
        }

        // Main loop for ATM operations
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the ATM System!");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            Console.Write("Please select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterUser();  // Call the registration method
                    break;
                case "2":
                    LoginUser();  // Call the login method
                    break;
                case "3":
                    Console.WriteLine("Thank you for using the ATM. Goodbye!");
                    return;  // Exit the program
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // Login method
    static void LoginUser()
    {
        string username, pincode;

        // Ask for the username once
        Console.Write("Enter your username: ");
        username = Console.ReadLine();

        string userFile = Path.Combine(usersDirectory, username + ".txt");

        // Check if the username exists
        if (File.Exists(userFile))
        {
            // Read user data from the file
            string[] credentials = File.ReadAllLines(userFile);
            string savedPincode = credentials[1].Split('=')[1].Trim();  // Extract saved PIN

            // Ask for the PIN and validate
            while (true)
            {
                Console.Write("Enter your pincode: ");
                pincode = Console.ReadLine();

                // Check if the entered PIN matches the saved PIN
                if (savedPincode == pincode)
                {
                    Console.WriteLine("Login successful!");
                    double balance = Convert.ToDouble(credentials[2].Split('=')[1].Trim()); // Extract balance
                    ShowATMMenu(username, balance);  // Call the ATM menu with the balance
                    break;  // Exit the loop after successful login
                }
                else
                {
                    // PIN is incorrect, ask for the PIN again (inside the PIN loop)
                    Console.WriteLine("Incorrect pincode. Please try again.");
                }
            }
        }
        else
        {
            // Username doesn't exist, show error and allow retry for username
            Console.WriteLine("User not found. Please try again.");
        }
    }

    // Register method
    static void RegisterUser()
    {
        string username, pincode;

        Console.Write("Enter a username: ");
        username = Console.ReadLine();

        string userFile = Path.Combine(usersDirectory, username + ".txt");

        // Check if username already exists
        if (File.Exists(userFile))
        {
            Console.WriteLine("Username already exists. Please choose a different username.");
            return;
        }

        Console.Write("Enter your pincode: ");
        pincode = Console.ReadLine();

        // Create a new user file and save the username and pincode
        using (StreamWriter sw = File.CreateText(userFile))
        {
            sw.WriteLine($"Username={username}");
            sw.WriteLine($"Pincode={pincode}");
            sw.WriteLine($"Balance=0");  // Initialize balance to 0
        }

        Console.WriteLine("Registration successful! You can now log in.");
    }

    // ATM Menu after login
    static void ShowATMMenu(string username, double balance)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Welcome {username} to the ATM System!");
            Console.WriteLine("ATM MENU:");
            Console.WriteLine("1. Withdraw");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Check Balance");
            Console.WriteLine("4. Logout");

            Console.Write("Please select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    balance = Withdraw(balance, username);  // Call the withdraw method and update balance
                    break;
                case "2":
                    balance = Deposit(balance, username);  // Call the deposit method and update balance
                    break;
                case "3":
                    Console.WriteLine($"Your current balance is: ${balance}");
                    break;
                case "4":
                    Console.WriteLine("Logging out...");
                    return;  // Exit the ATM menu and log out
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // Withdraw method
    static double Withdraw(double balance, string username)
    {
        double amount;

        // Make sure to read and parse the withdrawal amount correctly
        Console.Write("Enter the amount to withdraw: $");
        string input = Console.ReadLine();

        // Validate input (is it a valid number?)
        if (!double.TryParse(input, out amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return balance;  // Return the original balance without changes
        }

        if (amount <= 0)
        {
            Console.WriteLine("Amount must be positive.");
        }
        else if (amount > balance)
        {
            Console.WriteLine("Insufficient funds.");
        }
        else
        {
            balance -= amount;
            Console.WriteLine($"You have successfully withdrawn ${amount}. New balance: ${balance}");

            // Update balance in the user's file
            UpdateBalanceInFile(username, balance);
        }

        // Wait for the user to press Enter before going back to the ATM menu
        Console.WriteLine("Press Enter to return to the ATM menu...");
        Console.ReadLine();

        return balance;  // Return the updated balance
    }

    // Deposit method
    static double Deposit(double balance, string username)
    {
        double amount;

        Console.Write("Enter the amount to deposit: $");
        string input = Console.ReadLine();

        // Validate input (is it a valid number?)
        if (!double.TryParse(input, out amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return balance;  // Return the original balance without changes
        }

        if (amount <= 0)
        {
            Console.WriteLine("Amount must be positive.");
        }
        else
        {
            balance += amount;
            Console.WriteLine($"You have successfully deposited ${amount}. New balance: ${balance}");

            // Update balance in the user's file
            UpdateBalanceInFile(username, balance);
        }

        // Wait for the user to press Enter before going back to the ATM menu
        Console.WriteLine("Press Enter to return to the ATM menu...");
        Console.ReadLine();

        return balance;  // Return the updated balance
    }

    // Method to update balance in the file
    static void UpdateBalanceInFile(string username, double balance)
    {
        string userFile = Path.Combine(usersDirectory, username + ".txt");

        // Read all lines from the file
        string[] lines = File.ReadAllLines(userFile);

        // Find the line with balance and update it
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("Balance="))
            {
                lines[i] = $"Balance={balance}";
                break;
            }
        }

        // Write the updated content back to the file
        File.WriteAllLines(userFile, lines);
    }
}

