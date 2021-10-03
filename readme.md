# SQL database access in the C# using Dapper

[Dapper] is module for C# that makes it easy to use SQL with C#.

It is a thin layer of C# extension methods for the `IDbConnection` interface.

Some of the methods it adds are:

* [Query](https://github.com/DapperLib/Dapper#execute-a-query-and-map-the-results-to-a-strongly-typed-list) - return a list of objects from a SQL query.
* [Execute](https://github.com/DapperLib/Dapper#execute-a-command-that-returns-no-results) - run a SQL query that returns nothing
* [Query](https://github.com/DapperLib/Dapper#execute-a-command-multiple-times) - run a command multiple times

To use it add the `using Dapper;` statement to your class file.

You will also need to add the to your `.csproj` file.

```xml
<ItemGroup>
    <PackageReference Include="Dapper"
Version="1.50.2" />
  </ItemGroup>
```

Dapper works with any SQL database supported by C# that implement the `IDbConnection` interface.

To use SQLite add this to your `.csproj` file's `<ItemGroup>` tag:

```xml
<PackageReference Include="Microsoft.Data.Sqlite"
Version="2.0.0" />
```

> You can use Dapper with PostgreSQL by using the [Npgsql module](https://www.npgsql.org/doc/index.html)

## Using Dapper

Create a new dotnet project using this command:

```
dotnet new console -o TryingDapper
```

The `-o` means that a new folder called `TryingDapper` will be created. 

Change into the folder using `./TryingDapper`.

Reference the `Dapper` and `Microsoft.Data.Sqlite` module in your project by adding this to your `TryingDapper.csproj` file

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.Data.Sqlite" Version="2.0.0" />
    <PackageReference Include="Dapper" Version="1.50.2" />
  </ItemGroup>
```

Create a class:

We will create a `Pizza` class in a `Pizza.cs` file.

```c#
namespace TryingDapper
{
    public class Pizza
    {
        string Type {
            get;
            set;
        }

        string Size {
            get;
            set
        }

        double Price {
            get;
            set
        }
    }
}
```

Setup Dapper and add these 2 import statements to your  `Program.cs` class file:

```c#
using Microsoft.Data.Sqlite;
using Dapper;
```

## Creating a table

Create a table using this ddl command:

```sql
create table if not exists pizza (
	id integer primary key AUTOINCREMENT,
	type text,
	size text,
	price real
);
```

Create IDBConnection instance using this command:

```c#
using (var connection = new SqliteConnection("Data Source=./trying_dapper.db;")) {
    connection.Open();
}
```

If this command is successfull a `trying_dapper.db` file will be created in the root of your project directory.


Run the create table script using an `Execute` statement:

```c#

string CREATE_PIZZA_TABLE = @"create table if not exists pizza (
	id integer primary key AUTOINCREMENT,
	type text,
	size text,
	price real
);" ;

connection.Execute(CREATE_PIZZA_TABLE);
```

## Creating data in the table

Insert data in the `pizza` table:

```c#
connection.Execute(@"
	insert into 
		pizza (type, size, price)
	values 
		(@Type, @Size, @Price);",
		new Pizza() {
		Type = "Regina",
		Size = "small",
		Price = 31.75
	});
```

Add a list of Pizza's like this:

```c#

connection.Execute(@"
	insert into 
		pizza (type, size, price)
	values 
		(@Type, @Size, @Price);",
	new Object[] {
		new Pizza() {
		Type = "Regina",
		Size = "small",
		Price = 31.75
	}, new Pizza {
		Type = "Regina",
		Size = "medium",
		Price = 51.75
	}, new Pizza {
		Type = "Regina",
		Size = "large",
		Price = 89.75
	}
});
```

## Querying the table

Read the data in the table using this code:

```c#
var pizzas = connection.Query<Pizza>(@"select * from pizza");
Console.WriteLine(pizzas.Count());
```

**Note:** add `using System.Linq;` to make `pizzas.Count()` work.

### To group data

To group data using a `group by` SQL query you can use an approach like this.

Create a class like this:

```c#
namespace TryingDapper
{
    public class PizzaGrouped
    {
        public string Grouping {
            get;
            set;
        }

        public double Total {
            get;
            set;
        }

        
    }
}
```

And run a Query like this with `Dapper`:

```c#
var GROUP_BY_SIZE = @"select size as grouping, sum(price) as total from pizza group by size";

var pizzaTotalPerSize = connection.Query<PizzaGrouped>(GROUP_BY_SIZE);

foreach (var pizza in pizzaTotalPerSize)
{
	Console.WriteLine($"size : {pizza.Grouping} - total @ {pizza.Total} ");
}
```


