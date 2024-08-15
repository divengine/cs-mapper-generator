using Divengine.CSMapperGenerator.Example.Mappers;
using Divengine.CSMapperGenerator.Example.Model;

var employee = new Employee
{
    Id = 1,
    FirstName = "John",
    LastName = "Doe",
    DateOfBirth = new DateTime(1990, 1, 1),
    Department = "IT",
    Address = new Address
    {
        Street = "123 Elm St.",
        City = "Springfield",
        State = "IL",
        ZipCode = "62701"
    }
};

var dto = EmployeeMapper.Map(employee);

Console.WriteLine(dto.Id);
Console.WriteLine(dto.Department);

var clonedDto = EmployeeMapper.Clone(dto);
Console.WriteLine(clonedDto.Id);
Console.WriteLine(clonedDto.Department);

var dto2 = CustomEmployeeMapper.Map(employee);

Console.WriteLine(dto.Address?.Street);
dto.Address.Street = "123 Main St.";

Console.WriteLine(dto2.Address?.Street);