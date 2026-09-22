using System;
using System.Collections.Generic;
using System.Linq;
using SlojPodataka;

namespace PrijavaTakmicaraNaTakmicenjeIzIza
{
    public partial class StampaPrijave : System.Web.UI.Page
    {
        private readonly PrijavaRepository _prijavaRepo = new PrijavaRepository();
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

            // Dodajemo podrazumevanu opciju za sve kategorije
            ddlKategorijaFilter.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Sve kategorije", "0"));
        }

        private void UcitajiPrikaziPodatke()
        {
            if ( ViewState["PrijavaID"] == null ) return;

            int prijavaId = ( int ) ViewState["PrijavaID"];
            var prijava = _prijavaRepo.DohvatiPoId(prijavaId);

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

                    // Filtriranje po tekstu (Ime, Prezime ili Težinska kategorija)
                    string pretraga = txtPretraga.Text.Trim().ToLower();
                    if ( !string.IsNullOrEmpty(pretraga) )
                    {
                        query = query.Where(s =>
                            (s.ImePrezime != null && s.ImePrezime.ToLower().Contains(pretraga)) ||
                            (s.TezinskaKategorija != null && s.TezinskaKategorija.ToLower().Contains(pretraga))
                        );
                    }

                    var filtriraneStavke = query.Select(s => new
                    {
                        ImePrezime = s.ImePrezime,
                        DatumRodjenja = s.DatumRodjenja,
                        NazivKategorije = s.StarosnaKategorija != null ? s.StarosnaKategorija.NazivKategorije : "",
                        Disciplina = prijava.Disciplinam,
                        TezinskaKategorija = s.TezinskaKategorija
                    }).ToList();

                    if ( filtriraneStavke.Any() )
                    {
                        rptStavke.DataSource = filtriraneStavke;
                        rptStavke.DataBind();
                        rptStavke.Visible = true;
                        lblNemaPodataka.Visible = false;
                    }
                    else
                    {
                        rptStavke.Visible = false;
                        lblNemaPodataka.Visible = true;
                    }
                }
            }
        }

        protected void btnFiltriraj_Click( object sender, EventArgs e )
        {
            UcitajiPrikaziPodatke();
        }

        protected void btnResetuj_Click( object sender, EventArgs e )
        {
            ddlKategorijaFilter.SelectedValue = "0";
            txtPretraga.Text = string.Empty;
            UcitajiPrikaziPodatke();
        }
    }
}