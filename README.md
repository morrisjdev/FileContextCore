# FileContextCore [![Build Status](https://travis-ci.org/morrisjdev/FileContextCore.svg?branch=master)](https://travis-ci.org/morrisjdev/FileContextCore) [![Maintainability](https://api.codeclimate.com/v1/badges/72cbed89392efad4c743/maintainability)](https://codeclimate.com/github/morrisjdev/FileContextCore/maintainability)

FileContextCore is a "Database"-Provider for Entity Framework Core and adds the ability to store information in files.
It enables fast developments because of the advantage of just copy, edit and delete files.

This framework bases on the idea of FileContext by DevMentor ([https://github.com/pmizel/DevMentor.Context.FileContext](https://github.com/pmizel/DevMentor.Context.FileContext))

## Advantages

- No database needed
- easy configuration
- rapid data-modelling, -modification
- share data through version-control
- supports all serializable .NET types
- integrates seamlessly into EF Core
- different serializer supported (XML, JSON, CSV, Excel)
- supports encryption
- supports relations
- supports multiple databases

!This extension is not intended to be used in production systems!

## Install

[https://www.nuget.org/packages/FileContextCore/](https://www.nuget.org/packages/FileContextCore/)

```
PM > Install-Package FileContextCore
```

### Configure EF Core

#### Configure in DI-Service configuration

In your `Startup.cs` use this:

```cs
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddDbContext<Context>(options => options.UseFileContextDatabase());
    ...
}
```

#### or

#### Override `OnConfiguring` method 

You can also override the `OnConfiguring` method of your DbContext to apply the settings:

```cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseFileContextDatabase();
}
```

## Example

For a simple example check out: [Example](https://github.com/morrisjdev/FileContextCore/tree/master/Example)

You can also play around with this example on dotnetfiddle.net: [Demo](https://dotnetfiddle.net/jvFdaY)

## Configuration

By default the extension uses `JSON`-serialization and the `DefaultFileManager`

You can use a different serializer to support other serialization methods.

## Available Serializer

### XMLSerializer

Serializes data using System.XML

```cs
optionsBuilder.UseFileContextDatabase("xml");
```

### CSVSerializer

Serializes data using CsvHelper ([https://joshclose.github.io/CsvHelper/](https://joshclose.github.io/CsvHelper/))

```cs
optionsBuilder.UseFileContextDatabase("csv");
```

### JSONSerializer

Serializes data using Newtonsoft Json.NET ([http://www.newtonsoft.com/json](http://www.newtonsoft.com/json))

```cs
optionsBuilder.UseFileContextDatabase("json");
```
or just
```
optionsBuilder.UseFileContextDatabase();
```

### BSONSerializer

Serializes data to bson using Newtonsoft Json.NET ([http://www.newtonsoft.com/json](http://www.newtonsoft.com/json))

```cs
optionsBuilder.UseFileContextDatabase("bson");
```

### EXCELSerializer

Saves files into an .xlsx-file and enables the quick editing of the data using Excel

Uses [EEPlus](http://epplus.codeplex.com/documentation) implementation for .Net Core ([https://github.com/VahidN/EPPlus.Core](https://github.com/VahidN/EPPlus.Core))

```cs
optionsBuilder.UseFileContextDatabase("excel");
```

If you want to secure the excel file with a password use:
```cs
optionsBuilder.UseFileContextDatabase("excel:<password>");
```

To run on Linux-Systems
```
sudo apt-get update
sudo apt-get install libgdiplus
```

## File Manager

The file manager controls how the files are stored.

### DefaultFileManager

The default file manager just creates normal files.

```cs
optionsBuilder.UseFileContextDatabase("json", "default");
```

### EncryptedFileManager

The encrypted file manager encrypts the files with a password.

```cs
optionsBuilder.UseFileContextDatabase("json", "encrypted:<password>");
```

## Configuration string

If you want to define the options of file context in a single string you can use the default connection string syntax of databases.

```css
optionsBuilder.UseFileContextDatabaseConnectionString("serializer=xml;databasename=Test;location:test;filemanager:default");
```


## Custom file-location

By default the files are stored in a subfolder of your running application called `appdata`.
If you want to control this behavior you can also use define a custom location.

```cs
optionsBuilder.UseFileContextDatabase(location: @"C:\Users\mjanatzek\Documents\Projects\test");
```

## Multiple Databases

If noting is configured all files of your application will be stored in a flat folder.
You can optionally define a name for your database and all the corresponding data will saved in a subfolder.
So you are able to use FileContext with multiple DbContext-configurations.

```cs
optionsBuilder.UseFileContextDatabase(databasename: "database");
```

## Version compability

| FileContext Version | EF Core Version |
|---------------------|-----------------|
| 3.2.*              | 3.0.0           |
| 3.0.1/3.0.0/2.2.6   | 2.2.6           |
| 2.2.0               | 2.2.0           |

## Author

[Morris Janatzek](http://morrisj.net) ([morrisjdev](https://github.com/morrisjdev))
