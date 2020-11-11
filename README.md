# FileContextCore [![Build Status](https://travis-ci.org/morrisjdev/FileContextCore.svg?branch=master)](https://travis-ci.org/morrisjdev/FileContextCore) [![Maintainability](https://api.codeclimate.com/v1/badges/72cbed89392efad4c743/maintainability)](https://codeclimate.com/github/morrisjdev/FileContextCore/maintainability)

FileContextCore is a "Database"-Provider for Entity Framework Core and adds the ability to store information in files.
It enables fast developments because of the advantage of just copy, edit and delete files.

This framework bases on the idea of FileContext by DevMentor ([https://github.com/pmizel/DevMentor.Context.FileContext](https://github.com/pmizel/DevMentor.Context.FileContext))

## Advantages

- No database needed
- Easy configuration
- Rapid data-modelling, -modification
- Share data through version-control
- Supports all serializable .NET types
- Integrates seamlessly into EF Core
- Different serializer supported (XML, JSON, CSV, Excel)
- Supports encryption
- Supports relations
- Supports multiple databases

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

You can also play around with this example on dotnetfiddle.net: [Demo](https://dotnetfiddle.net/BtoW5a)

## Configuration

By default the extension uses `JSON`-serialization and the `DefaultFileManager`

You can use a different serializer to support other serialization methods.

## Available Serializer

### XMLSerializer

Serializes data using System.XML

```cs
optionsBuilder.UseFileContextDatabase<XMLSerializer, DefaultFileManager>();
```

### CSVSerializer

Serializes data using CsvHelper ([https://joshclose.github.io/CsvHelper/](https://joshclose.github.io/CsvHelper/))

```cs
optionsBuilder.UseFileContextDatabase<CSVSerializer, DefaultFileManager>();
```

### JSONSerializer

Serializes data using Newtonsoft Json.NET ([http://www.newtonsoft.com/json](http://www.newtonsoft.com/json))

```cs
optionsBuilder.UseFileContextDatabase<JSONSerializer, DefaultFileManager>();
```
or just
```
optionsBuilder.UseFileContextDatabase();
```

### BSONSerializer

Serializes data to bson using Newtonsoft Json.NET ([http://www.newtonsoft.com/json](http://www.newtonsoft.com/json))

```cs
optionsBuilder.UseFileContextDatabase<BSONSerializer, DefaultFileManager>();
```

### EXCELSerializer

Saves files into an .xlsx-file and enables the quick editing of the data using Excel

Uses [EEPlus](http://epplus.codeplex.com/documentation) implementation for .Net Core ([https://github.com/VahidN/EPPlus.Core](https://github.com/VahidN/EPPlus.Core))

```cs
optionsBuilder.UseFileContextDatabase<EXCELStoreManager>();
```

If you want to secure the excel file with a password use:
```cs
optionsBuilder.UseFileContextDatabase<EXCELStoreManager>(password: "<password>");
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
optionsBuilder.UseFileContextDatabase<JSONSerializer, DefaultFileManager>();
```

### EncryptedFileManager

The encrypted file manager encrypts the files with a password.

```cs
optionsBuilder.UseFileContextDatabase<JSONSerializer, EncryptedFileManager>(password: "<password>");
```

## Custom file-location

By default the files are stored in a subfolder of your running application called `appdata`.
If you want to control this behavior you can also use define a custom location.

```cs
optionsBuilder.UseFileContextDatabase(location: @"C:\Users\mjanatzek\Documents\Projects\test");
```

## Multiple Databases

If nothing is configured all files of your application will be stored in a flat folder.
You can optionally define a name for your database and all the corresponding data will saved in a subfolder.
So you are able to use FileContext with multiple DbContext-configurations.

```cs
optionsBuilder.UseFileContextDatabase(databaseName: "database");
```

## Custom provider

You can create custom serializer, file manager and store manager if you want.

If you want to create a custom serializer implement the interface `ISerializer`.

If you want to control storing of data implement interface `IFileManager`.

If you want to create a store manager that does both implement `IStoreManager`.

After adding a custom provider you have to add it as a transient dependency in the dependency injection.

Feel free to create a PR with your new provider and I'll add it to FileContextCore.

## Version compability

| FileContext Version | EF Core Version |
|---------------------|-----------------|
| 3.4.*              | 3.1.0           |
| 3.3.*              | 3.0.0           |
| 3.2.*              | 3.0.0           |
| 3.0.1/3.0.0/2.2.6   | 2.2.6           |
| 2.2.0               | 2.2.0           |

## Custom table/file name

It seems that EF Core currently does not support to define a custom table name using annotations on models.
Use the `OnModelCreating`-method to define a custom table name.

````c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .ToTable("custom_user_table");
}
````

This will store the data in a file called `custom_user_table.json` for example.

## Author

[Morris Janatzek](http://morrisj.net) ([morrisjdev](https://github.com/morrisjdev))
