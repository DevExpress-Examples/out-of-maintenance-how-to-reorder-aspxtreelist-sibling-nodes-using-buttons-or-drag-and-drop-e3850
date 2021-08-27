<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128548780/11.2.8%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E3850)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [Default.aspx](./CS/WebSite/Default.aspx) (VB: [Default.aspx](./VB/WebSite/Default.aspx))
* [Default.aspx.cs](./CS/WebSite/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
<!-- default file list end -->
# How to reorder ASPxTreeList sibling nodes, using buttons or drag-and-drop
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/e3850/)**
<!-- run online end -->


<p>This example demonstrates how to move TreeList nodes using buttons or Drag&Drop.</p>
<p>To persist the node order, it is necessary to set up an extra column to store node order indexes. Then, sort TreeList nodes by this column and deny sorting by other columns. This example stores this information in a dynamically created column and additionally, puts a dictionary "node key = node index" in the session. We have implemented this approach only for demo purposes. You can store this information in your DataSource.<br><br>See also: <br><a href="https://www.devexpress.com/Support/Center/p/T462559">T462559: TreeList - How to reorder sibling nodes</a><br><a href="https://www.devexpress.com/Support/Center/Example/Details/T450346">TreeList - How to implement node reordering</a> </p>

<br/>


