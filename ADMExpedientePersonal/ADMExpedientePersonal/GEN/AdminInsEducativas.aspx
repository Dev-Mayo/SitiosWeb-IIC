<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="AdminInsEducativas.aspx.cs"
    Inherits="ADMExpedientePersonal.GEN.AdminInsEducativas" %>

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
        .grid-pager { text-align: center; padding: 10px; }
        .table { width: 100%; }
        .modal-lg { max-width: 800px; }
        .modal-body { padding: 30px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card-custom bg-white">
        <div class="card-header-custom">
            <h5 class="fw-bold mb-0">Administración de Instituciones Educativas</h5>
        </div>

        <div class="p-4">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />

            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>

                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo"
                        CssClass="btn btn-primary mb-3 fw-semibold"
                        OnClick="btnNuevo_Click" />

                    <asp:HiddenField ID="hfCodigo" runat="server" />
                    <asp:HiddenField ID="hfAccion" runat="server" />

                    <asp:GridView ID="gvInstituciones" runat="server"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
                        DataKeyNames="codigo_institucion"
                        AllowPaging="true"
                        PageSize="10"
                        PagerStyle-CssClass="grid-pager"
                        OnPageIndexChanging="gvInstituciones_PageIndexChanging"
                        OnRowCommand="gvInstituciones_RowCommand">

                        <Columns>
                            <asp:BoundField DataField="codigo_institucion" HeaderText="Código" />
                            <asp:BoundField DataField="nombre"             HeaderText="Nombre" />

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server"
                                        CommandName="Editar"
                                        CommandArgument='<%# Eval("codigo_institucion") %>'
                                        CssClass="btn btn-sm btn-warning me-1">
                                        Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton runat="server"
                                        CommandName="Eliminar"
                                        CommandArgument='<%# Eval("codigo_institucion") %>'
                                        CssClass="btn btn-sm btn-danger">
                                        Eliminar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <!-- Modal Nuevo/Editar -->
    <div id="modalForm" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title">Formulario Institución Educativa</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModalForm" runat="server">
                        <ContentTemplate>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Código</label>
                                <asp:TextBox ID="txtCodigo" runat="server"
                                    CssClass="form-control" MaxLength="50" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Nombre</label>
                                <asp:TextBox ID="txtNombre" runat="server"
                                    CssClass="form-control" MaxLength="150" />
                                <small class="text-muted">Máximo 150 caracteres, solo letras.</small>
                            </div>

                            <asp:Label ID="lblMensajeError" runat="server"
                                CssClass="text-danger small" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnGuardar" runat="server"
                        Text="Guardar"
                        CssClass="btn btn-primary"
                        OnClick="btnGuardar_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                </div>

            </div>
        </div>
    </div>

    <!-- Modal Confirmar Eliminar -->
    <div id="modalEliminar" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">Confirmar eliminación</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p>¿Realmente desea eliminar el elemento seleccionado?</p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmarEliminar" runat="server"
                        Text="Eliminar"
                        CssClass="btn btn-danger"
                        OnClick="btnConfirmarEliminar_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Mensaje -->
    <div id="modalMensaje" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
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

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
    <script type="text/javascript">
        function showModal() {
            var modalEl = document.getElementById('<%= modalForm.ClientID %>');
            bootstrap.Modal.getOrCreateInstance(modalEl).show();
        }
        function hideModal() {
            var modalEl = document.getElementById('<%= modalForm.ClientID %>');
            var m = bootstrap.Modal.getInstance(modalEl);
            if (m) m.hide();
        }
        function showEliminarModal() {
            var modalEl = document.getElementById('<%= modalEliminar.ClientID %>');
            bootstrap.Modal.getOrCreateInstance(modalEl).show();
        }
        function hideEliminarModal() {
            var modalEl = document.getElementById('<%= modalEliminar.ClientID %>');
            var m = bootstrap.Modal.getInstance(modalEl);
            if (m) m.hide();
        }
        function showMensajeModal() {
            var modalEl = document.getElementById('<%= modalMensaje.ClientID %>');
            bootstrap.Modal.getOrCreateInstance(modalEl).show();
        }
    </script>

</asp:Content>