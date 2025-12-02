using Microsoft.Data.SqlClient;

namespace CustomerDBConsole;

class Program
{
    static string connectionString = "Server=PUTER;Database=CustomerDb;Integrated Security=true;TrustServerCertificate=true;";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nCustomer Management");
            Console.WriteLine("----------------------------");
            Console.WriteLine("1) Create");
            Console.WriteLine("2) List");
            Console.WriteLine("3) Update");
            Console.WriteLine("4) Delete");
            Console.WriteLine("5) Exit");
            Console.Write("Choice: ");

            string choice = Console.ReadLine() ?? "";
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    CreateCustomer();
                    break;
                case "2":
                    ListCustomers();
                    break;
                case "3":
                    UpdateCustomer();
                    break;
                case "4":
                    DeleteCustomer();
                    break;
                case "5":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static SqlConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }

    static void CreateCustomer()
    {
        Console.Write("First name: ");
        string firstName = Console.ReadLine() ?? "";

        Console.Write("Last name: ");
        string lastName = Console.ReadLine() ?? "";

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";

        using var connection = GetConnection();
        connection.Open();

        string sql = @"INSERT INTO dbo.Customer (FirstName, LastName, Email)
                      VALUES (@FirstName, @LastName, @Email);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FirstName", firstName);
        command.Parameters.AddWithValue("@LastName", lastName);
        command.Parameters.AddWithValue("@Email", email);

        int newId = (int)command.ExecuteScalar();
        Console.WriteLine($"Customer added with ID = {newId}");
    }

    static void ListCustomers()
    {
        using var connection = GetConnection();
        connection.Open();

        string sql = "SELECT CustomerId, FirstName, LastName, Email FROM Customer;";

        using var command = new SqlCommand(sql, connection);
        using SqlDataReader reader = command.ExecuteReader();

        Console.WriteLine("ID      First     Last      Email");
        Console.WriteLine("----------------------------------------------");

        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            string email = reader.GetString(3);

            Console.WriteLine($"{id,-8}{firstName,-10}{lastName,-10}{email}");
        }
    }

    static void UpdateCustomer()
    {
        Console.Write("Enter ID to update: ");
        int customerId = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("New Email: ");
        string newEmail = Console.ReadLine() ?? "";

        using var connection = GetConnection();
        connection.Open();

        string sql = "UPDATE Customer SET Email = @newEmail WHERE CustomerId = @customerId";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@newEmail", newEmail);
        command.Parameters.AddWithValue("@customerId", customerId);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            Console.WriteLine("Updated.");
        }
        else
        {
            Console.WriteLine("No customer found with that ID.");
        }
    }

    static void DeleteCustomer()
    {
        Console.Write("Enter ID to delete: ");
        int customerId = int.Parse(Console.ReadLine() ?? "0");

        using var connection = GetConnection();
        connection.Open();

        string sql = "DELETE FROM Customer WHERE CustomerId = @customerId";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            Console.WriteLine("Deleted.");
        }
        else
        {
            Console.WriteLine("No customer found with that ID.");
        }
    }
}
