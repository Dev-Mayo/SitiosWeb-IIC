<%@ Page Language="C#" AutoEventWireup="true" Async="true" MasterPageFile="~/Site.Master" CodeBehind="AgendarEntrevista.aspx.cs" Inherits="ADMExpedientePersonal.OFE.AgendarEntrevista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card-custom {
            width: calc(100% - 80px);
            max-width: none;
            margin: 40px;
            border-radius: 14px;
            box-shadow: 0 6px 28px rgba(0,0,0,.15);
        }
        .card-header-custom {
            background: #1aad94;
            padding: 20px;
            text-align: center;
            color: white;
        }
        .table {
            width: 100%;
        }
        .perm-toggle { min-width: 110px; }
        .grid-pager { text-align: center; padding-top: 10px; padding-bottom: 10px;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card-custom bg-white">
        <div class="card-header-custom">
            <h5 class="fw-bold mb-0">Administración de Oferentes</h5>
        </div>

        <div class="p-4">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>
                    <asp:Button ID="btnNuevo" runat="server" Text="Nueva Entrevista" CssClass="btn btn-primary mb-3 fw-semibold"
                        OnClick="btnNuevo_Click" />

                    <asp:HiddenField ID="hfEntrevistaId" runat="server" />

                    <asp:GridView ID="gvEntrevistas" runat="server" CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False" DataKeyNames="EntrevistaId" OnRowCommand="gvEntrevistas_RowCommand"
                        AllowPaging="true" PageSize="10" OnPageIndexChanging="gvEntrevistas_PageIndexChanging" PagerStyle-CssClass="grid-pager">
                        <Columns>
                            <asp:BoundField DataField="EntrevistaId" HeaderText="Entrevista" />
                            <asp:BoundField DataField="OferenteIdentificacion" HeaderText="Oferente" />
                            <asp:BoundField DataField="EmpleadoId" HeaderText="Empleado" />
                            <asp:BoundField DataField="FechaEntrevista" HeaderText="Fecha Entrevista" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Editar"
                                        CommandArgument='<%# Eval("EntrevistaId") %>' CssClass="btn btn-sm btn-warning me-2">
                                        <i class="bi bi-pencil"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkEliminar" runat="server" CommandName="Eliminar"
                                        CommandArgument='<%# Eval("EntrevistaId") %>' CssClass="btn btn-sm btn-danger">
                                        <i class="bi bi-trash"></i> Eliminar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkCambiarEstado" runat="server" CommandName="CambiarEstado"
                                        CommandArgument='<%# Eval("EntrevistaId") %>' CssClass="btn btn-sm btn-success">
                                        <i class="bi bi-check"></i> Realizado
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <!-- Modal Confirmación Eliminar -->
    <div id="modalEliminar" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">Confirmar eliminación</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p>¿Está seguro que desea eliminar la entrevista?</p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmarEliminar" runat="server" Text="Eliminar" CssClass="btn btn-danger"
                        OnClick="btnConfirmarEliminar_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

        <!-- Modal para Agregar / Editar Entrevistas -->
    <div id="modalEntrevista" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title" id="modalTitle">Formulario Entrevista</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModalEntrevista" runat="server">
                        <ContentTemplate>

                            <asp:HiddenField ID="hfAccion" runat="server" />

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Entrevista</label>
                                <asp:TextBox ID="txtEntrevistaId" runat="server" CssClass="form-control" Enabled="false" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Oferente</label>
                                <asp:DropDownList ID="ddlOferentes" runat="server" CssClass="form-select"
                                    DataTextField="nombre_completo" DataValueField="identificacion" />
                            </div>
                            

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Empleado</label>
                                <asp:DropDownList ID="ddlEmpleados" runat="server" CssClass="form-select"
                                    DataTextField="NombreEmpleado" DataValueField="EmpleadoId" />
                            </div>
                            

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Fecha de entrevista</label>
                                <asp:TextBox ID="txtFechaEntrevista" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>

                            <asp:Label ID="lblMensajeError" runat="server" CssClass="form-label fw-semibold text-danger" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnGuardarEntrevista" runat="server" Text="Guardar" CssClass="btn btn-primary"
                        OnClick="btnGuardarEntrevista_Click" CausesValidation="true" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

        <!-- UpdatePanel exclusivo para el modal -->
<asp:UpdatePanel ID="upModalMensaje" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <!-- Modal Mensaje -->
        <div id="modalMensaje" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header bg-warning text-dark">
                        <h5 class="modal-title">Mensaje del sistema</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <asp:Literal ID="litMensajeModal" runat="server" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>




    <!-- Script para abrir/cerrar modal desde codigo -->
    <script type="text/javascript">
        function showModal() {
            var modalEl = document.getElementById('<%= modalEntrevista.ClientID %>');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }
        function hideModal() {
            var modalEl = document.getElementById('<%= modalEntrevista.ClientID %>');
                var modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) modal.hide();
        }

        function showEliminarModal() {
            var modalEl = document.getElementById('<%= modalEliminar.ClientID %>');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }
        function hideEliminarModal() {
            var modalEl = document.getElementById('<%= modalEliminar.ClientID %>');
            var modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) modal.hide();
        }

        function showMensajeModal() {
            var modalEl = document.getElementById('<%= modalMensaje.ClientID %>');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }
        function hideMensajeModal() {
                    var modalEl = document.getElementById('<%= modalMensaje.ClientID %>');
                    var modal = bootstrap.Modal.getInstance(modalEl);
                    if (modal) modal.hide();
        }


    </script>
</asp:Content>