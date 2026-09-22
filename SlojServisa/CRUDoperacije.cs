using System.Collections.Generic;
using System.Linq;
using SlojPodataka;

namespace SlojServisa
{
    public class CRUDoperacije
    {
        private readonly PrijavaRepository _prijavaRepo = new PrijavaRepository();

        // READ - Dohvatanje svih prijava
        public List<PrijavaTakmicara> DohvatiSvePrijave()
        {
            return _prijavaRepo.DohvatiSve();
        }

        // READ SINGLE - Dohvatanje jedne prijave po ID-u
        public PrijavaTakmicara DohvatiPrijavuPoId( int id )
        {
            return _prijavaRepo.DohvatiPoId(id);
        }

        // CREATE - Unos nove prijave sa stavkama
        public bool DodajPrijavu( PrijavaTakmicara prijava )
        {
            return _prijavaRepo.Dodaj(prijava);
        }

        // UPDATE - Izmena postojeće prijave
        public bool IzmeniPrijavu( PrijavaTakmicara prijava )
        {
            return _prijavaRepo.Izmeni(prijava);
        }

        // DELETE - Brisanje prijave po ID-u
        public bool ObrisiPrijavu( int id )
        {
            return _prijavaRepo.Obrisi(id);
        }
    }
}