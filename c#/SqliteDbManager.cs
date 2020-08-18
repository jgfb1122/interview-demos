using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    public class SqliteDbManager : IDbManager
    {
        public DbConnection getConnection()
        {
            try
            {
                var connection = new SQLiteConnection("Data Source=citystatecountry.db;Version=3;FailIfMissing=True");

                return connection.OpenAndReturn();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Dictionary<string, int> getDataFromDB(DbConnection connection)
        {
            try
            {
                //This should probably not be hard coded, this could be dynamic or automated or something. Or this could be fed from a source where we keep queries
                string queryString = "SELECT CountryName, SUM(Population) FROM Country LEFT JOIN State ON Country.CountryId = State.CountryId LEFT JOIN City ON State.StateId = City.StateId GROUP BY CountryName";

                //create the command
                DbCommand command = connection.CreateCommand();
                command.CommandText = queryString;

                Dictionary<string, int> populationByCountryData = new Dictionary<string, int>();


                // Retrieve the data. Loop the DB data and fill the dictionary
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("Country Name:{0} and its population of, {1}", reader[0], reader[1]);

                    //This is a check just to double check that if the data has already been added and if it has just increment the value by the new value found
                    //This could potential be removed depending on our understand of what the data is suppose to be. Duplicate rows maybe shouldn't be expected and handled a different way.
                    if (populationByCountryData.ContainsKey(reader[0].ToString()))
                    {
                        populationByCountryData[reader[0].ToString()] += Convert.ToInt32(reader[1]);
                    }
                    else
                    {
                        populationByCountryData.Add(reader[0].ToString(), Convert.ToInt32(reader[1]));
                    }

                }
                connection.Close();

                return populationByCountryData;
            }
            catch (Exception ex)
            {
                connection.Close();
                Console.WriteLine("Exception.Message: {0}", ex.Message);
                return null;
            }
        }

        public Task<Dictionary<string, int>> getDataFromDBAsync(DbConnection connection)
        {
            return Task.FromResult<Dictionary<string, int>>(getDataFromDB(connection));
        }
    }
}
