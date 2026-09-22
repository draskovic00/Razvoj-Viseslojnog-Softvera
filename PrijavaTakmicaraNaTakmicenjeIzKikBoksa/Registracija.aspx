<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registracija.aspx.cs" Inherits="PrijavaTakmicaraNaTakmicenjeIzIza.Registracija" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <link href="css/Stilovi.css" rel="stylesheet" type="text/css" />
    <title>REGISTRACIJA TRENERA</title>
    <style>
        body {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            margin: 0;
            background-color: #f4f7f6;
        }
        .register-card {
            width: 100%;
            max-width: 450px;
            background: #fff;
            padding: 35px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }
        .register-card h2 {
            text-align: center;
            margin-bottom: 25px;
            border-bottom: 2px solid #e67e22;
            padding-bottom: 10px;
        }
        .btn-register {
            width: 100%;
            margin-top: 20px;
        }
        .login-link {
            display: block;
            text-align: center;
            margin-top: 15px;
            color: #2c3e50;
            text-decoration: none;
            font-size: 0.9rem;
        }
        .login-link:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="register-card">
            <h2>REGISTRACIJA TRENERA</h2>
            
            <asp:Label ID="lblStatus" runat="server" CssClass="status-label"></asp:Label>
            
            <div class="form-group" style="margin-top: 15px;">
                <label>Ime i prezime trenera:</label>
                <asp:TextBox ID="txtImePrezime" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="form-group" style="margin-top: 15px;">
                <label>Naziv kik boks kluba:</label>
                <asp:TextBox ID="txtKlub" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            
            <div class="form-group" style="margin-top: 15px;">
                <label>Korisničko ime:</label>
                <asp:TextBox ID="txtKorisnickoIme" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            
            <div class="form-group" style="margin-top: 15px;">
                <label>Lozinka:</label>
                <asp:TextBox ID="txtLozinka" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="form-group" style="margin-top: 15px;">
                <label>Potvrdi lozinku:</label>
                <asp:TextBox ID="txtPotvrdaLozinke" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            
            <asp:Button ID="btnRegistrujSe" runat="server" Text="REGISTROVANJE" CssClass="btn btn-primary btn-register" OnClick="btnRegistrujSe_Click" />
            
            <a href="Login.aspx" class="login-link">Već imate nalog? Prijavite se ovde.</a>
        </div>
    </form>
</body>
</html>