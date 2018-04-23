Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Data
Imports System.Data.OleDb
Imports DevExpress.Data
Imports DevExpress.Web.ASPxTreeList

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Private Function GetConnection() As OleDbConnection
		Dim connection As New OleDbConnection()
		connection.ConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", MapPath("~/App_Data/Departments.mdb"))
		Return connection
	End Function

	 Private Function GetData() As DataTable
		Dim DT As New DataTable()

		Using connection As OleDbConnection = GetConnection()
			Dim adapter As New OleDbDataAdapter(String.Empty, connection)
			adapter.SelectCommand.CommandText = "SELECT [ID], [PARENTID], [DEPARTMENT], [BUDGET], [LOCATION] FROM [Departments]"
			adapter.Fill(DT)
		End Using

		Return DT
	 End Function

	Private Sub UpdateTreeListButtons(ByVal treeList As ASPxTreeList)
		If treeList.FocusedNode IsNot Nothing Then
			Dim siblingNodes As TreeListNodeCollection
			If (treeList.FocusedNode.ParentNode Is Nothing) Then
				siblingNodes = treeList.Nodes
			Else
				siblingNodes = treeList.FocusedNode.ParentNode.ChildNodes
			End If
			treeList.JSProperties("cpbtMoveUp_Enabled") = Not(CInt(Fix(treeList.FocusedNode.GetValue("SortIndex"))) = 0)
			treeList.JSProperties("cpbtMoveDown_Enabled") = Not(CInt(Fix(treeList.FocusedNode.GetValue("SortIndex"))) = (siblingNodes.Count - 1))
		End If
	End Sub

	Protected Sub tlData_Init(ByVal sender As Object, ByVal e As EventArgs)
		Dim treeList As ASPxTreeList = TryCast(sender, ASPxTreeList)
		treeList.DataSource = GetData()
		treeList.DataBind()
	End Sub
	Protected Sub tlData_Load(ByVal sender As Object, ByVal e As EventArgs)
		UpdateTreeListButtons(TryCast(sender, ASPxTreeList))
	End Sub

	Protected Sub tlData_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
		Dim treeList As ASPxTreeList = TryCast(sender, ASPxTreeList)
		Dim data As DataTable = TryCast(treeList.DataSource, DataTable)

		If data IsNot Nothing Then
			' This code adds a dummy column to the TreeList's data source to store sort order values of nodes
			' You can avoid using it if you have a persistent column in your data source
			If (Not data.Columns.Contains("SortIndex")) Then
				data.Columns.Add("SortIndex", GetType(Integer))
			End If

			If treeList.Columns("SortIndex") Is Nothing Then
				Dim sortIndex As New TreeListTextColumn()
				sortIndex.FieldName = "SortIndex"
				sortIndex.Visible = False
				treeList.Columns.Add(sortIndex)
			End If
		End If
	End Sub

	''' <summary>
	''' This method fills the sort index column with appropriate values
	''' You can avoid using it if you have a persistent column in your data source
	''' </summary>
	Protected Sub tlData_DataBound(ByVal sender As Object, ByVal e As EventArgs)
		Dim treeList As ASPxTreeList = TryCast(sender, ASPxTreeList)

		Dim defaultNodes As Boolean = (Session(treeList.UniqueID & "_Sort") Is Nothing)
		Dim allNodes As ICollection(Of TreeListNode) = treeList.GetAllNodes()

		If defaultNodes Then
			Dim nodeOrder As New Dictionary(Of String, Integer)()
			For Each node As TreeListNode In allNodes
				Dim parentKey As String
				If (node.ParentNode Is Nothing) Then
					parentKey = String.Empty
				Else
					parentKey = node.ParentNode.Key
				End If
				Dim order As Integer = 0
				If nodeOrder.ContainsKey(parentKey) Then
					nodeOrder(parentKey) += 1
					order = nodeOrder(parentKey)
				Else
					nodeOrder(parentKey) = order
				End If
				node.SetValue("SortIndex", order)
			Next node

			Dim SortIndex As New Dictionary(Of String, Integer)()
			For Each node As TreeListNode In allNodes
				SortIndex.Add(node.Key, CInt(Fix(node.GetValue("SortIndex"))))
			Next node
			Session(treeList.UniqueID & "_Sort") = SortIndex
		Else
			Dim SortIndex As Dictionary(Of String, Integer) = TryCast(Session(treeList.UniqueID & "_Sort"), Dictionary(Of String, Integer))
			For Each node As TreeListNode In allNodes
				node.SetValue("SortIndex", SortIndex(node.Key))
			Next node
		End If

		treeList.SortBy(TryCast(treeList.Columns("SortIndex"), TreeListDataColumn), ColumnSortOrder.Ascending)
	End Sub

	Protected Sub tlData_HtmlRowPrepared(ByVal sender As Object, ByVal e As TreeListHtmlRowEventArgs)
		Dim treeList As ASPxTreeList = TryCast(sender, ASPxTreeList)
		If e.RowKind = TreeListRowKind.Data Then
			Dim parent As TreeListNode = treeList.FindNodeByKeyValue(e.NodeKey).ParentNode
			If (parent Is Nothing) Then
				e.Row.Attributes.Add("nodeParent",String.Empty)
			Else
				e.Row.Attributes.Add("nodeParent",parent.Key)
			End If
		End If
	End Sub

	Protected Sub tlData_CustomCallback(ByVal sender As Object, ByVal e As TreeListCustomCallbackEventArgs)
		Dim siblingNodes As TreeListNodeCollection
		Dim treeList As ASPxTreeList = TryCast(sender, ASPxTreeList)
		Dim SortIndex As Dictionary(Of String, Integer) = TryCast(Session(treeList.UniqueID & "_Sort"), Dictionary(Of String, Integer))

		If e.Argument = "MOVEUP" OrElse e.Argument = "MOVEDOWN" Then
			If (treeList.FocusedNode.ParentNode Is Nothing) Then
				siblingNodes = treeList.Nodes
			Else
				siblingNodes = treeList.FocusedNode.ParentNode.ChildNodes
			End If
			Dim siblingCount As Integer = siblingNodes.Count

			Dim Index As Integer = SortIndex(treeList.FocusedNode.Key)
			Dim NewIndex As Integer = Index

			If e.Argument = "MOVEUP" Then
				If (Index = 0) Then
					NewIndex = Index
				Else
					NewIndex = Index - 1
				End If
			End If
			If e.Argument = "MOVEDOWN" Then
				If (Index = (siblingCount - 1)) Then
					NewIndex = Index
				Else
					NewIndex = Index + 1
				End If
			End If

			For Each node As TreeListNode In siblingNodes
				If SortIndex(node.Key) = NewIndex Then
					SortIndex(treeList.FocusedNode.Key) = NewIndex
					SortIndex(node.Key) = Index
					Exit For
				End If
			Next node
		End If

		If e.Argument.Contains("DRAGNODE") Then
			Dim swapKeys() As String = e.Argument.Remove(0, 8).Split("|"c)
			Dim draggingNode As TreeListNode = treeList.FindNodeByKeyValue(swapKeys(0))
			Dim targetNode As TreeListNode = treeList.FindNodeByKeyValue(swapKeys(1))
			If (draggingNode IsNot Nothing) AndAlso (targetNode IsNot Nothing) Then
				If (targetNode.ParentNode Is Nothing) Then
					siblingNodes = treeList.Nodes
				Else
					siblingNodes = targetNode.ParentNode.ChildNodes
				End If

				Dim targetIndex As Integer = SortIndex(targetNode.Key)
				Dim draggingIndex As Integer = SortIndex(draggingNode.Key)
				Dim draggingDirection As Integer
				If (targetIndex < draggingIndex) Then
					draggingDirection = 1
				Else
					draggingDirection = -1
				End If

				For Each node As TreeListNode In siblingNodes
					If (SortIndex(node.Key) > Math.Min(targetIndex, draggingIndex)) AndAlso (SortIndex(node.Key) < Math.Max(targetIndex, draggingIndex)) Then
						SortIndex(node.Key) += draggingDirection
					End If
				Next node

				SortIndex(draggingNode.Key) = targetIndex
				SortIndex(targetNode.Key) = targetIndex + draggingDirection

			End If
		End If

		treeList.DataBind()
		UpdateTreeListButtons(treeList)
	End Sub
End Class