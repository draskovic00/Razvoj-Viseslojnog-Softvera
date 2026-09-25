using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using SlojPodataka;
using SlojServisa;

namespace PrijavaTakmicaraNaTakmicenjeIzIza
{
    public partial class StampaPrijave : System.Web.UI.Page
    {
        private readonly CRUDoperacije _crudService = new CRUDoperacije();
        private readonly KategorijaRepository _katRepo = new KategorijaRepository();

        protected void Page_Load( object sender, EventArgs e )
        {
            if ( !IsPostBack )
            {
                UcitajKategorijeUFilter();

                if ( Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int prijavaId) )
                {
                    ViewState["PrijavaID"] = prijavaId;
                    UcitajiPrikaziPodatke();
                }
            }
        }

        private void UcitajKategorijeUFilter()
        {
            var kategorije = _katRepo.DohvatiSve();
            ddlKategorijaFilter.DataSource = kategorije;
            ddlKategorijaFilter.DataTextField = "NazivKategorije";
            ddlKategorijaFilter.DataValueField = "KategorijaID";
            ddlKategorijaFilter.DataBind();

            ddlKategorijaFilter.Items.Insert(0, new ListItem("Sve kategorije", "0"));
        }

        private void UcitajiPrikaziPodatke()
        {
            if ( ViewState["PrijavaID"] == null ) return;

            int prijavaId = ( int ) ViewState["PrijavaID"];
            var prijava = _crudService.DohvatiPrijavuPoId(prijavaId);

            if ( prijava != null )
            {
                lblPrvenstvo.Text = prijava.NazivPrvenstva;
                lblDisciplina.Text = prijava.Disciplinam;
                lblDatum.Text = prijava.DatumPrijave.ToString("dd.MM.yyyy.");
                lblMesto.Text = prijava.Mesto;

                if ( prijava.Korisnik != null )
                {
                    lblKlub.Text = prijava.Korisnik.NazivKluba;
                    lblTrener.Text = prijava.Korisnik.ImePrezimeTrenera;
                }

                if ( prijava.StavkaPrijave != null )
                {
                    var query = prijava.StavkaPrijave.AsEnumerable();

                    // Filtriranje po starosnoj kategoriji
                    int izabranaKatId = Convert.ToInt32(ddlKategorijaFilter.SelectedValue);
                    if ( izabranaKatId > 0 )
                    {
                        query = query.Where(s => s.KategorijaID == izabranaKatId);
                    }

                    // Filtriranje po tekstu
                    string pretraga = txtPretraga.Text.Trim().ToLower();
                    if ( !string.IsNullOrEmpty(pretraga) )
                    {
                        query = query.Where(s =>
                            (s.ImePrezime != null && s.ImePrezime.ToLower().Contains(pretraga)) ||
                            (s.TezinskaKategorija != null && s.TezinskaKategorija.ToLower().Contains(pretraga))
                        );
                    }

                    var filtriraneStavke = query.Select(( s, index ) => new
                    {
                        Index = index,
                        StavkaID = s.StavkaID,
                        ImePrezime = s.ImePrezime,
                        DatumRodjenja = s.DatumRodjenja.ToString("yyyy-MM-dd"),
                        KategorijaID = s.KategorijaID,
                        NazivKategorije = s.StarosnaKategorija != null ? s.StarosnaKategorija.NazivKategorije : "",
                        Disciplina = prijava.Disciplinam,
                        TezinskaKategorija = s.TezinskaKategorija
                    }).ToList();

                    if ( filtriraneStavke.Any() )
                    {
                        gvStavke.DataSource = filtriraneStavke;
                        gvStavke.DataBind();
                        gvStavke.Visible = true;
                        lblNemaPodataka.Visible = false;
                    }
                    else
                    {
                        gvStavke.Visible = false;
                        lblNemaPodataka.Visible = true;
                    }
                }
            }
        }

        protected void btnFiltriraj_Click( object sender, EventArgs e )
        {
            gvStavke.EditIndex = -1;
            UcitajiPrikaziPodatke();
        }

        protected void btnResetuj_Click( object sender, EventArgs e )
        {
            gvStavke.EditIndex = -1;
            ddlKategorijaFilter.SelectedValue = "0";
            txtPretraga.Text = string.Empty;
            UcitajiPrikaziPodatke();
        }

        protected void btnObrisiPrijavu_Click( object sender, EventArgs e )
        {
            if ( ViewState["PrijavaID"] != null )
            {
                int prijavaId = ( int ) ViewState["PrijavaID"];
                bool uspesno = _crudService.ObrisiPrijavu(prijavaId);

                if ( uspesno )
                {
                    Response.Redirect("UnosPrijave.aspx");
                }
                else
                {
                    lblNemaPodataka.Text = "Greška prilikom brisanja prijave iz baze!";
                    lblNemaPodataka.Visible = true;
                }
            }
        }

        #region CRUD Nad Pojedinačnim Takmičarima (Detail stavkama)

        protected void gvStavke_RowEditing( object sender, GridViewEditEventArgs e )
        {
            gvStavke.EditIndex = e.NewEditIndex;
            UcitajiPrikaziPodatke();
        }

        protected void gvStavke_RowCancelingEdit( object sender, GridViewCancelEditEventArgs e )
        {
            gvStavke.EditIndex = -1;
            UcitajiPrikaziPodatke();
        }

        protected void gvStavke_RowUpdating( object sender, GridViewUpdateEventArgs e )
        {
            int index = e.RowIndex;

            GridViewRow row = gvStavke.Rows[index];

            TextBox txtEditIme = ( TextBox ) row.FindControl("txtEditImePrezime");
            TextBox txtEditDatum = ( TextBox ) row.FindControl("txtEditDatumRodjenja");
            DropDownList ddlEditKat = ( DropDownList ) row.FindControl("ddlEditKategorija");
            TextBox txtEditTezina = ( TextBox ) row.FindControl("txtEditTezina");

            if ( ViewState["PrijavaID"] != null && txtEditIme != null && txtEditDatum != null && ddlEditKat != null && txtEditTezina != null )
            {
                int prijavaId = ( int ) ViewState["PrijavaID"];

                // Sveže dohvatanje cele prijave
                var prijava = _crudService.DohvatiPrijavuPoId(prijavaId);

                if ( prijava != null && prijava.StavkaPrijave != null )
                {
                    // Pronalaženje stavke preko DataKey (StavkaID) ili preko indeksa reda
                    StavkaPrijave stavka = null;
                    if ( gvStavke.DataKeys[index] != null && Convert.ToInt32(gvStavke.DataKeys[index].Value) > 0 )
                    {
                        int stavkaId = Convert.ToInt32(gvStavke.DataKeys[index].Value);
                        stavka = prijava.StavkaPrijave.FirstOrDefault(s => s.StavkaID == stavkaId);
                    }

                    if ( stavka == null && index < prijava.StavkaPrijave.Count )
                    {
                        stavka = prijava.StavkaPrijave.ElementAt(index);
                    }

                    if ( stavka != null )
                    {
                        // Fleksibilno parsiranje datuma u više formata (srpski i ISO)
                        string[] podrznatiFormati = { "yyyy-MM-dd", "dd.MM.yyyy.", "dd.MM.yyyy", "d.M.yyyy.", "d.M.yyyy" };
                        string unetiDatumText = txtEditDatum.Text.Trim();

                        bool datumUspesno = DateTime.TryParseExact(unetiDatumText, podrznatiFormati,
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out DateTime noviDatum)
                            || DateTime.TryParse(unetiDatumText, out noviDatum);

                        if ( datumUspesno )
                        {
                            stavka.ImePrezime = txtEditIme.Text.Trim();
                            stavka.DatumRodjenja = noviDatum;
                            stavka.KategorijaID = int.Parse(ddlEditKat.SelectedValue);
                            stavka.TezinskaKategorija = txtEditTezina.Text.Trim();

                            // Poziv servisa za izmenu
                            bool uspesno = _crudService.IzmeniPrijavu(prijava);

                            if ( !uspesno )
                            {
                                lblNemaPodataka.Text = "Greška: Izmena nije sačuvana u bazi podataka!";
                                lblNemaPodataka.Visible = true;
                                return;
                            }
                        }
                        else
                        {
                            lblNemaPodataka.Text = "Neispravan format datuma! Unesite datum npr. 2005-05-15 ili 15.05.2005.";
                            lblNemaPodataka.Visible = true;
                            return;
                        }
                    }
                }
            }

            gvStavke.EditIndex = -1;
            UcitajiPrikaziPodatke();
        }

        protected void gvStavke_RowDeleting( object sender, GridViewDeleteEventArgs e )
        {
            int index = e.RowIndex;

            if ( ViewState["PrijavaID"] != null )
            {
                int prijavaId = ( int ) ViewState["PrijavaID"];
                var prijava = _crudService.DohvatiPrijavuPoId(prijavaId);

                if ( prijava != null && prijava.StavkaPrijave != null )
                {
                    StavkaPrijave stavkaZaBrisanje = null;
                    if ( gvStavke.DataKeys[index] != null && Convert.ToInt32(gvStavke.DataKeys[index].Value) > 0 )
                    {
                        int stavkaId = Convert.ToInt32(gvStavke.DataKeys[index].Value);
                        stavkaZaBrisanje = prijava.StavkaPrijave.FirstOrDefault(s => s.StavkaID == stavkaId);
                    }

                    if ( stavkaZaBrisanje == null && index < prijava.StavkaPrijave.Count )
                    {
                        stavkaZaBrisanje = prijava.StavkaPrijave.ElementAt(index);
                    }

                    if ( stavkaZaBrisanje != null )
                    {
                        prijava.StavkaPrijave.Remove(stavkaZaBrisanje);
                        _crudService.IzmeniPrijavu(prijava);
                    }
                }
            }

            gvStavke.EditIndex = -1;
            UcitajiPrikaziPodatke();
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
    }
}