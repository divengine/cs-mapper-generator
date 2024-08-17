namespace Divengine.CSharpMapper.Example.Model
{
    public class Employee
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Department { get; set; }
        public Address? Address { get; set; }  
    }
}
