<%@ Page Language="C#" AutoEventWireup="true" Async="true" MasterPageFile="~/Site.Master" CodeBehind="MainOferentes.aspx.cs" Inherits="ADMExpedientePersonal.OFE.MainOferentes" %>

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
        .no-select { pointer-events: none; /* Evita que el cursor interactúe */ background-color: #f8f9fa; /* Opcional: estilo visual */}
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
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-primary mb-3 fw-semibold"
                        OnClick="btnNuevo_Click" />

                    <asp:HiddenField ID="hfIdOferente" runat="server" />

                    <asp:GridView ID="gvOferentes" runat="server" CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False" DataKeyNames="identificacion" OnRowCommand="gvOferentes_RowCommand"
                        AllowPaging="true" PageSize="10" OnPageIndexChanging="gvOferentes_PageIndexChanging" PagerStyle-CssClass="grid-pager">
                        <Columns>
                            <asp:BoundField DataField="identificacion" HeaderText="Identificación" />
                            <asp:BoundField DataField="nombre_completo" HeaderText="Nombre Completo" />
                            <asp:BoundField DataField="EmailDisplay" HeaderText="Email" />
                            <asp:BoundField DataField="TelefonoDisplay" HeaderText="Telefono" />
                            <asp:BoundField DataField="ConcursoDisplay" HeaderText="Concurso" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Editar"
                                        CommandArgument='<%# Eval("identificacion") %>' CssClass="btn btn-sm btn-warning me-2">
                                        <i class="bi bi-pencil"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkEliminar" runat="server" CommandName="Eliminar"
                                        CommandArgument='<%# Eval("identificacion") %>' CssClass="btn btn-sm btn-danger">
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
                    <p>¿Está seguro que desea eliminar este oferente?</p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmarEliminar" runat="server" Text="Eliminar" CssClass="btn btn-danger"
                        OnClick="btnConfirmarEliminar_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

        <!-- Modal para Agregar / Editar Oferente -->
    <div id="modalOferente" runat="server" class="modal fade" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header card-header-custom text-white">
                    <h5 class="modal-title" id="modalTitle">Formulario Oferente</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <asp:UpdatePanel ID="upModalOferente" runat="server">
                        <ContentTemplate>

                            <asp:HiddenField ID="hfAccion" runat="server" />

                            <!-- Tipo de identificación -->
                            <div class="mb-3">
                                <label class="form-label fw-semibold">Tipo de identificación</label>
                                <asp:DropDownList ID="ddlTipoIdentificacion" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoIdentificacion_SelectedIndexChanged">
                                    <asp:ListItem Text="Cédula de identidad" Value="Cedula" />
                                    <asp:ListItem Text="DIMEX" Value="Dimex" />
                                    <asp:ListItem Text="Pasaporte" Value="Pasaporte" />
                                </asp:DropDownList>
                            </div>

                            <!-- Identificación -->
                            <div class="mb-3">
                                <label class="form-label fw-semibold">Identificación</label>
                                <asp:TextBox ID="txtIdentificacion" runat="server" CssClass="form-control" />
    
                                <!-- Validador dinámico -->
                                <asp:RegularExpressionValidator 
                                    ID="revIdentificacion" 
                                    runat="server" 
                                    ControlToValidate="txtIdentificacion"
                                    ErrorMessage="Formato inválido para el tipo seleccionado"
                                    CssClass="text-danger" 
                                    Display="Dynamic" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label fw-semibold">Nombre completo</label>
                                <asp:TextBox ID="txtNombreCompleto" runat="server" CssClass="form-control" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label fw-semibold">Fecha de nacimiento</label>
                                <asp:TextBox ID="txtFechaNacimiento" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>

                            <!-- Correos electrónicos -->
                            <div class="mb-3">
                                <label class="form-label fw-semibold">Correos electrónicos</label>
                                <asp:Repeater ID="rptCorreos" runat="server">
                                    <ItemTemplate>
                                        <div class="input-group mb-2">
                                            <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" Text='<%# Container.DataItem %>' />
                                            <asp:LinkButton ID="btnEliminarCorreo" runat="server" CommandName="EliminarCorreo" CommandArgument='<%# Container.ItemIndex %>' CssClass="btn btn-danger">X</asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Button ID="btnAgregarCorreo" runat="server" Text="Agregar correo" CssClass="btn btn-sm btn-success" OnClick="btnAgregarCorreo_Click" />
                            </div>

                            <!-- Teléfonos -->
                            <div class="mb-3">
                                <label class="form-label fw-semibold">Teléfonos de contacto</label>
                                <asp:Repeater ID="rptTelefonos" runat="server">
                                    <ItemTemplate>
                                        <div class="input-group mb-2">
                                            <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" Text='<%# Container.DataItem %>' />
                                            <asp:LinkButton ID="btnEliminarTelefono" runat="server" CommandName="EliminarTelefono" CommandArgument='<%# Container.ItemIndex %>' CssClass="btn btn-danger">X</asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Button ID="btnAgregarTelefono" runat="server" Text="Agregar teléfono" CssClass="btn btn-sm btn-success" OnClick="btnAgregarTelefono_Click" />
                            </div>

                            <!-- Concursos -->
                            <div class="mb-3">
                                <label class="form-label fw-semibold">Concursos</label>
                                <asp:CheckBoxList ID="chkConcursos" runat="server" CssClass="form-check">
                                </asp:CheckBoxList>
                            </div>


                            <asp:Label ID="lblMensajeError" runat="server" CssClass="form-label fw-semibold text-danger" />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnGuardarOferente" runat="server" Text="Guardar" CssClass="btn btn-primary"
                        OnClick="btnGuardarOferente_Click" CausesValidation="true" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
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

    <!-- Script para abrir/cerrar modal desde codigo -->
    <script type="text/javascript">
        function showModal() {
            var modalEl = document.getElementById('<%= modalOferente.ClientID %>');
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        }
        function hideModal() {
            var modalEl = document.getElementById('<%= modalOferente.ClientID %>');
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