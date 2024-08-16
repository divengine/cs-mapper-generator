# Div CSharp Mapper Generator

The Div CSharp Mapper Generator is a powerful tool designed to automate the creation of mapping classes in C#. It simplifies the process of mapping between models, DTOs, and other classes by generating the necessary code based on a simple JSON configuration file. This tool supports bi-directional mapping by default, with the flexibility to extend and customize the generated mappers through inheritance.

## Features

- *Automatic Code Generation*: Generate C# mapping classes based on a JSON configuration file.
- *Bi-Directional Mapping*: Supports mapping in both directions between classes.
- *Object Property Handling*: Automatically maps object properties, allowing for deep copies rather than reference assignments.
- *Extensibility*: Easily extend and customize the generated mapping methods by inheriting from the generated classes.
- *Custom Logic*: Override default methods to implement custom mapping logic as needed.

## Getting Started

### Installation

You can either compile the Div CSharp Mapper Generator from source or download the precompiled binary from the GitHub releases page.

### Usage

#### 1. Create a JSON Configuration File

Create a mapper.json file in the root of your project. This file defines the mappings you wish to generate.

```json
{
  "mappers": [
    {
      "namespace": "Namespace.Of.Your.Mapper",
      "mapper": "MapperClassNameToGenerate",
      "outputFolder": "output/folder/of/generated/mapper",
      "classPairs": [
        [ "Relative/Path/To/ModelClassX.cs", "Relative/Path/To/ModelClassX.cs" ] // to generate Clone method
        [ "Relative/Path/To/ModelClassA.cs", "Relative/Path/To/ModelClassB.cs" ] // to generate Map method
      ]
    }
  ]
}
```

- `namespace`: The namespace for the generated mapper class.
- `mapper`: The name of the mapper class to generate.
- `outputFolder`: The directory where the generated mapper file will be placed.
- `classPairs`: An array of class pairs to map. Identical class pairs will generate a cloning method.

#### 2. Run the Mapper Generator

Execute the following command from the root of your project:

```bash
path/to/div-cs-mapper-generator.exe path/to/mapper.json
```

This command will generate the mapping classes based on the configuration provided in mapper.json.

#### 3. Locate the Generated Files

The generated files will be found in the output folder specified in the JSON configuration.

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

## Documentation

Visit [https://divengine.org](https://divengine.org)
