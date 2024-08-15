# Div CS Mapper Generator

## Example project

This project is an example of how to use the Div CS Mapper Generator.

### How to use

1. Compile or download the Div CS Mapper Generator from GitHub.

2. Create a `mapper.json` file in the root of the project with the following content:

```json
{
  "mappers": [
    {
      "namespace": "Namespace.Of.Your.Mapper",
      "mapper": "MapperClassNameToGenerate",
      "outputFolder": "output/folder/of/generated/mapper",
      "classPairs": [
        [ "Relative/Path/To/ModelClassX.cs", "Relative/Path/To/ModelClassX.cs" ] // clone
        [ "Relative/Path/To/ModelClassA.cs", "Relative/Path/To/ModelClassB.cs" ] // map
      ]
    }
  ]
}
```

3. Run the following command in the root of the project:

```bash
$ path/to/div-cs-mapper-generator.exe path/to/mapper.json
```

4. The generated files will be in the defined outputFolder

### Extends and customization

You can inherit the generated mapper class to override the generated methods or add new ones.

```csharp
namespace Namespace.Of.Your.Mapper
{
	public class MapperClassNameToGenerate : MapperClassNameToGenerateBase
	{
		public override ModelClassB Map(ModelClassA model)
		{
			var result = base.Map(model);
			
			// Custom logic here

			return result;
		}
	}
}
```
