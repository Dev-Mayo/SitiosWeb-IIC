<%@ Page Title="Administración de Requisitos de Puestos"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="RequisitosPuestos.aspx.cs"
    Inherits="ADMExpedientePersonal.EMP.RequisitosPuestos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow-sm">

        <div class="card-header text-white"
             style="background-color:#1aad94;">
            <h4 class="mb-0">
                Administración de Requisitos de Puestos
            </h4>
        </div>

        <div class="card-body">

           <asp:Button ID="btnNuevo"
    runat="server"
    Text="Nuevo"
    CssClass="btn btn-success mb-3"
    OnClick="btnNuevo_Click" />
            <asp:Label ID="lblMensaje"
                runat="server"
                CssClass="alert alert-success d-block mb-3"
                Visible="false"></asp:Label>

          <asp:GridView ID="gvRequisitos"
    runat="server"
    AutoGenerateColumns="false"
    CssClass="table table-striped table-bordered"
                AllowPaging="true"
    PageSize="10"
    OnRowCommand="gvRequisitos_RowCommand"
    OnPageIndexChanging="gvRequisitos_PageIndexChanging">

    <Columns>

        <asp:BoundField
            DataField="requisito_id"
            HeaderText="ID" />

        <asp:BoundField
            DataField="nombre"
            HeaderText="Nombre" />

        <asp:TemplateField HeaderText="Acciones">
            <ItemTemplate>

                <asp:LinkButton ID="btnEditar"
                    runat="server"
                    Text="Editar"
                    CssClass="btn btn-sm btn-primary"
                    CommandName="Editar"
                    CommandArgument='<%# Eval("requisito_id") %>' />

                <asp:LinkButton ID="btnEliminar"
                    runat="server"
                    Text="Eliminar"
                    CssClass="btn btn-sm btn-danger ms-2"
                    CommandName="Eliminar"
                    CommandArgument='<%# Eval("requisito_id") %>'
                    OnClientClick="return confirm('¿Realmente desea eliminar el elemento seleccionado?');" />

            </ItemTemplate>
        </asp:TemplateField>

    </Columns>

</asp:GridView>

        </div>

    </div>

</asp:Content>