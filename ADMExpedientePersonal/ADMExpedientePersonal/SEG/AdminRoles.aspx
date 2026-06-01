<%@ Page Language="C#" AutoEventWireup="true" Async="true" MasterPageFile="~/Site.Master" CodeBehind="AdminRoles.aspx.cs" Inherits="ADMExpedientePersonal.SEG.AdminRoles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card-custom { max-width: 900px; margin: 40px auto; border-radius: 14px; box-shadow: 0 6px 28px rgba(0,0,0,.15); }
        .card-header-custom { background: #1aad94; padding: 20px; text-align: center; color: white; }
        .perm-toggle { min-width: 110px; }
        .grid-pager { text-align: center; padding-top: 10px; padding-bottom: 10px;}
        .no-select { pointer-events: none; /* Evita que el cursor interactúe */ background-color: #f8f9fa; /* Opcional: estilo visual */}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card-custom bg-white">
        <div class="card-header-custom">
            <h5 class="fw-bold mb-0">Administración de Roles</h5>
        </div>

        <div class="p-4">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary mb-3 fw-semibold"
                        OnClick="btnNuevo_Click" />

                    <asp:HiddenField ID="hfIdRol" runat="server" />

                    <asp:GridView ID="gvRoles" runat="server" CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False" DataKeyNames="id_rol" OnRowCommand="gvRoles_RowCommand"
                        AllowPaging="true" PageSize="10" OnPageIndexChanging="gvRoles_PageIndexChanging" PagerStyle-CssClass="grid-pager">
                        <Columns>
                            <asp:BoundField DataField="id_rol" HeaderText="ID" />
                            <asp:BoundField DataField="nombre_rol" HeaderText="Nombre" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Editar"
                                        CommandArgument='<%# Eval("id_rol") %>' CssClass="btn btn-sm btn-warning me-2">
                                        <i class="bi bi-pencil"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkEliminar" runat="server" CommandName="Eliminar"
                                        CommandArgument='<%# Eval("id_rol") %>' CssClass="btn btn-sm btn-danger">
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
                    <p>¿Está seguro que desea eliminar este rol?</p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmarEliminar" runat="server" Text="Eliminar" CssClass="btn btn-danger"
                        OnClick="btnConfirmarEliminar_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

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


    <!-- Modal para Agregar / Editar Rol -->
    <div id="modalRol" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title" id="modalTitle">Formulario Rol</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModal" runat="server">
                        <ContentTemplate>

                            <asp:HiddenField ID="hfAccion" runat="server" />

                            <div class="mb-3">
                                <label class="form-label fw-semibold">ID del rol</label>
                                <asp:TextBox ID="txtIDRol" runat="server" CssClass="form-control no-select" ReadOnly="true" />
                                <label class="form-label fw-semibold">Nombre del rol</label>
                                <asp:TextBox ID="txtRolNombre" runat="server" CssClass="form-control" />
                                <asp:Label ID="lblMensajeError" runat="server" CssClass="form-label fw-semibold text-danger" />
                            </div>

                            <hr />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnGuardarRol" runat="server" Text="Guardar" CssClass="btn btn-primary"
                        OnClick="btnGuardarRol_Click" CausesValidation="true" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Script para abrir/cerrar modal desde codigo -->
    <script type="text/javascript">
        function showModal() {
            var modalEl = document.getElementById('<%= modalRol.ClientID %>');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }
        function hideModal() {
            var modalEl = document.getElementById('<%= modalRol.ClientID %>');
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