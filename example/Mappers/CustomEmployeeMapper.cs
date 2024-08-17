using Divengine.CSharpMapper.Example.Model;

namespace Divengine.CSharpMapper.Example.Mappers
{
    public class CustomEmployeeMapper: EmployeeMapper
    {
        public static new EmployeeDto Map(Employee fromEntity)
        {
            EmployeeDto toEntity = EmployeeMapper.Map(fromEntity);
            
            toEntity.Address = Clone(fromEntity.Address);
            toEntity.FullName = $"{fromEntity.FirstName} {fromEntity.LastName}";
            toEntity.Age = DateTime.Now.Year - fromEntity.DateOfBirth.Year;

            return toEntity;
        }
    }
}
