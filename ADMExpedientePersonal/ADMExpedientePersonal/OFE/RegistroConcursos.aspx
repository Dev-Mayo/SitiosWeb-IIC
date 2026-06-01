<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="RegistroConcursos.aspx.cs"
    Inherits="ADMExpedientePersonal.OFE.RegistroConcursos" %>

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
            <h5 class="fw-bold mb-0">Administración de Concursos</h5>
        </div>

        <div class="p-4">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />

            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>

                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo"
                        CssClass="btn btn-primary mb-3 fw-semibold"
                        OnClick="btnNuevo_Click" />

                    <asp:HiddenField ID="hfCodigoConcurso" runat="server" />

                    <asp:GridView ID="gvConcursos" runat="server"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
                        DataKeyNames="codigo_concurso"
                        AllowPaging="true"
                        PageSize="10"
                        PagerStyle-CssClass="grid-pager"
                        OnPageIndexChanging="gvConcursos_PageIndexChanging"
                        OnRowCommand="gvConcursos_RowCommand">

                        <Columns>
                            <asp:BoundField DataField="codigo_concurso" HeaderText="Código" />
                            <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="fecha_inicio" HeaderText="Fecha Inicio" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="fecha_fin" HeaderText="Fecha Fin" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="estado" HeaderText="Estado" />

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEditar" runat="server"
                                        CommandName="Editar"
                                        CommandArgument='<%# Eval("codigo_concurso") %>'
                                        CssClass="btn btn-sm btn-warning me-2">
                                        Editar
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="lnkEstado" runat="server"
                                        CommandName="Estado"
                                        CommandArgument='<%# Eval("codigo_concurso") %>'
                                        CssClass="btn btn-sm btn-info me-2">
                                        Cambiar Estado
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="lnkEliminar" runat="server"
                                        CommandName="Eliminar"
                                        CommandArgument='<%# Eval("codigo_concurso") %>'
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
    <div id="modalConcurso" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title">Formulario Concurso</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModalConcurso" runat="server">
                        <ContentTemplate>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Código</label>
                                <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control" MaxLength="50" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Nombre</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="150" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Fecha Inicio</label>
                                <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Fecha Fin</label>
                                <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>

                            <asp:Label ID="lblMensajeError" runat="server" CssClass="form-label fw-semibold text-danger" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnGuardarConcurso" runat="server"
                        Text="Guardar"
                        CssClass="btn btn-primary"
                        OnClick="btnGuardarConcurso_Click" />

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
        function showConcursoModal() {
            var modalEl = document.getElementById('<%= modalConcurso.ClientID %>');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }

        function hideConcursoModal() {
            var modalEl = document.getElementById('<%= modalConcurso.ClientID %>');
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