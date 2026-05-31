<%@ Page Title="Acción de Personal" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccionPersonalForm.aspx.cs" Inherits="ADMExpedientePersonal.EMP.AccionPersonalForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Formulario de Acción de Personal</h2>

    <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>

    <br /><br />

    <div class="form-group">
        <label>Código de la acción</label>
        <asp:TextBox ID="txtCodigoAccion" runat="server" CssClass="form-control"></asp:TextBox>
    </div>

    <div class="form-group">
        <label>Fecha de la acción</label>
        <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
    </div>

    <div class="form-group">
        <label>Descripción</label>
        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="500" Rows="5"></asp:TextBox>
    </div>

    <div class="form-group">
        <label>Empleado</label>
        <asp:DropDownList ID="ddlEmpleado" runat="server" CssClass="form-control">
        </asp:DropDownList>
    </div>

    <div class="form-group">
        <label>Jefatura que aprueba</label>
        <asp:DropDownList ID="ddlJefatura" runat="server" CssClass="form-control">
        </asp:DropDownList>
    </div>

    <br />

    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="btnGuardar_Click" />
    <asp:Button ID="btnRegresar" runat="server" Text="Regresar" CssClass="btn btn-secondary" OnClick="btnRegresar_Click" />

</asp:Content>