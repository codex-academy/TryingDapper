using System;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Linq;
using System.Data;



namespace TryingDapper
{
    class CreatePizza {

        IDbConnection connection;

        public CreatePizza(IDbConnection connection) {
            this.connection = connection;
        }

        public void Execute(Pizza[] pizzas) {
            
        }

    }

    class Program
    {

        static void CreatePizzas() {

        }



        static void Main(string[] args)
        {

            string CREATE_PIZZA_TABLE = @"create table if not exists pizza (
	            id integer primary key AUTOINCREMENT,
	            type text,
	            size text,
	            price real
            );" ;

            using (var connection = new SqliteConnection("Data Source=./trying_dapper.db;")) {
                connection.Open();

                connection.Execute(CREATE_PIZZA_TABLE);
                
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

                var pizzas = connection.Query<Pizza>(@"select * from pizza");

                Console.WriteLine(pizzas.Count());

                var GROUP_BY_SIZE = @"select size as grouping, sum(price) as total from pizza group by size";
                var pizzaTotalPerSize = connection.Query<PizzaGrouped>(GROUP_BY_SIZE);

                foreach (var pizza in pizzaTotalPerSize)
                {
                    Console.WriteLine($"size : {pizza.Grouping} - total @ {pizza.Total} ");
                }

            }

        }
    }
}
