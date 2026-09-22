using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SlojPodataka
{
    public class DBUtils
    {
        protected string ConnectionString;

        public DBUtils()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["KonekcijaBaze"]?.ConnectionString
                               ?? ConfigurationManager.ConnectionStrings["TakmicenjeDBEntities2"]?.ConnectionString;
        }

        // Metoda za kreiranje i pripremu komande (dodela upita / procedure)
        protected SqlCommand KreirajKomandu( string upitIliProcedura, CommandType tipKomande, SqlConnection conn )
        {
            SqlCommand cmd = new SqlCommand(upitIliProcedura, conn);
            cmd.CommandType = tipKomande;
            return cmd;
        }
    }
}