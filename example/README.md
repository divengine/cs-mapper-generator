# Div CS Mapper Generator Example project

This project is an example of how to use the Div CS Mapper Generator.

From the definition file `mapper.json`, the tool `csmapper` will generate one class by each mapper definition, in this case, `EmployeeMapper`, for the `Address`, `Employee` and `EmployeeDto` models.

```json
{
  "language": "csharp",
  "mappers": [
    {
      "mapper": "EmployeeMapper",
      "outputFolder": "Mappers",
      "namespace": "Divengine.CSMapperGenerator.Example.Mappers",
      "classPairs": [
        [ "Model/Address.cs", "Model/Address.cs" ],
        [ "Model/EmployeeDto.cs", "Model/EmployeeDto.cs" ],
        [ "Model/Employee.cs", "Model/Employee.cs" ],
        [ "Model/Employee.cs", "Model/EmployeeDto.cs" ]
      ]
    }
  ]
}
```

From that generated mapper, the class `CustomEmployeeMapper` is created to show how to extend the generated mapper.

