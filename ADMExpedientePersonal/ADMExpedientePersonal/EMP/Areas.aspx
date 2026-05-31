<%@ Page Title="Áreas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Areas.aspx.cs" Inherits="ADMExpedientePersonal.EMP.Areas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Administración de Áreas</h2>

    <asp:Label ID="lblMensaje" runat="server" CssClass="text-success"></asp:Label>
    <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>

    <br /><br />

    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary" OnClick="btnNuevo_Click" />

    <br /><br />

    <asp:GridView ID="gvAreas" runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-bordered table-striped"
        AllowPaging="true"
        PageSize="10"
        OnPageIndexChanging="gvAreas_PageIndexChanging"
        OnRowCommand="gvAreas_RowCommand">

        <Columns>
            <asp:BoundField DataField="codigo_area" HeaderText="Código" />
            <asp:BoundField DataField="nombre" HeaderText="Nombre del área" />
            <asp:BoundField DataField="nombre_jefatura" HeaderText="Jefatura" />
            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <asp:Button ID="btnEditar" runat="server"
                        Text="Editar"
                        CssClass="btn btn-warning btn-sm"
                        CommandName="EditarArea"
                        CommandArgument='<%# Eval("codigo_area") %>' />

                    <asp:Button ID="btnEliminar" runat="server"
                        Text="Eliminar"
                        CssClass="btn btn-danger btn-sm"
                        CommandName="EliminarArea"
                        CommandArgument='<%# Eval("codigo_area") %>'
                        OnClientClick="return confirm('¿Realmente desea eliminar el elemento seleccionado?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>

    </asp:GridView>

</asp:Content>