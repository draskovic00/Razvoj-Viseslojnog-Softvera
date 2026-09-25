<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnosPrijave.aspx.cs" Inherits="PrijavaTakmicaraNaTakmicenjeIzIza.UnosPrijave" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <link href="css/Stilovi.css" rel="stylesheet" type="text/css" />
    <!-- Flatpickr CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/themes/material_orange.css">

    <title>Prijava takmičara</title>
    <script type="text/javascript">
/* global flatpickr */

function validirajFormu() {
    /** @type {HTMLInputElement} */
    var elNaziv = /** @type {HTMLInputElement} */ (document.getElementById('<%= txtNazivPrvenstva.ClientID %>'));
            
            /** @type {HTMLInputElement} */
            var elMesto = /** @type {HTMLInputElement} */ (document.getElementById('<%= txtMesto.ClientID %>'));

            if (!elNaziv || !elMesto) return false;

            var nazivPrvenstva = elNaziv.value;
            var mesto = elMesto.value;

            var regexTekst = /^[a-zA-Z0-9 šščćžŠŠČĆŽ]{2,100}$/;

            if (!regexTekst.test(nazivPrvenstva)) {
                alert("Naziv prvenstva mora imati između 2 i 100 karaktera!");
                return false;
            }
            if (!regexTekst.test(mesto)) {
                alert("Mesto takmičenja je obavezno!");
                return false;
            }
            return true;
        }

        document.addEventListener("DOMContentLoaded", function () {
            if (typeof flatpickr !== 'undefined') {
                flatpickr(".datepicker", {
                    dateFormat: "Y-m-d",
                    maxDate: "today",
                    disableMobile: true
                });
            }
        });
    </script>
    <style>
        .welcome-header {
            background-color: #2c3e50;
            color: #ffffff;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .welcome-header h3 {
            color: #ffffff;
            margin: 0;
            font-size: 1.1rem;
            text-transform: none;
            letter-spacing: normal;
        }
        .welcome-header span {
            color: #e67e22;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" onsubmit="return validirajFormu();">
        <div class="container">
            <div class="welcome-header">
                <h3>Dobrodošli, <asp:Label ID="lblImeTrenera" runat="server"></asp:Label>!</h3>
                <small style="opacity: 0.8;">Sistem za prijavu takmičara</small>
            </div>

            <h2>Prijava takmičara za prvenstvo</h2>
            <asp:Label ID="lblStatus" runat="server" CssClass="status-label"></asp:Label>

            <!-- Master deo: Podaci o prvenstvu -->
            <div class="form-section">
                <h3>Podaci o prvenstvu</h3>
                <div class="form-grid">
                    <div class="form-group">
                        <label>Naziv prvenstva:</label>
                        <asp:TextBox ID="txtNazivPrvenstva" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Disciplina:</label>
                        <asp:DropDownList ID="ddlDisciplina" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Low Kick" Value="Low Kick"></asp:ListItem>
                            <asp:ListItem Text="K1" Value="K1"></asp:ListItem>
                            <asp:ListItem Text="Point Fighting" Value="Point Fighting"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Mesto:</label>
                        <asp:TextBox ID="txtMesto" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Detail deo: Unos takmičara -->
            <div class="form-section">
                <h3>Unos takmičara (Detail)</h3>
                <div class="form-grid">
                    <div class="form-group">
                        <label>Ime i prezime:</label>
                        <asp:TextBox ID="txtImePrezime" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Datum rođenja:</label>
                        <asp:TextBox ID="txtDatumRodjenja" runat="server" CssClass="form-control datepicker" placeholder="Izaberite datum"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Starosna kategorija:</label>
                        <asp:DropDownList ID="ddlKategorija" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Težinska kategorija:</label>
                        <asp:TextBox ID="txtTezina" runat="server" CssClass="form-control" placeholder="npr. -71kg"></asp:TextBox>
                    </div>
                </div>
                <asp:Button ID="btnDodajStavku" runat="server" Text="Dodaj takmičara u listu" CssClass="btn btn-primary" OnClick="btnDodajStavku_Click" />
            </div>

            <!-- Tabela sa CRUD funkcionalnostima -->
            <asp:GridView ID="gvStavke" runat="server" AutoGenerateColumns="False" CssClass="grid-table"
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
                            <asp:TextBox ID="txtEditImePrezime" runat="server" Text='<%# Bind("ImePrezime") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Datum rođenja">
                        <ItemTemplate>
                            <%# Eval("DatumRodjenja") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditDatumRodjenja" runat="server" Text='<%# Bind("DatumRodjenja") %>' CssClass="form-control datepicker"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Starosna kategorija">
                        <ItemTemplate>
                            <%# Eval("StarosnaKategorija") %>
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

                    <asp:TemplateField HeaderText="Težinska kategorija">
                        <ItemTemplate>
                            <%# Eval("TezinskaKategorija") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditTezina" runat="server" Text='<%# Bind("TezinskaKategorija") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" 
                                      EditText="Izmeni" UpdateText="Sačuvaj" CancelText="Otkaz" DeleteText="Obriši" 
                                      ControlStyle-CssClass="btn btn-sm" />
                </Columns>
            </asp:GridView>

            <br />
            <asp:Button ID="btnSacuvajSve" runat="server" Text="Sačuvaj kompletnu prijavu" CssClass="btn btn-success" OnClick="btnSacuvajSve_Click" />
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
</body>
</html>