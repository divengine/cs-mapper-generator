namespace Divengine.CSMapperGenerator.Example.Model
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Department { get; set; }
        public int Age { get; set; }
        public Address? Address { get; set; }
    }
}
