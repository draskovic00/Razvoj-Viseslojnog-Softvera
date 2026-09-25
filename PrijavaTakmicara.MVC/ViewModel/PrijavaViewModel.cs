using System.ComponentModel.DataAnnotations;

namespace PrijavaTakmicara.MVC.ViewModels
{
    public class PrijavaViewModel
    {
        // Identifikator prijave (koristi se kod izmene i detalja)
        public int PrijavaID { get; set; }

        [Required(ErrorMessage = "Naziv prvenstva je obavezno polje.")]
        [Display(Name = "Naziv Prvenstva")]
        public string NazivPrvenstva { get; set; }

        [Required(ErrorMessage = "Disciplina je obavezno polje.")]
        [Display(Name = "Disciplina")]
        public string Disciplinam { get; set; }

        [Required(ErrorMessage = "Mesto je obavezno polje.")]
        [Display(Name = "Mesto")]
        public string Mesto { get; set; }
    }
}