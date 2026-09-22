using System;
using System.Linq;
using System.Web.UI;
using SlojPodataka;

namespace PrijavaTakmicaraNaTakmicenjeIzIza
{
    public partial class Registracija : Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {
        }

        protected void btnRegistrujSe_Click( object sender, EventArgs e )
        {
            lblStatus.Text = "";

            string imePrezime = txtImePrezime.Text.Trim();
            string klub = txtKlub.Text.Trim();
            string korisnickoIme = txtKorisnickoIme.Text.Trim();
            string lozinka = txtLozinka.Text;
            string potvrdaLozinke = txtPotvrdaLozinke.Text;

            if ( string.IsNullOrEmpty(imePrezime) || string.IsNullOrEmpty(klub) ||
                string.IsNullOrEmpty(korisnickoIme) || string.IsNullOrEmpty(lozinka) )
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Sva polja su obavezna!";
                return;
            }

            if ( lozinka != potvrdaLozinke )
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Lozinke se ne poklapaju!";
                return;
            }

            try
            {
                using ( var db = new TakmicenjeDBEntities2() )
                {
                    var postojeci = db.Korisnik.FirstOrDefault(k => k.KorisnickoIme == korisnickoIme);
                    if ( postojeci != null )
                    {
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        lblStatus.Text = "Korisničko ime je već zauzeto!";
                        return;
                    }

                    var noviKorisnik = new Korisnik
                    {
                        ImePrezimeTrenera = imePrezime,
                        NazivKluba = klub,
                        KorisnickoIme = korisnickoIme,
                        Lozinka = lozinka
                    };

                    db.Korisnik.Add(noviKorisnik);
                    db.SaveChanges();

                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    lblStatus.Text = "Registracija uspešna! Preusmeravanje na prijavu...";

                    Response.AddHeader("REFRESH", "2;URL=Login.aspx");
                }
            }
            catch ( Exception ex )
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Greška pri registraciji: " + ex.Message;
            }
        }
    }
}