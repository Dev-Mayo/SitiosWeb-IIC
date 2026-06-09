<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="VerBitacoras.aspx.cs"
    Inherits="ADMExpedientePersonal.GEN.VerBitacoras" %>

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
        .descripcion-cell { max-width: 400px; word-break: break-word; font-size: 0.85rem; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card-custom bg-white">
        <div class="card-header-custom">
            <h5 class="fw-bold mb-0">Bitácora del Sistema</h5>
        </div>

        <div class="p-4">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />

            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>

                    <!-- Filters -->
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label fw-semibold">Filtrar por usuario</label>
                            <asp:TextBox ID="txtFiltroUsuario" runat="server"
                                CssClass="form-control" placeholder="Nombre de usuario..." />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label fw-semibold">Filtrar por descripción</label>
                            <asp:TextBox ID="txtFiltroDescripcion" runat="server"
                                CssClass="form-control" placeholder="Texto en descripción..." />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label fw-semibold">Ordenar por</label>
                            <asp:DropDownList ID="ddlOrden" runat="server" CssClass="form-select">
                                <asp:ListItem Value="fecha_desc"    Text="Fecha (más reciente)" Selected="True" />
                                <asp:ListItem Value="fecha_asc"     Text="Fecha (más antigua)" />
                                <asp:ListItem Value="usuario_asc"   Text="Usuario A-Z" />
                                <asp:ListItem Value="usuario_desc"  Text="Usuario Z-A" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2 d-flex align-items-end">
                            <asp:Button ID="btnFiltrar" runat="server"
                                Text="Filtrar"
                                CssClass="btn btn-primary w-100"
                                OnClick="btnFiltrar_Click" />
                        </div>
                    </div>

                    <asp:GridView ID="gvBitacoras" runat="server"
                        CssClass="table table-striped table-bordered"
                        AutoGenerateColumns="False"
                        AllowPaging="true"
                        PageSize="100"
                        PagerStyle-CssClass="grid-pager"
                        OnPageIndexChanging="gvBitacoras_PageIndexChanging">

                        <Columns>
                            <asp:BoundField DataField="fecha"       HeaderText="Fecha"
                                DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="160px" />
                            <asp:BoundField DataField="usuario"     HeaderText="Usuario"    ItemStyle-Width="150px" />
                            <asp:BoundField DataField="accion"      HeaderText="Acción"     ItemStyle-Width="80px" />
                            <asp:TemplateField HeaderText="Descripción">
                                <ItemTemplate>
                                    <div class="descripcion-cell"><%# Eval("descripcion") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView>

                </ContentTemplate>
            </asp:UpdatePanel>
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
        function showMensajeModal() {
            var modalEl = document.getElementById('<%= modalMensaje.ClientID %>');
            bootstrap.Modal.getOrCreateInstance(modalEl).show();
        }
    </script>

</asp:Content>