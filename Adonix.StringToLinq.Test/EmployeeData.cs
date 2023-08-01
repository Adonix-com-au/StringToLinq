namespace Adonix.StringToLinq;

public class Employer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public int Age { get; set; }
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
                Name = "John Doe",
                Department = "IT",
                Age = 25,
                Birthday = new DateTime(2000, 01, 21),
                Residence = "Australia",
                Employer = employer
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "John Snow",
                Department = "Infrastructure",
                Age = 25,
                Birthday = new DateTime(2000, 01, 21),
                Residence = "Australia",
                Employer = employer
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Amy Richards",
                Department = "CEO",
                Age = 23,
                Birthday = new DateTime(2000, 01, 21),
                Residence = "Australia",
                Employer = employer
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alex Mitrakis",
                Department = "Infrastructure",
                Age = 24,
                Birthday = new DateTime(1999, 08, 28),
                Residence = "Australia",
                Employer = employer
            }
        }.AsQueryable();

        return employees;
    }
}