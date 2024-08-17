using Divengine.CSharpMapper.Example.Mappers;
using Divengine.CSharpMapper.Example.Model;

namespace Divengine.CSharpMapper.Example.Tests
{
    public class CustomMapperSuite
    {
        [Test]
        public void MapEmployeeToEmployeeDto()
        {
            // Arrange
            Employee employee = new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Department = "IT",
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "Springfield",
                    State = "IL",
                    ZipCode = "62701"
                }
            };

            // Act
            EmployeeDto employeeDto = CustomEmployeeMapper.Map(employee);

            // Assert
            Assert.That(employeeDto.Id, Is.EqualTo(1));
            Assert.That(employeeDto.FullName ?? "", Is.EqualTo("John Doe"));
            Assert.That(employeeDto.Department ?? "", Is.EqualTo("IT"));
            Assert.That(employeeDto.Age, Is.EqualTo(34));
            Assert.That(employeeDto.Address?.Street ?? "", Is.EqualTo("123 Main St"));
            Assert.That(employeeDto.Address?.City ?? "", Is.EqualTo("Springfield"));
            Assert.That(employeeDto.Address?.State ?? "", Is.EqualTo("IL"));
            Assert.That(employeeDto.Address?.ZipCode ?? "", Is.EqualTo("62701"));

            employee.Address = null;

            Assert.That(employeeDto.Address, Is.Not.Null);
        }
    }
}
