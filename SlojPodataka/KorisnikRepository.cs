using System;
using System.Data;
using System.Data.SqlClient;

namespace SlojPodataka
{
    // Nasleđivanje tehnološke klase DBUtils
    public class KorisnikRepository : DBUtils
    {
        public Korisnik PrijavaKorisnika( string korisnickoIme, string lozinka )
        {
            Korisnik korisnik = null;

            using ( SqlConnection conn = new SqlConnection(ConnectionString) )
            {
                // Poziv bazne metode KreirajKomandu za dodelu Stored Procedure i parametara
                using ( SqlCommand cmd = KreirajKomandu("sp_ProveriKorisnika", CommandType.StoredProcedure, conn) )
                {
                    cmd.Parameters.AddWithValue("@KorisnickoIme", korisnickoIme);
                    cmd.Parameters.AddWithValue("@Lozinka", lozinka);

                    conn.Open();
                    using ( SqlDataReader reader = cmd.ExecuteReader() )
                    {
                        if ( reader.Read() )
                        {
                            korisnik = new Korisnik
                            {
                                KorisnikID = ( int ) reader["KorisnikID"],
                                KorisnickoIme = reader["KorisnickoIme"].ToString(),
                                ImePrezimeTrenera = reader["ImePrezimeTrenera"].ToString(),
                                NazivKluba = reader["NazivKluba"].ToString()
                            };
                        }
                    }
                }
            }
            return korisnik;
        }
    }
}