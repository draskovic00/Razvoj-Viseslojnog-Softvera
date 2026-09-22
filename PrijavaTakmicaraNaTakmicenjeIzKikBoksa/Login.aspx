<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PrijavaTakmicaraNaTakmicenjeIzIza.Login" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <link href="css/Stilovi.css" rel="stylesheet" type="text/css" />
    <title>PRIJAVA NA SISTEM</title>
    <style>
        body {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            margin: 0;
            background-color: #f4f7f6;
        }
        .login-card {
            width: 100%;
            max-width: 400px;
            background: #fff;
            padding: 35px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }
        .login-card h2 {
            text-align: center;
            margin-bottom: 25px;
            border-bottom: 2px solid #e67e22;
            padding-bottom: 10px;
        }
        .btn-login {
            width: 100%;
            margin-top: 10px;
        }
        .btn-register-link {
         display: block;
  text-align: center;
  margin-top: 15px;
  color: #2c3e50;
  text-decoration: none;
  font-size: 0.9rem;
        }
        .btn-register-link:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            <h2>PRIJAVA TRENERA</h2>
            
            <asp:Label ID="lblGreska" runat="server" CssClass="status-label" ForeColor="Red"></asp:Label>
            
            <div class="form-group" style="margin-top: 15px;">
                <label>Korisničko ime:</label>
                <asp:TextBox ID="txtKorisnik" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            
            <div class="form-group" style="margin-top: 15px; margin-bottom: 20px;">
                <label>Lozinka:</label>
                <asp:TextBox ID="txtLozinka" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            
            <asp:Button ID="btnLogin" runat="server" Text="PRIJAVI SE" CssClass="btn btn-primary btn-login" OnClick="btnLogin_Click" />
            
            <a href="Registracija.aspx" class="btn-register-link">Nemate nalog? Registrujte se</a>
        </div>
    </form>
</body>
</html>