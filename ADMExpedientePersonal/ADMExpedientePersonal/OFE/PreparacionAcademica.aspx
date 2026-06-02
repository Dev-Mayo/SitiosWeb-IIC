<%@ Page Language="C#" AutoEventWireup="true" Async="true" MasterPageFile="~/Site.Master" CodeBehind="PreparacionAcademica.aspx.cs" Inherits="ADMExpedientePersonal.OFE.PreparacionAcademica" %>

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
            <h5 class="fw-bold mb-0">Administración de preparación académica</h5>
        </div>

        <div class="p-4">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>
                    <asp:Button ID="btnNuevo" runat="server" Text="Nueva Preparación Académica" CssClass="btn btn-primary mb-3 fw-semibold"
                        OnClick="btnNuevo_Click" />

                    <asp:HiddenField ID="hfPreparacionAcadId" runat="server" />

                    <asp:GridView ID="gvPreparacionAcad" runat="server" CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False" DataKeyNames="Id" OnRowCommand="gvPreparacionAcad_RowCommand"
                        AllowPaging="true" PageSize="10" OnPageIndexChanging="gvPreparacionAcad_PageIndexChanging" PagerStyle-CssClass="grid-pager">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="Preparación Académica" />
                            <asp:BoundField DataField="Institucion" HeaderText="Institución" />
                            <asp:BoundField DataField="OferenteId" HeaderText="Oferente" />
                            <asp:BoundField DataField="Titulo" HeaderText="Titulo" />
                            <asp:BoundField DataField="FechaInicio" HeaderText="Fecha inicio" />
                            <asp:BoundField DataField="FechaFin" HeaderText="Fecha fin" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Editar"
                                        CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-warning me-2">
                                        <i class="bi bi-pencil"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkEliminar" runat="server" CommandName="Eliminar"
                                        CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-danger">
                                        <i class="bi bi-trash"></i> Eliminar
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
                    <p>¿Está seguro que desea eliminar la preparación académica?</p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmarEliminar" runat="server" Text="Eliminar" CssClass="btn btn-danger"
                        OnClick="btnConfirmarEliminar_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

        <!-- Modal para Agregar / Editar Preparacion academica -->
    <div id="modalPreparacionAcad" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title" id="modalTitle">Formulario Preparación Académica</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModalPreparacionAcad" runat="server">
                        <ContentTemplate>

                            <asp:HiddenField ID="hfAccion" runat="server" />

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Preparación Académica</label>
                                <asp:TextBox ID="txtPreparacionAcadId" runat="server" CssClass="form-control" Enabled="false" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Institución</label>
                                <asp:DropDownList ID="ddlInstituciones" runat="server" CssClass="form-select"
                                    DataTextField="nombre" DataValueField="codigo_institucion" />
                            </div>
                            

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Oferente</label>
                                <asp:TextBox ID="txtOferente" runat="server" CssClass="form-control" Enabled="false" />
                            </div>
                            

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Titulo</label>
                                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" />
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
                    <asp:Button ID="btnGuardarPreparacionAcad" runat="server" Text="Guardar" CssClass="btn btn-primary"
                        OnClick="btnGuardarPreparacionAcad_Click" CausesValidation="true" />
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
            var modalEl = document.getElementById('<%= modalPreparacionAcad.ClientID %>');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }
        function hideModal() {
            var modalEl = document.getElementById('<%= modalPreparacionAcad.ClientID %>');
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