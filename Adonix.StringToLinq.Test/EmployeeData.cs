namespace Adonix.StringToLinq;

public class Employer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class Employee
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Department { get; set; }
    public double Salary { get; set; }
    public string Residence { get; set; }
    public DateTime Birthday { get; set; }
    public Employer Employer { get; set; }
}

public class EmployeeData
{
    public IQueryable<Employee> GetEmployees()
    {
        var employer = new Employer
        {
            Id = Guid.NewGuid(),
            Name = "Test Company"
        };

        var employees = new[]
        {
            new Employee
            {
                Id = Guid.NewGuid(),
                Number = 1,
                FirstName = "John",
                LastName = "Doe",
                Department = "IT",
                Salary = 50000,
                Birthday = new DateTime(1965, 01, 21),
                Residence = "Australia",
                Employer = employer
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Number = 2,
                FirstName = "Harry",
                LastName = "Smith",
                Department = "Infrastructure",
                Salary = 60000,
                Birthday = new DateTime(1980, 01, 21),
                Residence = "Australia",
                Employer = employer
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Number = 3,
                FirstName = "Amy",
                LastName = "Richards",
                Department = "CEO",
                Salary = 1000000,
                Birthday = new DateTime(2000, 01, 21),
                Residence = "Australia",
                Employer = employer
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Number = 4,
                FirstName = "Alex",
                LastName = "Mitrakis",
                Department = "Infrastructure",
                Salary = 75000,
                Birthday = new DateTime(1999, 08, 28),
                Residence = "Australia",
                Employer = employer
            }
        }.AsQueryable();

        return employees;
    }
}