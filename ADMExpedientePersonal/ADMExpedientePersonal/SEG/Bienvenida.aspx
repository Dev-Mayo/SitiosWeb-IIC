<%@ Page Title="Bienvenida" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Bienvenida.aspx.cs" Inherits="ADMExpedientePersonal.SEG.Bienvenida" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .welcome-card {
            width: calc(100% - 100px);
            max-width: none;
            margin: 55px 50px;
            background: #fff;
            border-radius: 6px;
            box-shadow: 0 6px 22px rgba(0,0,0,.18);
            padding: 35px 45px;
            text-align: center;
        }

        .welcome-title {
            font-size: 32px;
            font-weight: 700;
            color: #1f2d3d;
            margin-bottom: 25px;
        }

        .welcome-title span {
            color: #1aad94;
        }

        
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="welcome-card">
        <h2 class="welcome-title">
            Bienvenido,
            <span><asp:Label ID="lblNombreUsuario" runat="server"></asp:Label></span>
        </h2>

        <img src="../Content/img/logo.png" class="welcome-logo" />
    </div>

</asp:Content>