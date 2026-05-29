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
                CssClass="btn btn-success mb-3" />

            <asp:GridView ID="gvRequisitos"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-striped table-bordered">

                <Columns>

                    <asp:BoundField
                        DataField="requisito_id"
                        HeaderText="ID" />

                    <asp:BoundField
                        DataField="nombre"
                        HeaderText="Nombre" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>