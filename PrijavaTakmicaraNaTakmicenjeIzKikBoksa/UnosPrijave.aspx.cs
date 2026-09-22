using PoslovnaLogika;
using SlojPodataka;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PrijavaTakmicaraNaTakmicenjeIzIza
{
    public partial class UnosPrijave : System.Web.UI.Page
    {
        private readonly KategorijaRepository _katRepo = new KategorijaRepository();
        private readonly ObradaPrijave _obrada = new ObradaPrijave();

        // Privremena lista za Detail stavke pre upisa u bazu
        private static List<StavkaPrijave> _privremeneStavke = new List<StavkaPrijave>();

        protected void Page_Load( object sender, EventArgs e )
        {
            if ( !IsPostBack )
            {
                // Prikaz imena ulogovanog trenera iz sesije
                if ( Session["ImeTrenera"] != null )
                {
                    lblImeTrenera.Text = Session["ImeTrenera"].ToString();
                }
                else if ( Session["KorisnikID"] != null )
                {
                    lblImeTrenera.Text = Session["KorisnikID"].ToString();
                }
                else
                {
                    lblImeTrenera.Text = "Trener";
                }

                UcitajKategorije();
            }
        }

        private void UcitajKategorije()
        {
            ddlKategorija.DataSource = _katRepo.DohvatiSve();
            ddlKategorija.DataTextField = "NazivKategorije";
            ddlKategorija.DataValueField = "KategorijaID";
            ddlKategorija.DataBind();
        }

        protected void btnDodajStavku_Click( object sender, EventArgs e )
        {
            if ( string.IsNullOrWhiteSpace(txtDatumRodjenja.Text) )
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Datum rođenja je obavezan!";
                return;
            }

            if ( string.IsNullOrWhiteSpace(txtImePrezime.Text) )
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Ime i prezime takmičara je obavezno!";
                return;
            }

            DateTime datumRodjenja = DateTime.Parse(txtDatumRodjenja.Text);

            // Precizan obračun godina na današnji dan
            int godine = DateTime.Now.Year - datumRodjenja.Year;
            if ( datumRodjenja.Date > DateTime.Now.AddYears(-godine) ) godine--;

            string selektovanaKatNaziv = ddlKategorija.SelectedItem.Text;
            int selektovanaKatID = int.Parse(ddlKategorija.SelectedValue);
            string izabranaDisciplina = ddlDisciplina.SelectedValue; // Uzimamo disciplinu sa forme prvenstva
            string urlServisa = ConfigurationManager.AppSettings["RestServisUrl"] ?? "https://localhost:44398";

            // Validacija poslovnog pravila sa REST servisa
            var pravila = _obrada.UcitajParametreSaServisa(urlServisa);
            string greska;
            bool ispravno = _obrada.ValidirajKategorijuiGodine(godine, selektovanaKatNaziv, pravila, out greska);

            if ( !ispravno )
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = greska;
                return;
            }

            // Dodavanje u privremenu listu
            _privremeneStavke.Add(new StavkaPrijave
            {
                ImePrezime = txtImePrezime.Text,
                DatumRodjenja = datumRodjenja,
                KategorijaID = selektovanaKatID,
                TezinskaKategorija = txtTezina.Text
            });

            // Prikaz u GridView u traženom redosledu:
            // Ime i prezime | Datum rođenja | Starosna kategorija | Disciplina | Težinska kategorija
            gvStavke.DataSource = _privremeneStavke.Select(s => new {
                ImeIPrezime = s.ImePrezime,
                DatumRodjenja = s.DatumRodjenja.ToString("dd.MM.yyyy."),
                StarosnaKategorija = selektovanaKatNaziv,
                Disciplina = izabranaDisciplina,
                TezinskaKategorija = s.TezinskaKategorija
            }).ToList();

            gvStavke.DataBind();

            lblStatus.ForeColor = System.Drawing.Color.Green;
            lblStatus.Text = "Takmičar uspešno dodat u listu!";
        }

        protected void btnSacuvajSve_Click( object sender, EventArgs e )
        {
            if ( _privremeneStavke.Count == 0 )
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Morate dodati bar jednog takmičara!";
                return;
            }

            var novaPrijava = new PrijavaTakmicara
            {
                NazivPrvenstva = txtNazivPrvenstva.Text,
                Disciplinam = ddlDisciplina.SelectedValue,
                Mesto = txtMesto.Text,
                DatumPrijave = DateTime.Now,
                KorisnikID = Session["KorisnikID"] != null ? ( int ) Session["KorisnikID"] : 1,
                StavkaPrijave = _privremeneStavke
            };

            bool uspesno = _obrada.SacuvajKompletnuPrijavu(novaPrijava);
            if ( uspesno )
            {
                Response.Redirect("StampaPrijave.aspx?id=" + novaPrijava.PrijavaID);
            }
            else
            {
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Greška prilikom čuvanja prijave u bazi!";
            }
        }
    }
}