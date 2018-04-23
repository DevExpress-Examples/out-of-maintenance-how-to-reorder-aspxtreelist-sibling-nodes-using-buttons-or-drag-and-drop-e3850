using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using DevExpress.Data;
using DevExpress.Web.ASPxTreeList;

public partial class _Default : System.Web.UI.Page {
    private OleDbConnection GetConnection() {
        OleDbConnection connection = new OleDbConnection();
        connection.ConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MapPath("~/App_Data/Departments.mdb"));
        return connection;
    }

     DataTable GetData() {
        DataTable DT = new DataTable();

        using(OleDbConnection connection = GetConnection()) {
            OleDbDataAdapter adapter = new OleDbDataAdapter(string.Empty, connection);
            adapter.SelectCommand.CommandText = "SELECT [ID], [PARENTID], [DEPARTMENT], [BUDGET], [LOCATION] FROM [Departments]";
            adapter.Fill(DT);
        }

        return DT;
    }

    void UpdateTreeListButtons(ASPxTreeList treeList) {
        if(treeList.FocusedNode != null) {
            TreeListNodeCollection siblingNodes = (treeList.FocusedNode.ParentNode == null) ? treeList.Nodes : treeList.FocusedNode.ParentNode.ChildNodes;
            treeList.JSProperties["cpbtMoveUp_Enabled"] = !((int)treeList.FocusedNode.GetValue("SortIndex") == 0);
            treeList.JSProperties["cpbtMoveDown_Enabled"] = !((int)treeList.FocusedNode.GetValue("SortIndex") == (siblingNodes.Count - 1));
        }
    }

    protected void tlData_Init(object sender, EventArgs e) {
        ASPxTreeList treeList = sender as ASPxTreeList;
        treeList.DataSource = GetData();
        treeList.DataBind();
    }
    protected void tlData_Load(object sender, EventArgs e) {
        UpdateTreeListButtons(sender as ASPxTreeList);
    }

    protected void tlData_DataBinding(object sender, EventArgs e) {
        ASPxTreeList treeList = sender as ASPxTreeList;
        DataTable data = treeList.DataSource as DataTable;

        if(data != null) {
            // This code adds a dummy column to the TreeList's data source to store sort order values of nodes
            // You can avoid using it if you have a persistent column in your data source
            if(!data.Columns.Contains("SortIndex"))
                data.Columns.Add("SortIndex", typeof(int));

            if(treeList.Columns["SortIndex"] == null) {
                TreeListTextColumn sortIndex = new TreeListTextColumn();
                sortIndex.FieldName = "SortIndex";
                sortIndex.Visible = false;
                treeList.Columns.Add(sortIndex);
            }
        }
    }

    /// <summary>
    /// This method fills the sort index column with appropriate values
    /// You can avoid using it if you have a persistent column in your data source
    /// </summary>
    protected void tlData_DataBound(object sender, EventArgs e) {
        ASPxTreeList treeList = sender as ASPxTreeList;

        bool defaultNodes = (Session[treeList.UniqueID + "_Sort"] == null);
        ICollection<TreeListNode> allNodes = treeList.GetAllNodes();

        if(defaultNodes) {
            Dictionary<string, int> nodeOrder = new Dictionary<string, int>();
            foreach(TreeListNode node in allNodes) {
                string parentKey = (node.ParentNode == null) ? String.Empty : node.ParentNode.Key;
                int order = 0;
                if(nodeOrder.ContainsKey(parentKey))
                    order = ++nodeOrder[parentKey];
                else
                    nodeOrder[parentKey] = order;
                node.SetValue("SortIndex", order);
            }

            Dictionary<string, int> SortIndex = new Dictionary<string, int>();
            foreach(TreeListNode node in allNodes) {
                SortIndex.Add(node.Key, (int)node.GetValue("SortIndex"));
            }
            Session[treeList.UniqueID + "_Sort"] = SortIndex;
        }
        else {
            Dictionary<string, int> SortIndex = Session[treeList.UniqueID + "_Sort"] as Dictionary<string, int>;
            foreach(TreeListNode node in allNodes) {
                node.SetValue("SortIndex", SortIndex[node.Key]);
            }
        }

        treeList.SortBy(treeList.Columns["SortIndex"] as TreeListDataColumn, ColumnSortOrder.Ascending);
    }

    protected void tlData_HtmlRowPrepared(object sender, TreeListHtmlRowEventArgs e) {
        ASPxTreeList treeList = sender as ASPxTreeList;
        if(e.RowKind == TreeListRowKind.Data) {
            TreeListNode parent = treeList.FindNodeByKeyValue(e.NodeKey).ParentNode;
            e.Row.Attributes.Add("nodeParent", (parent == null) ? String.Empty : parent.Key);
        }
    }

    protected void tlData_CustomCallback(object sender, TreeListCustomCallbackEventArgs e) {
        TreeListNodeCollection siblingNodes;
        ASPxTreeList treeList = sender as ASPxTreeList;
        Dictionary<string, int> SortIndex = Session[treeList.UniqueID + "_Sort"] as Dictionary<string, int>;

        if(e.Argument == "MOVEUP" || e.Argument == "MOVEDOWN") {
            siblingNodes = (treeList.FocusedNode.ParentNode == null) ? treeList.Nodes : treeList.FocusedNode.ParentNode.ChildNodes;
            int siblingCount = siblingNodes.Count;

            int Index = SortIndex[treeList.FocusedNode.Key];
            int NewIndex = Index;

            if(e.Argument == "MOVEUP") {
                NewIndex = (Index == 0) ? Index : Index - 1;
            }
            if(e.Argument == "MOVEDOWN") {
                NewIndex = (Index == (siblingCount - 1)) ? Index : Index + 1;
            }

            foreach(TreeListNode node in siblingNodes) {
                if(SortIndex[node.Key] == NewIndex) {
                    SortIndex[treeList.FocusedNode.Key] = NewIndex;
                    SortIndex[node.Key] = Index;
                    break;
                }
            }
        }

        if(e.Argument.Contains("DRAGNODE")) {
            string[] swapKeys = e.Argument.Remove(0, 8).Split('|');
            TreeListNode draggingNode = treeList.FindNodeByKeyValue(swapKeys[0]);
            TreeListNode targetNode = treeList.FindNodeByKeyValue(swapKeys[1]);
            if((draggingNode != null) && (targetNode != null)) {
                siblingNodes = (targetNode.ParentNode == null) ? treeList.Nodes : targetNode.ParentNode.ChildNodes;

                int targetIndex = SortIndex[targetNode.Key];
                int draggingIndex = SortIndex[draggingNode.Key];
                int draggingDirection = (targetIndex < draggingIndex) ? 1 : -1;

                foreach(TreeListNode node in siblingNodes) {
                    if((SortIndex[node.Key] > Math.Min(targetIndex, draggingIndex)) && (SortIndex[node.Key] < Math.Max(targetIndex, draggingIndex))) {
                        SortIndex[node.Key] += draggingDirection;
                    }
                }

                SortIndex[draggingNode.Key] = targetIndex;
                SortIndex[targetNode.Key] = targetIndex + draggingDirection;

            }
        }

        treeList.DataBind();
        UpdateTreeListButtons(treeList);
    }
}