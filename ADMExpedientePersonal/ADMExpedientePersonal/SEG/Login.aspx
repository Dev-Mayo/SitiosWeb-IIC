<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="Login.aspx.cs" Inherits="ADMExpedientePersonal.SEG.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>Login Administracion de Personal</title>
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css"
          rel="stylesheet" />
<style>
        body { background: #eef2f7; }
        .card-login {
            max-width: 420px;
            margin: 80px auto;
            border-radius: 14px;
            overflow: hidden;
            box-shadow: 0 6px 28px rgba(0,0,0,.15);
        }
        .card-header-custom {
            background: #1aad94;
            padding: 36px 24px;
            text-align: center;
            color: white;
        }
        .logo-emoji {
          font-size: 64px;
          text-align: center;
        }
</style>
</head>
<body>
<form id="form1" runat="server">
<div class="card-login bg-white">
<div class="card-header-custom">
<div class="logo-emoji">🏢</div>
<h5 class="mt-2 mb-0 fw-bold">Administración de Personal</h5>
<small class="opacity-75">Portal Administrativo</small>
</div>
<div class="p-4">
<asp:Panel ID="pnlMensaje" runat="server" Visible="false">
<div id="divMensaje" runat="server" class="alert mb-3"></div>
</asp:Panel>
<asp:Panel ID="pnlBloqueado" runat="server" Visible="false">
<div class="alert alert-danger">
<strong>Usuario bloqueado.</strong>
                Se superaron 3 intentos fallidos.
</div>
</asp:Panel>
<div class="mb-3">
<label class="form-label fw-semibold">ID de usuario</label>
<asp:TextBox ID="txtUsuario" runat="server"
                         CssClass="form-control"
                         placeholder="Ingrese su ID de usuario" />
<asp:RequiredFieldValidator runat="server"
                ControlToValidate="txtUsuario"
                ErrorMessage="El ID de usuario es requerido."
                CssClass="text-danger small" Display="Dynamic" />
</div>
<div class="mb-4">
<label class="form-label fw-semibold">Contraseña</label>
<asp:TextBox ID="txtPassword" runat="server"
                         TextMode="Password"
                         CssClass="form-control"
                         placeholder="Ingrese su contraseña" />
<asp:RequiredFieldValidator runat="server"
                ControlToValidate="txtPassword"
                ErrorMessage="La contraseña es requerida."
                CssClass="text-danger small" Display="Dynamic" />
</div>
<asp:Button ID="btnIngresar" runat="server"
                    Text="Ingresar"
                    CssClass="btn btn-primary w-100 py-2 fw-semibold"
                    OnClick="btnIngresar_Click" />
</div>
</div>
</form>
</body>
</html>
