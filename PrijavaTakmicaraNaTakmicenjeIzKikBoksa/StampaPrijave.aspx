<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StampaPrijave.aspx.cs" Inherits="PrijavaTakmicaraNaTakmicenjeIzIza.StampaPrijave" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link href="css/Stilovi.css" rel="stylesheet" type="text/css" />
    <title>Štampa Prijave Takmičara</title>
    <style>
        .zaglavlje { 
            margin-bottom: 20px; 
            line-height: 1.5; 
            background: #f8f9fa;
            padding: 15px;
            border-left: 4px solid #e67e22;
            border-radius: 4px;
        }
        .naslov { 
            text-align: center; 
            font-size: 22px; 
            font-weight: bold; 
            margin: 20px 0 10px 0; 
            color: #1a252f;
            text-transform: uppercase;
        }
        .linija { 
            border-bottom: 2px solid #e67e22; 
            margin-bottom: 20px; 
        }
        .tabela-info { 
            width: 100%; 
            margin-bottom: 20px; 
            font-size: 0.95rem;
        }
        .tabela-info td { 
            padding: 6px; 
        }
        .okvir-polje {
            border: 1px solid #ddd;
            padding: 10px;
            border-radius: 5px;
            background-color: #fafafa;
        }
        .filter-sekcija {
            background-color: #eef2f5;
            padding: 15px;
            border-radius: 6px;
            margin-bottom: 20px;
            display: flex;
            gap: 15px;
            align-items: flex-end;
        }
        .filter-grupa {
            display: flex;
            flex-direction: column;
        }
        .filter-grupa label {
            font-size: 0.85rem;
            font-weight: bold;
            margin-bottom: 5px;
        }
        .akcije-traka {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
        }
        @media print {
            .no-print { display: none !important; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Gornja komandna traka (sakriva se prilikom štampe) -->
            <div class="no-print">
                <div class="akcije-traka">
                    <asp:Button ID="btnObrisiPrijavu" runat="server" Text="Obriši celokupnu prijavu" CssClass="btn btn-danger" 
                        OnClick="btnObrisiPrijavu_Click" OnClientClick="return confirm('Da li ste sigurni da želite da obrišete kompletnu prijavu iz baze?');" />
                    <button onclick="window.print(); return false;" class="btn btn-primary">Štampaj dokument</button>
                </div>

                <div class="filter-sekcija">
                    <div class="filter-grupa">
                        <label>Starosna kategorija:</label>
                        <asp:DropDownList ID="ddlKategorijaFilter" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                    </div>

                    <div class="filter-grupa">
                        <label>Težinska kategorija / Pretraga:</label>
                        <asp:TextBox ID="txtPretraga" runat="server" CssClass="form-control" placeholder="Ime, prezime ili težina..."></asp:TextBox>
                    </div>

                    <asp:Button ID="btnFiltriraj" runat="server" Text="Filtriraj" CssClass="btn btn-primary" OnClick="btnFiltriraj_Click" />
                    <asp:Button ID="btnResetuj" runat="server" Text="Prikaži sve" CssClass="btn" OnClick="btnResetuj_Click" />
                </div>
            </div>

            <div class="zaglavlje">
                Univerzitet u Novom Sadu<br />
                Tehnički fakultet „Mihajlo Pupin“<br />
                Zrenjanin<br />
                Šk. 2025/26<br /><br />
                <b>Naziv predmeta:</b> Razvoj višeslojnog softvera<br />
                <b>Prezime:</b> Drašković<br />
                <b>Ime:</b> Bogdan<br />
                <b>Broj indeksa:</b> 52/22<br />
                <b>Naziv poslovnog procesa:</b> Veb aplikacija za prijavu takmičara na takmičenje iz kik boksa<br />
                <b>Naziv dokumenta:</b> Prijava takmičara na prvenstvo<br />
                <b>Poslovno pravilo:</b> AKO takmičar ima određeni broj godina ONDA mora biti u određenoj starosnoj kategoriji (Senior, Junior, Kadet)
            </div>

            <div class="naslov">Prijava Takmičara na Prvenstvo</div>
            <div class="linija"></div>

            <table class="tabela-info">
                <tr>
                    <td><b>Prvenstvo:</b> <asp:Label ID="lblPrvenstvo" runat="server"></asp:Label></td>
                    <td><b>Disciplina:</b> <asp:Label ID="lblDisciplina" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td><b>Mesto:</b> <asp:Label ID="lblMesto" runat="server"></asp:Label></td>
                    <td><b>Datum:</b> <asp:Label ID="lblDatum" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2"><br /><b>Naziv kluba:</b></td>
                </tr>
                <tr>
                    <td colspan="2" class="okvir-polje"><asp:Label ID="lblKlub" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2"><br /><b>Ime i prezime trenera:</b></td>
                </tr>
                <tr>
                    <td colspan="2" class="okvir-polje"><asp:Label ID="lblTrener" runat="server"></asp:Label></td>
                </tr>
            </table>

            <div class="naslov" style="font-size: 18px; text-align: left; border-bottom: 1px solid #ccc; padding-bottom: 5px;">Prijavljeni Takmičari</div>

            <!-- Tabela prijavljenih takmičara sa opcijama za izmenu i brisanje -->
           <!-- Tabela prijavljenih takmičara sa ispravljenim sintaksnim greškama -->
<asp:GridView ID="gvStavke" runat="server" AutoGenerateColumns="False" CssClass="grid-table"
    DataKeyNames="StavkaID"
    OnRowEditing="gvStavke_RowEditing" 
    OnRowCancelingEdit="gvStavke_RowCancelingEdit" 
    OnRowUpdating="gvStavke_RowUpdating" 
    OnRowDeleting="gvStavke_RowDeleting"
    OnRowDataBound="gvStavke_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderText="Ime i prezime">
            <ItemTemplate>
                <%# Eval("ImePrezime") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditImePrezime" runat="server" Text='<%# Eval("ImePrezime") %>' CssClass="form-control"></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Datum rođenja">
            <ItemTemplate>
                <%# Convert.ToDateTime(Eval("DatumRodjenja")).ToString("dd.MM.yyyy.") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditDatumRodjenja" runat="server" Text='<%# Eval("DatumRodjenja") %>' CssClass="form-control datepicker"></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Starosna kategorija">
            <ItemTemplate>
                <%# Eval("NazivKategorije") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:DropDownList ID="ddlEditKategorija" runat="server" CssClass="form-control"></asp:DropDownList>
                <asp:HiddenField ID="hfSelectedKatID" runat="server" Value='<%# Eval("KategorijaID") %>' />
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Disciplina">
            <ItemTemplate>
                <%# Eval("Disciplina") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Težinska Kategorija">
            <ItemTemplate>
                <%# Eval("TezinskaKategorija") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditTezina" runat="server" Text='<%# Eval("TezinskaKategorija") %>' CssClass="form-control"></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" 
                          EditText="Izmeni" UpdateText="Sačuvaj" CancelText="Otkaz" DeleteText="Obriši" 
                          HeaderStyle-CssClass="no-print" ItemStyle-CssClass="no-print" ControlStyle-CssClass="btn btn-sm" />
    </Columns>
</asp:GridView>
            
            <asp:Label ID="lblNemaPodataka" runat="server" Text="Nema takmičara koji odgovaraju izabranim filterima." Visible="false" Style="color: red; display: block; margin-top: 10px;"></asp:Label>
        </div>
    </form>
</body>
</html>