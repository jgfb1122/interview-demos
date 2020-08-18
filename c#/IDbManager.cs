using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    public interface IDbManager
    {
        DbConnection getConnection();
        Dictionary<string,int> getDataFromDB(DbConnection connection);
        Task<Dictionary<string, int>> getDataFromDBAsync(DbConnection connection);
    }
}
