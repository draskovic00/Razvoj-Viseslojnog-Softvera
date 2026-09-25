using SlojPodataka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Http;
namespace SlojServisa.Controllers
{
    public class PrijavaKontroler : ApiController
    {
        private readonly PrijavaRepository _prijavaRepo = new PrijavaRepository();
        private readonly KorisnikRepository _korisnikRepo = new KorisnikRepository();

        // 1. REST servis kao međusloj za CRUD operacije
        [HttpGet]
        [Route("api/prijava/dohvati-sve")]
        public IHttpActionResult DohvatiSvePrijave()
        {
            var liste = _prijavaRepo.DohvatiSve();
            return Ok(liste);
        }

        [HttpGet]
        [Route("api/prijava/dohvati/{id}")]
        public IHttpActionResult DohvatiPoId( int id )
        {
            var prijava = _prijavaRepo.DohvatiPoId(id);
            if ( prijava == null ) return NotFound();
            return Ok(prijava);
        }

        [HttpPost]
        [Route("api/prijava/dodaj")]
        public IHttpActionResult DodajPrijavu( [FromBody] PrijavaTakmicara prijava )
        {
            if ( prijava == null ) return BadRequest();
            bool uspesno = _prijavaRepo.Dodaj(prijava);
            if ( uspesno ) return Ok("Prijava uspešno sačuvana.");
            return InternalServerError();
        }

        // 2. REST servis koji obezbeđuje parametre za poslovnu logiku iz JSON-a
        [HttpGet]
        [Route("api/prijava/parametri-kategorija")]
        public HttpResponseMessage DohvatiParametreKategorija()
        {
            try
            {
                string putanja = AppDomain.CurrentDomain.BaseDirectory + "poslovnapravila.json";
                if ( !File.Exists(putanja) )
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);

                string jsonSadrzaj = File.ReadAllText(putanja);

                var response = Request.CreateResponse(System.Net.HttpStatusCode.OK);
                response.Content = new StringContent(jsonSadrzaj, System.Text.Encoding.UTF8, "application/json");
                return response;
            }
            catch ( Exception ex )
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}