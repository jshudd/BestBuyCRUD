using System;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace BestBuyCRUD
{
    class Program
    {
        static void Main(string[] args)
        {
            var departments = GetAllDepartments();

            foreach (var dept in departments)
            {
                Console.WriteLine(dept);
            }

            var response = "";

            do
            {
                Console.WriteLine("Would you like to add a new Department?");
                Console.WriteLine("\n     Type Y for yes \n     or type EXIT to exit the program");
                response = Console.ReadLine().ToUpper();

                if (response == "Y")
                {
                    Console.WriteLine("What is the name of the new department?");

                    var departmentName = Console.ReadLine();
                    CreateDepartment(departmentName);
                }

            } while (response != "EXIT");

            do
            {
                Console.WriteLine("");
                Console.WriteLine("Would you like to modify a Department name?");
                Console.WriteLine("\n     Type Y for yes \n     or type EXIT to exit the program");
                response = Console.ReadLine().ToUpper();

                if (response == "Y")
                {
                    Console.WriteLine("What is the current name of the department?");

                    var oldDeptName = Console.ReadLine();

                    Console.WriteLine("");
                    Console.WriteLine($"Accessing {oldDeptName}. What would you like to change the name to?");

                    var newDeptName = Console.ReadLine();

                    UpdateDepartment(oldDeptName, newDeptName);

                    Console.WriteLine("");

                    var updateDept = GetAllDepartments();

                    Console.WriteLine($"Here is the updated list of Departments:");
                    foreach (var dept in departments)
                    {
                        Console.WriteLine(dept);
                    }

                }

            } while (response != "EXIT");

        }

        static IEnumerable GetAllDepartments()
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = System.IO.File.ReadAllText("ConnectionString.txt");

            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Name FROM Departments;";

            using (conn)
            {
                conn.Open();
                List<string> allDepartments = new List<string>();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read() == true)
                {
                    var currentDepartment = reader.GetString("Name");
                    allDepartments.Add(currentDepartment);
                }

                return allDepartments;
            }
        }

        static void CreateDepartment(string departmentName)
        {
            var connStr = System.IO.File.ReadAllText("ConnectionString.txt");

            //If you adopt initializing the connection inside the using statement then you can't make a mistake
            //later when reorganizing or refactoring code and accidentally doing something that implicitly
            //opens a connection that isn't closed

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                // parameterized query to prevent SQL Injection
                cmd.CommandText = "INSERT INTO departments (Name) VALUES (@departmentName);";
                cmd.Parameters.AddWithValue("departmentName", departmentName);
                cmd.ExecuteNonQuery();
            }
        }
        
        static void UpdateDepartment(string oldDeptName, string newDeptName)
        {
            var connStr = System.IO.File.ReadAllText("ConnectionString.txt");

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE departments SET Name = @newDeptName WHERE Name = @oldDeptName;";
                cmd.Parameters.AddWithValue("oldDeptName", oldDeptName);
                cmd.Parameters.AddWithValue("newDeptName", newDeptName);

                cmd.ExecuteNonQuery();
            }
        }

    }
}
