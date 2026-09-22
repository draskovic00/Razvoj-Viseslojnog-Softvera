using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using SlojPodataka;

namespace PoslovnaLogika
{
    public class ModelKategorijeParametar
    {
        public string Kategorija { get; set; }
        public int MinGodina { get; set; }
        public int MaxGodina { get; set; }
    }

    public class PodaciParametara
    {
        public List<ModelKategorijeParametar> StarosneKategorije { get; set; }
    }

    public class ObradaPrijave
    {
        private readonly PrijavaRepository _prijavaRepo = new PrijavaRepository();

        // Čitanje parametara sa REST servisa
        public List<ModelKategorijeParametar> UcitajParametreSaServisa( string urlServisa )
        {
            using ( HttpClient client = new HttpClient() )
            {
                var response = client.GetAsync(urlServisa + "/api/prijava/parametri-kategorija").Result;
                if ( response.IsSuccessStatusCode )
                {
                    string jsonResult = response.Content.ReadAsStringAsync().Result;
                    var podaci = JsonConvert.DeserializeObject<PodaciParametara>(jsonResult);
                    return podaci?.StarosneKategorije;
                }
            }
            return new List<ModelKategorijeParametar>();
        }

        // Poslovno pravilo: AKO takmičar ima N godina ONDA pripada određenoj kategoriji
        // Poslovno pravilo: AKO takmičar ima N godina ONDA pripada određenoj kategoriji
        public bool ValidirajKategorijuiGodine( int godine, string izabranaKategorija, List<ModelKategorijeParametar> pravila, out string porukaGreske )
        {
            porukaGreske = "";

            // 1. Pronalazimo koja kategorija zapravo odgovara godinama takmičara
            var odgovarajucePravilo = pravila.Find(p => godine >= p.MinGodina && godine <= p.MaxGodina);

            // 2. Proveravamo da li se izabrana kategorija poklapa sa pravilom za te godine
            if ( odgovarajucePravilo != null )
            {
                if ( !odgovarajucePravilo.Kategorija.Equals(izabranaKategorija, StringComparison.OrdinalIgnoreCase) )
                {
                    porukaGreske = $"Takmičar sa {godine} god. po pravilu mora biti u kategoriji '{odgovarajucePravilo.Kategorija}' (raspon {odgovarajucePravilo.MinGodina}-{odgovarajucePravilo.MaxGodina} god.).";
                    return false;
                }
            }
            else
            {
                // Alternativna provera ako izabrana kategorija postoji u pravilima ali takmičar ne spada po godinama
                var izabranoPravilo = pravila.Find(p => p.Kategorija.Equals(izabranaKategorija, StringComparison.OrdinalIgnoreCase));
                if ( izabranoPravilo != null && (godine < izabranoPravilo.MinGodina || godine > izabranoPravilo.MaxGodina) )
                {
                    porukaGreske = $"Takmičar sa {godine} god. ne može biti u kategoriji '{izabranaKategorija}' (dozvoljeni raspon je {izabranoPravilo.MinGodina}-{izabranoPravilo.MaxGodina} god.).";
                    return false;
                }
            }

            return true;
        }

        // Sačuvanje celina-deo (Master-Detail) kroz Entity Framework
        public bool SacuvajKompletnuPrijavu( PrijavaTakmicara prijava )
        {
            return _prijavaRepo.Dodaj(prijava);
        }
    }
}