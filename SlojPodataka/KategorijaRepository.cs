using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SlojPodataka
{
    // Nasleđivanje tehnološke klase DBUtils
    public class KategorijaRepository : DBUtils
    {
        public List<StarosnaKategorija> DohvatiSve()
        {
            var lista = new List<StarosnaKategorija>();

            using ( SqlConnection conn = new SqlConnection(ConnectionString) )
            {
                string query = "SELECT KategorijaID, NazivKategorije FROM StarosnaKategorija";

                // Kreiranje komande i dodela upita preko metode iz bazne klase DBUtils
                using ( SqlCommand cmd = KreirajKomandu(query, CommandType.Text, conn) )
                {
                    conn.Open();
                    using ( SqlDataReader reader = cmd.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            lista.Add(new StarosnaKategorija
                            {
                                KategorijaID = ( int ) reader["KategorijaID"],
                                NazivKategorije = reader["NazivKategorije"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }
    }
}