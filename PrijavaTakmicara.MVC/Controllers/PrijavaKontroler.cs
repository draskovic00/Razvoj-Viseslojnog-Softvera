using System.Web.Mvc;
using SlojServisa;
using PrijavaTakmicara.MVC.ViewModels; // Uvozimo ViewModel

namespace PrijavaTakmicara.MVC.Controllers
{
    public class PrijavaController : Controller
    {
        private readonly CRUDoperacije _crudService = new CRUDoperacije();

        // 1. GET: /Prijava/Prikaz - Prikaz svih prijava
        public ActionResult Prikaz()
        {
            var prijave = _crudService.DohvatiSvePrijave();
            return View(prijave); // Otvara Views/Prijava/Prikaz.cshtml
        }

        // 2. GET: /Prijava/Detalji/5 - Detalji jedne prijave
        public ActionResult Detalji( int id )
        {
            var prijava = _crudService.DohvatiPrijavuPoId(id);
            if ( prijava == null )
            {
                return HttpNotFound();
            }
            return View(prijava); // Otvara Views/Prijava/Detalji.cshtml
        }

        // 3. GET: /Prijava/Kreiraj - Prikaz prazne forme za novu prijavu
        public ActionResult Kreiraj()
        {
            var viewModel = new PrijavaViewModel();
            return View(viewModel); // Otvara Views/Prijava/Kreiraj.cshtml
        }

        // 4. POST: /Prijava/Kreiraj - Čuvanje nove prijave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Kreiraj( PrijavaViewModel viewModel )
        {
            if ( ModelState.IsValid )
            {
                // Mapiranje sa ViewModel-a na domain model baze
                var novaPrijava = new SlojPodataka.PrijavaTakmicara
                {
                    NazivPrvenstva = viewModel.NazivPrvenstva,
                    Disciplinam = viewModel.Disciplinam,
                    Mesto = viewModel.Mesto
                };

                bool uspesno = _crudService.DodajPrijavu(novaPrijava);
                if ( uspesno )
                {
                    return RedirectToAction("Prikaz");
                }
                ModelState.AddModelError("", "Greška prilikom čuvanja u bazi!");
            }

            return View(viewModel);
        }

        // 5. GET: /Prijava/Izmeni/5 - Prikaz forme popunjene postojećim podacima
        public ActionResult Izmeni( int id )
        {
            var prijava = _crudService.DohvatiPrijavuPoId(id);
            if ( prijava == null )
            {
                return HttpNotFound();
            }

            // Mapiranje iz baze na ViewModel
            var viewModel = new PrijavaViewModel
            {
                PrijavaID = prijava.PrijavaID,
                NazivPrvenstva = prijava.NazivPrvenstva,
                Disciplinam = prijava.Disciplinam,
                Mesto = prijava.Mesto
            };

            return View(viewModel); // Otvara Views/Prijava/Izmeni.cshtml
        }

        // 6. POST: /Prijava/Izmeni/5 - Čuvanje izmena postojeće prijave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Izmeni( PrijavaViewModel viewModel )
        {
            if ( ModelState.IsValid )
            {
                // Mapiranje sa ViewModel-a na domain model baze za ažuriranje
                var prijavaZaIzmenu = new SlojPodataka.PrijavaTakmicara
                {
                    PrijavaID = viewModel.PrijavaID,
                    NazivPrvenstva = viewModel.NazivPrvenstva,
                    Disciplinam = viewModel.Disciplinam,
                    Mesto = viewModel.Mesto
                };

                bool uspesno = _crudService.IzmeniPrijavu(prijavaZaIzmenu);
                if ( uspesno )
                {
                    return RedirectToAction("Prikaz");
                }
                ModelState.AddModelError("", "Greška prilikom čuvanja izmena u bazi!");
            }

            return View(viewModel);
        }

        // 7. POST: /Prijava/Obrisi/5 - Brisanje prijave
        [HttpPost]
        public ActionResult Obrisi( int id )
        {
            _crudService.ObrisiPrijavu(id);
            return RedirectToAction("Prikaz");
        }
    }
}