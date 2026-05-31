<%@ Page Title="Área" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AreaForm.aspx.cs" Inherits="ADMExpedientePersonal.EMP.AreaForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Formulario de Área</h2>

    <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>

    <br /><br />

    <div class="form-group">
        <label>Código del área</label>
        <asp:TextBox ID="txtCodigoArea" runat="server" CssClass="form-control"></asp:TextBox>
    </div>

    <div class="form-group">
        <label>Nombre del área</label>
        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
    </div>

   <div class="form-group">
    <label>Jefatura</label>
    <asp:DropDownList ID="ddlJefatura" runat="server" CssClass="form-control">
    </asp:DropDownList>
</div>

    <br />

    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="btnGuardar_Click" />
    <asp:Button ID="btnRegresar" runat="server" Text="Regresar" CssClass="btn btn-secondary" OnClick="btnRegresar_Click" />

</asp:Content>