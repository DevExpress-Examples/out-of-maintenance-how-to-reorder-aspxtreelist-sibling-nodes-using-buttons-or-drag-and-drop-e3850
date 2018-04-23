<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.ASPxTreeList.v11.2, Version=11.2.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxTreeList" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v11.2, Version=11.2.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        function tlData_Init(s, e) {
            btMoveUp.SetEnabled(s.cpbtMoveUp_Enabled);
            btMoveDown.SetEnabled(s.cpbtMoveDown_Enabled);
        }

        function tlData_StartDragNode(s, e) {
            var nodeKeys = s.GetVisibleNodeKeys();
            e.targets.length = 0;
            for (var i = 0; i < nodeKeys.length; i++) {
                if (s.GetNodeHtmlElement(nodeKeys[i]).getAttribute("nodeParent") == s.GetNodeHtmlElement(e.nodeKey).getAttribute("nodeParent")) {
                    e.targets.push(s.GetNodeHtmlElement(nodeKeys[i]));
                }
            }
        }

        function tlData_EndDragNode(s, e) {
            var nodeKeys = s.GetVisibleNodeKeys();
            for (var i = 0; i < nodeKeys.length; i++) {
                if (s.GetNodeHtmlElement(nodeKeys[i]) == e.targetElement) {
                    s.PerformCallback("DRAGNODE" + e.nodeKey + "|" + nodeKeys[i]);
                    return;
                }
            }

            e.cancel = true;
        }

        function btMoveUp_Click(s, e) {
            tlData.PerformCallback("MOVEUP");
        }

        function btMoveDown_Click(s, e) {
            tlData.PerformCallback("MOVEDOWN");
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td rowspan="2" width="80%">
                    <dx:ASPxTreeList ID="tlData" runat="server" Width="100%" AutoGenerateColumns="False"
                        KeyFieldName="ID" ParentFieldName="PARENTID" OnDataBinding="tlData_DataBinding"
                        OnDataBound="tlData_DataBound" OnInit="tlData_Init" OnCustomCallback="tlData_CustomCallback"
                        OnLoad="tlData_Load" OnHtmlRowPrepared="tlData_HtmlRowPrepared">
                        <Columns>
                            <dx:TreeListTextColumn FieldName="DEPARTMENT" VisibleIndex="0" AllowSort="False">
                            </dx:TreeListTextColumn>
                            <dx:TreeListTextColumn FieldName="BUDGET" VisibleIndex="1" AllowSort="False">
                            </dx:TreeListTextColumn>
                            <dx:TreeListTextColumn FieldName="LOCATION" VisibleIndex="2" AllowSort="False">
                            </dx:TreeListTextColumn>
                        </Columns>
                        <SettingsBehavior AllowFocusedNode="True" AutoExpandAllNodes="True" ProcessFocusedNodeChangedOnServer="True" />
                        <SettingsEditing AllowNodeDragDrop="True" />
                        <ClientSideEvents Init="tlData_Init" EndCallback="tlData_Init" EndDragNode="tlData_EndDragNode"
                            StartDragNode="tlData_StartDragNode" />
                    </dx:ASPxTreeList>
                </td>
                <td valign="bottom">
                    <dx:ASPxButton ID="btMoveUp" runat="server" Text="Move Up" Width="100%" AutoPostBack="false">
                        <ClientSideEvents Click="btMoveUp_Click" />
                    </dx:ASPxButton>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <dx:ASPxButton ID="btMoveDown" runat="server" Text="Move Down" Width="100%" AutoPostBack="false">
                        <ClientSideEvents Click="btMoveDown_Click" />
                    </dx:ASPxButton>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
