using System;
using SlojPodataka;

namespace PrijavaTakmicaraNaTakmicenjeIzIza
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly KorisnikRepository _korisnikRepo = new KorisnikRepository();

        protected void btnLogin_Click( object sender, EventArgs e )
        {
            var korisnik = _korisnikRepo.PrijavaKorisnika(txtKorisnik.Text, txtLozinka.Text);
            if ( korisnik != null )
            {
                Session["KorisnikID"] = korisnik.KorisnikID;
                Session["Trener"] = korisnik.ImePrezimeTrenera;
                Session["Klub"] = korisnik.NazivKluba;
                Response.Redirect("UnosPrijave.aspx");
            }
            else
            {
                lblGreska.Text = "Neispravno korisničko ime ili lozinka!";
            }
        }
    }
}