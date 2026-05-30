<%@ Page Title="Formulario Requisito de Puesto"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="RequisitoPuestoForm.aspx.cs"
    Inherits="ADMExpedientePersonal.EMP.RequisitoPuestoForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow-sm">

        <div class="card-header text-white" style="background-color:#1aad94;">
            <h4 class="mb-0">Formulario Requisito de Puesto</h4>
        </div>

        <div class="card-body">

            <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger d-block mb-3"></asp:Label>

            <div class="mb-3">
                <label class="form-label">Nombre del requisito</label>
                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <asp:Button ID="btnGuardar"
                runat="server"
                Text="Guardar"
                CssClass="btn btn-success"
                OnClick="btnGuardar_Click" />

            <asp:Button ID="btnCancelar"
                runat="server"
                Text="Cancelar"
                CssClass="btn btn-secondary ms-2"
                OnClick="btnCancelar_Click" />

        </div>

    </div>

</asp:Content>