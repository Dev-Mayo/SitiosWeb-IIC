<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="AdminPuestos.aspx.cs"
    Inherits="ADMExpedientePersonal.EMP.AdminPuestos" %>

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

        .grid-pager {
            text-align: center;
            padding-top: 10px;
            padding-bottom: 10px;
        }

        .table {
            width: 100%;
        }

        .modal-lg {
            max-width: 800px;
        }

        .modal-body {
            padding: 30px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card-custom bg-white">
        <div class="card-header-custom">
            <h5 class="fw-bold mb-0">Administración de Puestos</h5>
        </div>

        <div class="p-4">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />

            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>

                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo"
                        CssClass="btn btn-primary mb-3 fw-semibold"
                        OnClick="btnNuevo_Click" />

                    <asp:HiddenField ID="hfPuestoId" runat="server" />

                    <asp:GridView ID="gvPuestos" runat="server"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
                        DataKeyNames="puesto_id"
                        AllowPaging="true"
                        PageSize="10"
                        PagerStyle-CssClass="grid-pager"
                        OnPageIndexChanging="gvPuestos_PageIndexChanging"
                        OnRowCommand="gvPuestos_RowCommand">

                        <Columns>
                            <asp:BoundField DataField="puesto_id" HeaderText="ID" />
                            <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="salario" HeaderText="Salario" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="jefe_puesto_id" HeaderText="Jefe Puesto ID" />

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEditar" runat="server"
                                        CommandName="Editar"
                                        CommandArgument='<%# Eval("puesto_id") %>'
                                        CssClass="btn btn-sm btn-warning me-2">
                                        Editar
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="lnkEliminar" runat="server"
                                        CommandName="Eliminar"
                                        CommandArgument='<%# Eval("puesto_id") %>'
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

    <!-- Modal Agregar / Editar -->
    <div id="modalPuesto" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title">Formulario Puesto</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModalPuesto" runat="server">
                        <ContentTemplate>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Nombre del puesto</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="100" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Salario</label>
                                <asp:TextBox ID="txtSalario" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">ID puesto jefatura</label>
                                <asp:TextBox ID="txtJefePuestoId" runat="server" CssClass="form-control" TextMode="Number" />
                                <small class="text-muted">Opcional</small>
                            </div>

                            <asp:Label ID="lblMensajeError" runat="server" CssClass="form-label fw-semibold text-danger" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnGuardarPuesto" runat="server"
                        Text="Guardar"
                        CssClass="btn btn-primary"
                        OnClick="btnGuardarPuesto_Click" />

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
        function showPuestoModal() {
            var modalEl = document.getElementById('<%= modalPuesto.ClientID %>');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }

        function hidePuestoModal() {
            var modalEl = document.getElementById('<%= modalPuesto.ClientID %>');
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
    </script>

</asp:Content>