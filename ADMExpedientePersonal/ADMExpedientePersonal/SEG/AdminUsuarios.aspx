<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="AdminUsuarios.aspx.cs"
    Inherits="ADMExpedientePersonal.SEG.AdminUsuarios" %>

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
        .table { width: 100%; }
        .modal-lg { max-width: 800px; }
        .modal-body { padding: 30px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card-custom bg-white">
        <div class="card-header-custom">
            <h5 class="fw-bold mb-0">Administración de Usuarios</h5>
        </div>

        <div class="p-4">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />

            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>

                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo"
                        CssClass="btn btn-primary mb-3 fw-semibold"
                        OnClick="btnNuevo_Click" />

                    <asp:HiddenField ID="hfIdUsuario" runat="server" />
                    <asp:HiddenField ID="hfAccion" runat="server" />

                    <asp:GridView ID="gvUsuarios" runat="server"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
                        DataKeyNames="id_usuario"
                        AllowPaging="true"
                        PageSize="10"
                        PagerStyle-CssClass="grid-pager"
                        OnPageIndexChanging="gvUsuarios_PageIndexChanging"
                        OnRowCommand="gvUsuarios_RowCommand">

                        <Columns>
                            <asp:BoundField DataField="id_usuario"       HeaderText="ID" />
                            <asp:BoundField DataField="nombreusuario"     HeaderText="Usuario" />
                            <asp:BoundField DataField="nombre_completo"   HeaderText="Nombre Completo" />
                            <asp:BoundField DataField="correo"            HeaderText="Correo" />
                            <asp:BoundField DataField="roles"             HeaderText="Roles" />
                            <asp:BoundField DataField="estado"            HeaderText="Estado" />

                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>

                                    <asp:LinkButton runat="server"
                                        CommandName="Editar"
                                        CommandArgument='<%# Eval("id_usuario") %>'
                                        CssClass="btn btn-sm btn-warning me-1">
                                        Editar
                                    </asp:LinkButton>

                                    <%-- Toggle Activo/Inactivo --%>
                                    <asp:LinkButton runat="server"
                                        CommandName="Inactivar"
                                        CommandArgument='<%# Eval("id_usuario") %>'
                                        CssClass="btn btn-sm btn-secondary me-1"
                                        Visible='<%# Eval("estado").ToString() == "Activo" %>'>
                                        Inactivar
                                    </asp:LinkButton>

                                    <asp:LinkButton runat="server"
                                        CommandName="Activar"
                                        CommandArgument='<%# Eval("id_usuario") %>'
                                        CssClass="btn btn-sm btn-success me-1"
                                        Visible='<%# Eval("estado").ToString() != "Activo" %>'>
                                        Activar
                                    </asp:LinkButton>

                                    <asp:LinkButton runat="server"
                                        CommandName="Eliminar"
                                        CommandArgument='<%# Eval("id_usuario") %>'
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

    <!-- Modal Nuevo Usuario -->
    <div id="modalNuevo" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title">Nuevo Usuario</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModalNuevo" runat="server">
                        <ContentTemplate>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Nombre de usuario</label>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" MaxLength="50" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Nombre completo</label>
                                <asp:TextBox ID="txtNombreCompleto" runat="server" CssClass="form-control" MaxLength="100" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Correo</label>
                                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" MaxLength="100" TextMode="Email" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Contraseña</label>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                                <small class="text-muted">Mínimo 8 caracteres, mayúsculas, minúsculas, números y caracteres especiales.</small>
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Confirmar contraseña</label>
                                <asp:TextBox ID="txtConfirmarPassword" runat="server" CssClass="form-control" TextMode="Password" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Roles</label>
                                <asp:CheckBoxList ID="cblRoles" runat="server" CssClass="form-check" />
                            </div>

                            <asp:Label ID="lblMensajeError" runat="server" CssClass="text-danger small" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnGuardarUsuario" runat="server"
                        Text="Guardar"
                        CssClass="btn btn-primary"
                        OnClick="btnGuardarUsuario_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                </div>

            </div>
        </div>
    </div>

    <!-- Modal Editar Usuario -->
    <div id="modalEditar" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title">Editar Usuario</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModalEditar" runat="server">
                        <ContentTemplate>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Nombre de usuario</label>
                                <asp:TextBox ID="txtUsernameEditar" runat="server" CssClass="form-control" MaxLength="50" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Nombre completo</label>
                                <asp:TextBox ID="txtNombreCompletoEditar" runat="server" CssClass="form-control" MaxLength="100" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Correo</label>
                                <asp:TextBox ID="txtCorreoEditar" runat="server" CssClass="form-control" MaxLength="100" TextMode="Email" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Contraseña <small class="text-muted">(dejar vacío para no cambiar)</small></label>
                                <asp:TextBox ID="txtPasswordEditar" runat="server" CssClass="form-control" TextMode="Password" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Confirmar contraseña</label>
                                <asp:TextBox ID="txtConfirmarPasswordEditar" runat="server" CssClass="form-control" TextMode="Password" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Estado</label>
                                <asp:DropDownList ID="ddlEstadoEditar" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="Activo">Activo</asp:ListItem>
                                    <asp:ListItem Value="Inactivo">Inactivo</asp:ListItem>
                                    <asp:ListItem Value="Bloqueado">Bloqueado</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Roles</label>
                                <asp:CheckBoxList ID="cblRolesEditar" runat="server" CssClass="form-check" />
                            </div>

                            <asp:Label ID="lblMensajeErrorEditar" runat="server" CssClass="text-danger small" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnGuardarEditar" runat="server"
                        Text="Guardar"
                        CssClass="btn btn-primary"
                        OnClick="btnGuardarEditar_Click" />
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
            var modalEl = document.getElementById('<%= modalNuevo.ClientID %>');
            bootstrap.Modal.getOrCreateInstance(modalEl).show();
        }
        function hideModal() {
            var modalEl = document.getElementById('<%= modalNuevo.ClientID %>');
            var m = bootstrap.Modal.getInstance(modalEl);
            if (m) m.hide();
        }
        function showModalEditar() {
            var modalEl = document.getElementById('<%= modalEditar.ClientID %>');
            bootstrap.Modal.getOrCreateInstance(modalEl).show();
        }
        function hideModalEditar() {
            var modalEl = document.getElementById('<%= modalEditar.ClientID %>');
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