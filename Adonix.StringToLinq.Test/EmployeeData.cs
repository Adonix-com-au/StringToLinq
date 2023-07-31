namespace Adonix.StringToLinq.Test;

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
                Id = Guid.NewGuid(),
                Name = "John Doe",
                Department = "IT",
                Age = 25,
                Residence = "Australia",
                Employer = new Employer
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Company"
                }
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "John Snow",
                Department = "Infrastructure",
                Age = 25,
                Residence = "Australia",
                Employer = new Employer
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Company"
                }
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Amy Richards",
                Department = "CEO",
                Age = 23,
                Residence = "Australia",
                Employer = new Employer
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Company"
                }
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                Name = "Alex Mitrakis",
                Department = "Infrastructure",
                Age = 24,
                Residence = "Australia",
                Employer = new Employer
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Company"
                }
            }
        }.AsQueryable();
        return Employees;
    }
}