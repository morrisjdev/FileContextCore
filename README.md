# FileContextCore

FileContextCore is a "Database"-Provider for Entity Framework Core and adds the ability to store information in files instead of being limited to databases. It enables fast developments because of the advantage of just copy, edit and delete files.

This framework bases on the idea of FileContext by DevMentor ([https://github.com/pmizel/DevMentor.Context.FileContext](https://github.com/pmizel/DevMentor.Context.FileContext))

## Advantages

- No database needed
- easy configuration
- rapid data-modelling, -modification
- share data through version-control
- supports all serializable .NET types
- integrated seamlessly into EF Core
- diferrent serializer supported (XML, JSON, CSV, Excel)
- supports encryption
- supports relations

!This extension is not intended to be used in production systems!

## Install

[https://www.nuget.org/packages/FileContextCore/](https://www.nuget.org/packages/FileContextCore/)

```
PM > Install-Package FileContextCore
```

Configure EF Core

```cs
optionsBuilder.UseFileContext();
```

or

```cs
services.AddEntityFramework().AddDbContext<Context>(options => options.UseFileContext());
```

## Example

For an Example check out: [Example](https://github.com/morrisjdev/FileContextCore/tree/master/src/Example)

## Configuration

By default the extension uses `JSON`-serialization and the `DefaultFileManager`

## Available Serializer

### XMLSerializer

Serializes data using System.XML

```cs
optionsBuilder.UseFileContext("xml");
```

### CSVSerializer

Serializes data using CsvHelper ([https://joshclose.github.io/CsvHelper/](https://joshclose.github.io/CsvHelper/))

```cs
optionsBuilder.UseFileContext("csv");
```

### JSONSerializer

Serializes data using Newtonsoft Json.NET ([http://www.newtonsoft.com/json](http://www.newtonsoft.com/json))

```cs
optionsBuilder.UseFileContext("json");
```

### BSONSerializer

Serializes data to bson using Newtonsoft Json.NET ([http://www.newtonsoft.com/json](http://www.newtonsoft.com/json))

```cs
optionsBuilder.UseFileContext("bson");
```

### EXCELSerializer

Saves files into an .xlsx-file and enables the quick editing of the data using Excel

Uses [EEPlus](http://epplus.codeplex.com/documentation) implementation for .Net Core ([https://github.com/VahidN/EPPlus.Core](https://github.com/VahidN/EPPlus.Core))

```cs
optionsBuilder.UseFileContext("excel");

//use password
optionsBuilder.UseFileContext("excel:password");
```

To run on Linux-Systems

```
sudo apt-get update
sudo apt-get install libgdiplus
```

## File Manager

### DefaultFileManager

Saves the data into files

```cs
optionsBuilder.UseFileContext("json", "default");
```

### EncryptedFileManager

Encrypts the data and saves them into files

```cs
optionsBuilder.UseFileContext("json", "encrypted:password");
```

## Combined Manager

## Custom implementation

For customization you can implement the Interfaces `ISerializer` and `IFileManager`

## Author

[Morris Janatzek](http://morrisj.net) ([morrisjdev](https://github.com/morrisjdev))
