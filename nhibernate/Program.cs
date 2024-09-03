using NHibernate;
using nhibernateOrm;

class Program
{
    static void Main(string[] args)
    {
        using (ISession session = NHibernateHelper.OpenSession())
        {
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    // Query existing users
                    var existingUsers = session.Query<User>().ToList();
                    Console.WriteLine($"Existing users before operations: {existingUsers.Count}");

                    // Only add new users if the table is empty
                    if (existingUsers.Count == 0)
                    {
                        var users = new List<User>
                        {
                            new User { Name = "John Doe", Email = "john@example.com" },
                            new User { Name = "Jane Smith", Email = "jane@example.com" },
                            new User { Name = "Bob Johnson", Email = "bob@example.com" },
                            new User { Name = "Alice Brown", Email = "alice@example.com" },
                            new User { Name = "Charlie Davis", Email = "charlie@example.com" }
                        };

                        foreach (var user in users)
                        {
                            session.Save(user);
                            Console.WriteLine($"Created user with ID: {user.Id}");
                        }
                    }

                    // Query and print all users
                    var allUsers = session.Query<User>().OrderBy(u => u.Id).ToList();
                    Console.WriteLine($"\nTotal users after operations: {allUsers.Count}");
                    Console.WriteLine("User List:");
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("| ID |        Name        |         Email         |");
                    Console.WriteLine("---------------------------------------------------");
                    foreach (var user in allUsers)
                    {
                        Console.WriteLine($"| {user.Id,-3}| {user.Name,-19}| {user.Email,-22}|");
                    }
                    Console.WriteLine("---------------------------------------------------");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    transaction.Rollback();
                }
            }
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
