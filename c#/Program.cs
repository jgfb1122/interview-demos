using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started");
            Console.WriteLine("Getting DB Connection...");

            IDbManager db = new SqliteDbManager();
            DbConnection conn = db.getConnection();

            //I made the assumption that if we can't get a connection to the data base we don't want to return just API data for confusion reasons.
            if(conn == null)//This check could be done within the class itself. Just depends on architecture decisions
            {
                Console.WriteLine("Failed to get connection. Data can not be aggregated.");
            }
            else
            {
                //Getting the data as a dictionary from the DB
                //This data will already come joined from the three tables of City, State, and Country
                Task<Dictionary<string, int>> dataBaseTask = db.getDataFromDBAsync(conn);

                //Get the data from the API
                IStatService sudoAPI = new ConcreteStatService();
                Task<List<Tuple<string, int>>> apiTask = sudoAPI.GetCountryPopulationsAsync();


                //wait for both the API and Database to finish and get the results
                dataBaseTask.Wait();
                apiTask.Wait();

                Dictionary<string, int> resultsFromDataBase = dataBaseTask.Result;
                List<Tuple<string, int>> resultsFromAPI = apiTask.Result;


                //Loop the Tuple of data from the API and do checks against the dictionary in constant time to aggregate the values
                //if the value doesn't exist add it.
                for (int listIndex = 0; listIndex < resultsFromAPI.Count; listIndex++ )
                {
                    string keyOfCountry = resultsFromAPI[listIndex].Item1;//variable added for readablity 
                    if (!resultsFromDataBase.ContainsKey(keyOfCountry))
                    {
                        int valueFromCountry = resultsFromAPI[listIndex].Item2;//variable added for readablity 
                        resultsFromDataBase.Add(keyOfCountry, valueFromCountry);
                    }
                }


                //output data to file
                Console.WriteLine("Writing data to output file");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Users\\John\\Documents\\QuickBase\\interview-demos\\c#\\AggregatedData.txt"))
                    foreach (var entry in resultsFromDataBase)
                        file.WriteLine("[{0}, {1}]", entry.Key, entry.Value);
                Console.WriteLine("finished writing data to output file");
            }
        }
    }
}
