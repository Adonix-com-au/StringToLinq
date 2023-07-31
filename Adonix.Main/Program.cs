using Adonix.StringToLinq;

namespace Adonix.Main
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var input = "Name eq \"John Doe\" or (Id eq 3 or Id eq 4)";
            var input = "Employer.Id eq 0";

            foreach (var item in new EmployeeData().GetEmployees()
                         .Where(StringExpression.ToExpression<Employee>(input)))
            {
                Console.WriteLine(item.Name);
            }
        }
    }
}

public class Employer
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public int Age { get; set; }
    public string Residence { get; set; }
    public Employer Employer { get; set; }
}

public class EmployeeData
{
    public IQueryable<Employee> Employees;

    public EmployeeData()
    {
        GetEmployees();
    }

    public IQueryable<Employee> GetEmployees()
    {
        Employees = new[]
        {
            new Employee
            {
                Id = 1,
                Name = "John Doe",
                Department = "IT",
                Age = 25,
                Residence = "Malibu",
                Employer = new Employer
                {
                    Id = 0,
                    Name = "Test Company"
                }
            },
            new Employee
            {
                Id = 2,
                Name = "Bob Mkenya",
                Department = "Infrastructure",
                Age = 25,
                Residence = "Kenya",
                Employer = new Employer
                {
                    Id = 0,
                    Name = "Test Company"
                }
            },
            new Employee
            {
                Id = 3,
                Name = "Abc",
                Department = "Infrastructure",
                Age = 25,
                Residence = "Kenya",
                Employer = new Employer
                {
                    Id = 0,
                    Name = "Test Company"
                }
            },
            new Employee
            {
                Id = 4,
                Name = "eew",
                Department = "Infrastructure",
                Age = 25,
                Residence = "Kenya",
                Employer = new Employer
                {
                    Id = 0,
                    Name = "Test Company"
                }
            }
        }.AsQueryable();
        return Employees;
    }
}