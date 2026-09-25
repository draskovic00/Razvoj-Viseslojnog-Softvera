using PoslovnaLogika;
using SlojPodataka;
using SlojServisa; // Dodato za CRUDoperacije
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;

namespace PrijavaTakmicaraNaTakmicenjeIzIza
{
    public partial class UnosPrijave : System.Web.UI.Page
    {
        private readonly KategorijaRepository _katRepo = new KategorijaRepository();
        private readonly ObradaPrijave _obrada = new ObradaPrijave();
        private readonly CRUDoperacije _crudService = new CRUDoperacije(); // Instanca servisa za CRUD operacije

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

        private void OsveziGrid()
        {
            var kategorije = _katRepo.DohvatiSve();
            string izabranaDisciplina = ddlDisciplina.SelectedValue;

            gvStavke.DataSource = _privremeneStavke.Select(s => new
            {
                ImePrezime = s.ImePrezime,
                DatumRodjenja = s.DatumRodjenja.ToString("yyyy-MM-dd"),
                KategorijaID = s.KategorijaID,
                StarosnaKategorija = kategorije.FirstOrDefault(k => k.KategorijaID == s.KategorijaID)?.NazivKategorije ?? "",
                Disciplina = izabranaDisciplina,
                TezinskaKategorija = s.TezinskaKategorija
            }).ToList();

            gvStavke.DataBind();
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

            OsveziGrid();

            // Reset unosa za novog takmičara
            txtImePrezime.Text = "";
            txtDatumRodjenja.Text = "";
            txtTezina.Text = "";

            lblStatus.ForeColor = System.Drawing.Color.Green;
            lblStatus.Text = "Takmičar uspešno dodat u listu!";
        }

        #region GridView CRUD Operacije nad stavkama

        protected void gvStavke_RowEditing( object sender, GridViewEditEventArgs e )
        {
            gvStavke.EditIndex = e.NewEditIndex;
            OsveziGrid();
        }

        protected void gvStavke_RowCancelingEdit( object sender, GridViewCancelEditEventArgs e )
        {
            gvStavke.EditIndex = -1;
            OsveziGrid();
        }

        protected void gvStavke_RowUpdating( object sender, GridViewUpdateEventArgs e )
        {
            int index = e.RowIndex;
            GridViewRow row = gvStavke.Rows[index];

            TextBox txtEditIme = ( TextBox ) row.FindControl("txtEditImePrezime");
            TextBox txtEditDatum = ( TextBox ) row.FindControl("txtEditDatumRodjenja");
            DropDownList ddlEditKat = ( DropDownList ) row.FindControl("ddlEditKategorija");
            TextBox txtEditTezina = ( TextBox ) row.FindControl("txtEditTezina");

            if ( txtEditIme != null && txtEditDatum != null && ddlEditKat != null && txtEditTezina != null )
            {
                if ( DateTime.TryParse(txtEditDatum.Text, out DateTime noviDatum) )
                {
                    _privremeneStavke[index].ImePrezime = txtEditIme.Text;
                    _privremeneStavke[index].DatumRodjenja = noviDatum;
                    _privremeneStavke[index].KategorijaID = int.Parse(ddlEditKat.SelectedValue);
                    _privremeneStavke[index].TezinskaKategorija = txtEditTezina.Text;

                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    lblStatus.Text = "Stavka uspešno izmenjena!";
                }
                else
                {
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    lblStatus.Text = "Neispravan format datuma!";
                    return;
                }
            }

            gvStavke.EditIndex = -1;
            OsveziGrid();
        }

        protected void gvStavke_RowDeleting( object sender, GridViewDeleteEventArgs e )
        {
            int index = e.RowIndex;
            if ( index >= 0 && index < _privremeneStavke.Count )
            {
                _privremeneStavke.RemoveAt(index);
                OsveziGrid();

                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = "Takmičar uklonjen iz liste!";
            }
        }

        protected void gvStavke_RowDataBound( object sender, GridViewRowEventArgs e )
        {
            if ( e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) > 0 )
            {
                DropDownList ddlEditKat = ( DropDownList ) e.Row.FindControl("ddlEditKategorija");
                HiddenField hfKatID = ( HiddenField ) e.Row.FindControl("hfSelectedKatID");

                if ( ddlEditKat != null )
                {
                    ddlEditKat.DataSource = _katRepo.DohvatiSve();
                    ddlEditKat.DataTextField = "NazivKategorije";
                    ddlEditKat.DataValueField = "KategorijaID";
                    ddlEditKat.DataBind();

                    if ( hfKatID != null && !string.IsNullOrEmpty(hfKatID.Value) )
                    {
                        ddlEditKat.SelectedValue = hfKatID.Value;
                    }
                }
            }
        }

        #endregion

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

            // Korišćenje CRUDoperacije servisa za čuvanje
            bool uspesno = _crudService.DodajPrijavu(novaPrijava);

            if ( uspesno )
            {
                _privremeneStavke.Clear(); // Pražnjenje privremene liste
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