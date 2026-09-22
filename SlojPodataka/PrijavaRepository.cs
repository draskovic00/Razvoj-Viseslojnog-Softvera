using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SlojPodataka
{
    // Nasleđivanje tehnološke klase DBUtils
    public class PrijavaRepository : DBUtils, IRepository<PrijavaTakmicara>
    {
        public List<PrijavaTakmicara> DohvatiSve()
        {
            var lista = new List<PrijavaTakmicara>();

            using ( SqlConnection conn = new SqlConnection(ConnectionString) )
            {
                string query = "SELECT PrijavaID, NazivPrvenstva, Disciplinam, DatumPrijave, Mesto, KorisnikID FROM PrijavaTakmicara";
                using ( SqlCommand cmd = KreirajKomandu(query, CommandType.Text, conn) )
                {
                    conn.Open();
                    using ( SqlDataReader reader = cmd.ExecuteReader() )
                    {
                        while ( reader.Read() )
                        {
                            lista.Add(new PrijavaTakmicara
                            {
                                PrijavaID = ( int ) reader["PrijavaID"],
                                NazivPrvenstva = reader["NazivPrvenstva"].ToString(),
                                Disciplinam = reader["Disciplinam"].ToString(),
                                DatumPrijave = ( DateTime ) reader["DatumPrijave"],
                                Mesto = reader["Mesto"].ToString(),
                                KorisnikID = ( int ) reader["KorisnikID"]
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public PrijavaTakmicara DohvatiPoId(int id)
{
    PrijavaTakmicara prijava = null;

    using (SqlConnection conn = new SqlConnection(ConnectionString))
    {
        conn.Open();

        // 1. Čitanje Master zapisa + Korisnik podataka (JOIN)
        string queryMaster = @"SELECT p.PrijavaID, p.NazivPrvenstva, p.Disciplinam, p.DatumPrijave, p.Mesto, p.KorisnikID,
                                      k.NazivKluba, k.ImePrezimeTrenera, k.KorisnickoIme
                               FROM PrijavaTakmicara p
                               INNER JOIN Korisnik k ON p.KorisnikID = k.KorisnikID
                               WHERE p.PrijavaID = @PrijavaID";

        using (SqlCommand cmd = KreirajKomandu(queryMaster, CommandType.Text, conn))
        {
            cmd.Parameters.AddWithValue("@PrijavaID", id);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    prijava = new PrijavaTakmicara
                    {
                        PrijavaID = (int)reader["PrijavaID"],
                        NazivPrvenstva = reader["NazivPrvenstva"].ToString(),
                        Disciplinam = reader["Disciplinam"].ToString(),
                        DatumPrijave = (DateTime)reader["DatumPrijave"],
                        Mesto = reader["Mesto"].ToString(),
                        KorisnikID = (int)reader["KorisnikID"],
                        Korisnik = new Korisnik
                        {
                            KorisnikID = (int)reader["KorisnikID"],
                            NazivKluba = reader["NazivKluba"].ToString(),
                            ImePrezimeTrenera = reader["ImePrezimeTrenera"].ToString(),
                            KorisnickoIme = reader["KorisnickoIme"].ToString()
                        },
                        StavkaPrijave = new List<StavkaPrijave>()
                    };
                }
            }
        }

        // 2. Čitanje Detail zapisa (Stavke prijave)
        if (prijava != null)
        {
            string queryDetail = @"SELECT s.StavkaID, s.PrijavaID, s.ImePrezime, s.DatumRodjenja, s.KategorijaID, s.TezinskaKategorija,
                                          k.NazivKategorije
                                   FROM StavkaPrijave s
                                   LEFT JOIN StarosnaKategorija k ON s.KategorijaID = k.KategorijaID
                                   WHERE s.PrijavaID = @PrijavaID";

            using (SqlCommand cmdDetail = KreirajKomandu(queryDetail, CommandType.Text, conn))
            {
                cmdDetail.Parameters.AddWithValue("@PrijavaID", id);
                using (SqlDataReader readerDetail = cmdDetail.ExecuteReader())
                {
                    while (readerDetail.Read())
                    {
                                prijava.StavkaPrijave.Add(new StavkaPrijave
                                {
                                    StavkaID = ( int ) readerDetail["StavkaID"],
                                    PrijavaID = ( int ) readerDetail["PrijavaID"],
                                    ImePrezime = readerDetail["ImePrezime"].ToString(),
                                    DatumRodjenja = ( DateTime ) readerDetail["DatumRodjenja"],
                                    KategorijaID = ( int ) readerDetail["KategorijaID"],
                                    TezinskaKategorija = readerDetail["TezinskaKategorija"].ToString(),


                            StarosnaKategorija = new StarosnaKategorija
                            {
                                KategorijaID = ( int ) readerDetail["KategorijaID"],
                                NazivKategorije = readerDetail["NazivKategorije"] != DBNull.Value
                        ? readerDetail["NazivKategorije"].ToString()
                        : ""
                            }
                                });
                    }
                }
            }
        }
    }

    return prijava;
}

        // UNOS PREKO STORED PROCEDURA I DBUtils SA TRANSAKCIJOM
        public bool Dodaj( PrijavaTakmicara entitet )
        {
            using ( SqlConnection conn = new SqlConnection(ConnectionString) )
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Unos Master zapisa preko SP
                    using ( SqlCommand cmdMaster = KreirajKomandu("sp_UnesiPrijavu", CommandType.StoredProcedure, conn) )
                    {
                        cmdMaster.Transaction = transaction;

                        cmdMaster.Parameters.AddWithValue("@NazivPrvenstva", entitet.NazivPrvenstva);
                        cmdMaster.Parameters.AddWithValue("@Disciplinam", entitet.Disciplinam);
                        cmdMaster.Parameters.AddWithValue("@Mesto", entitet.Mesto);
                        cmdMaster.Parameters.AddWithValue("@KorisnikID", entitet.KorisnikID);

                        SqlParameter outIdParam = new SqlParameter("@PrijavaID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmdMaster.Parameters.Add(outIdParam);

                        cmdMaster.ExecuteNonQuery();

                        int novaPrijavaID = ( int ) outIdParam.Value;
                        entitet.PrijavaID = novaPrijavaID;

                        // 2. Unos Detail stavki u petlji preko SP
                        if ( entitet.StavkaPrijave != null )
                        {
                            foreach ( var stavka in entitet.StavkaPrijave )
                            {
                                using ( SqlCommand cmdDetail = KreirajKomandu("sp_UnesiStavkuPrijave", CommandType.StoredProcedure, conn) )
                                {
                                    cmdDetail.Transaction = transaction;

                                    cmdDetail.Parameters.AddWithValue("@PrijavaID", novaPrijavaID);
                                    cmdDetail.Parameters.AddWithValue("@ImePrezime", stavka.ImePrezime);
                                    cmdDetail.Parameters.AddWithValue("@DatumRodjenja", stavka.DatumRodjenja);
                                    cmdDetail.Parameters.AddWithValue("@KategorijaID", stavka.KategorijaID);
                                    cmdDetail.Parameters.AddWithValue("@TezinskaKategorija", stavka.TezinskaKategorija);

                                    cmdDetail.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch ( Exception )
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool Izmeni( PrijavaTakmicara entitet )
        {
            using ( SqlConnection conn = new SqlConnection(ConnectionString) )
            {
                string query = "UPDATE PrijavaTakmicara SET NazivPrvenstva=@NazivPrvenstva, Disciplinam=@Disciplinam, Mesto=@Mesto WHERE PrijavaID=@PrijavaID";
                using ( SqlCommand cmd = KreirajKomandu(query, CommandType.Text, conn) )
                {
                    cmd.Parameters.AddWithValue("@NazivPrvenstva", entitet.NazivPrvenstva);
                    cmd.Parameters.AddWithValue("@Disciplinam", entitet.Disciplinam);
                    cmd.Parameters.AddWithValue("@Mesto", entitet.Mesto);
                    cmd.Parameters.AddWithValue("@PrijavaID", entitet.PrijavaID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Obrisi( int id )
        {
            using ( SqlConnection conn = new SqlConnection(ConnectionString) )
            {
                string query = "DELETE FROM PrijavaTakmicara WHERE PrijavaID = @PrijavaID";
                using ( SqlCommand cmd = KreirajKomandu(query, CommandType.Text, conn) )
                {
                    cmd.Parameters.AddWithValue("@PrijavaID", id);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}