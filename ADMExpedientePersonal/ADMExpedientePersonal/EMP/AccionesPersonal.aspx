<%@ Page Title="Acciones de Personal" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccionesPersonal.aspx.cs" Inherits="ADMExpedientePersonal.EMP.AccionesPersonal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Administración de Acciones de Personal</h2>

    <asp:Label ID="lblMensaje" runat="server" CssClass="text-success"></asp:Label>
    <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>

    <br /><br />

    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary" OnClick="btnNuevo_Click" />

    <br /><br />

    <asp:GridView ID="gvAccionesPersonal" runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-bordered table-striped"
        AllowPaging="true"
        PageSize="10"
        OnPageIndexChanging="gvAccionesPersonal_PageIndexChanging"
        OnRowCommand="gvAccionesPersonal_RowCommand">

        <Columns>
            <asp:BoundField DataField="codigo_accion" HeaderText="Código" />
            <asp:BoundField DataField="fecha" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
            <asp:BoundField DataField="nombre_empleado" HeaderText="Empleado" />
            <asp:BoundField DataField="nombre_jefatura" HeaderText="Jefatura aprueba" />

            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <asp:Button ID="btnEditar" runat="server"
                        Text="Editar"
                        CssClass="btn btn-warning btn-sm"
                        CommandName="EditarAccion"
                        CommandArgument='<%# Eval("accion_id") %>' />

                    <asp:Button ID="btnEliminar" runat="server"
                        Text="Eliminar"
                        CssClass="btn btn-danger btn-sm"
                        CommandName="EliminarAccion"
                        CommandArgument='<%# Eval("accion_id") %>'
                        OnClientClick="return confirm('¿Realmente desea eliminar el elemento seleccionado?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>

    </asp:GridView>

</asp:Content>