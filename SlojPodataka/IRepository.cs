using System.Collections.Generic;

namespace SlojPodataka
{
    public interface IRepository<T> where T : class
    {
        List<T> DohvatiSve();
        T DohvatiPoId( int id );
        bool Dodaj( T entitet );
        bool Izmeni( T entitet );
        bool Obrisi( int id );
    }
}