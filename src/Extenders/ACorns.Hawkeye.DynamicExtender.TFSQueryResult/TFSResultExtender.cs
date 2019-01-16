using System;
using System.Collections.Generic;
using System.Text;
using ACorns.Hawkeye.Public;
using ACorns.Hawkeye.Core.Utils.Accessors;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Threading;

namespace ACorns.Hawkeye.DynamicExtender.TFSQueryResult
{
	public enum SearchColumn
	{
		Title,
		Id,
		Description
	}

	public class TFSResultExtender : IDynamicSubclass
	{
		private DataGridView workItemResultGrid;
		private UserControl triageEditor;
		private Panel trigeResultInfoBar;

		private TextBox txtSearchBox;

		private FieldAccesor workItemsListAcc;
        //private FieldAccesor workItemsCachedAcc;

        //private IEnumerable oldWorkItemsList;
        //private IEnumerable currentWorkItemsList;

        //private PropertyAccesor titleAcc;
        //private PropertyAccesor idAccessor;
		private MethodAccesor resetRowsAcc;

		private DataGridViewColumn idColumn;
		private DataGridViewColumn titleColumn;

		private int lastSelectedIndex = -1;

		private int highlightRow = -1;

		#region Attach
		public void Attach(object target)
		{
			if (target == null)
				return;

			Type targetType = target.GetType();
			if (targetType.FullName == "Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.ResultView")
			{
				workItemResultGrid = target as DataGridView;

				workItemsListAcc = new FieldAccesor(workItemResultGrid, "m_items");
				resetRowsAcc = new MethodAccesor(workItemResultGrid.GetType(), "ResetRows");

				triageEditor = workItemResultGrid.Parent.Parent as UserControl;

				trigeResultInfoBar = FieldAccesor.GetValue(triageEditor, "m_infoBar") as Panel;

				AttachSearchBox();
			}
			else
			{
				return;
			}

		}

		void workItemResultGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (highlightRow != -1 && e != null)
			{
				if (e.RowIndex == highlightRow)
				{
					e.CellStyle.SelectionForeColor = Color.Red;
					e.CellStyle.SelectionBackColor = Color.LightBlue;
				}
			}
		}

		private void AttachSearchBox()
		{
			txtSearchBox = new TextBox();
			txtSearchBox.Location = new Point(0, 0);
			txtSearchBox.Size = new Size(100, txtSearchBox.Height);
			txtSearchBox.BorderStyle = BorderStyle.Fixed3D;
			txtSearchBox.Font = new Font("Tahoma", 8.25f);

			txtSearchBox.Dock = DockStyle.Right;

			trigeResultInfoBar.Controls.Add(txtSearchBox);

			txtSearchBox.TextChanged += new EventHandler(txtSearchBox_TextChanged);
			txtSearchBox.KeyDown += new KeyEventHandler(txtSearchBox_KeyDown);
			txtSearchBox.LostFocus += new EventHandler(txtSearchBox_LostFocus);
			txtSearchBox.GotFocus += new EventHandler(txtSearchBox_GotFocus);

			txtSearchBox.BackColor = Color.Coral;
		}

		void txtSearchBox_GotFocus(object sender, EventArgs e)
		{
			workItemResultGrid.CellFormatting -= new DataGridViewCellFormattingEventHandler(workItemResultGrid_CellFormatting);
			workItemResultGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(workItemResultGrid_CellFormatting);
		}

		void txtSearchBox_LostFocus(object sender, EventArgs e)
		{
			workItemResultGrid.CellFormatting -= new DataGridViewCellFormattingEventHandler(workItemResultGrid_CellFormatting); 
			highlightRow = -1;
			workItemResultGrid.Invalidate();
		}

		void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F3:
					{
						int previousValue = lastSelectedIndex;
						FindInGrid(true);
						if (previousValue == lastSelectedIndex && previousValue != -1)
						{
							// no new items were found, but we had at least one item previously - try again from the beginning
							FindInGrid(false);
						}
						break;
					}
				case Keys.Enter:
					{
						workItemResultGrid.Focus();
						break;
					}
			}
		}

		void txtSearchBox_TextChanged(object sender, EventArgs e)
		{
			FindInGrid(false);
		}
		#endregion

		private void FindInGrid(bool next)
		{		
			if (!next)
			{
				lastSelectedIndex = -1;
			}

			string textToFind = txtSearchBox.Text.Trim().ToLower();
			
			SearchColumn searchCol = SearchColumn.Title;

			if (textToFind.StartsWith("#"))
			{
				textToFind = textToFind.Substring(1);
				searchCol = SearchColumn.Id;
			}
			else if( textToFind.StartsWith("*") )
			{
				textToFind = textToFind.Substring(1);
				searchCol = SearchColumn.Id;
			}

			if ( textToFind.Length <= 1 )
				return;

			ResolveColumns();

			workItemResultGrid.ClearSelection();

			foreach (DataGridViewRow row in workItemResultGrid.Rows)
			{
				if (row.Index <= lastSelectedIndex)
					continue;
				
				object val = null;

				switch (searchCol)
				{
					case SearchColumn.Title:
						val = row.Cells[titleColumn.Index].Value;
						break;
					case SearchColumn.Id:
						val = row.Cells[idColumn.Index].Value;
						break;
					case SearchColumn.Description:
						break;
					default:
						// search in description inside the item!
						//row.DataBoundItem
						break;
				}
				
				if (val != null)
				{
					if (val.ToString().ToLower().Contains(textToFind))
					{
						SelectRow(row, true);
						break;
					}
				}
			}
		}

		private void SelectRow(DataGridViewRow row, bool highlight)
		{
			row.Selected = true;
			workItemResultGrid.CurrentCell = row.Cells[1];

			lastSelectedIndex = row.Index;
			highlightRow = lastSelectedIndex;
			workItemResultGrid.Invalidate();
		}

		private void ResolveColumns()
		{
			foreach (DataGridViewColumn col in workItemResultGrid.Columns)
			{
				if (col.Name == "ID")
				{
					idColumn = col;
				}
				else if (col.Name == "Title")
				{
					titleColumn = col;
				}
			}
		}

		//private void FilterSelectedElements()
		//{
		//    if (oldWorkItemsList == null || workItemsCachedAcc == null)
		//        return;

		//    string searchCriteria = txtSearchBox.Text;

		//    if (searchCriteria.Length > 0)
		//    {
		//        //ArrayList resultingList = new ArrayList();
		//        foreach (object workItem in currentWorkItemsList)
		//        {
		//            if (GetTitle(workItem).Contains(searchCriteria))
		//            {
		//                // select the item
		//                //resultingList.Add(workItem);
		//                int workItemId = Convert.ToInt32(idAccessor.Save().Value);
						
						
		//            }
		//        }

		//        // find this id in the rows
		//        workItemResultGrid.ClearSelection();
		//        foreach (DataGridViewRow row in workItemResultGrid.Rows)
		//        {
		//        }

		//        //// write the value back and force a reset!
		//        //object resultToWrite = resultingList.ToArray( oldWorkItemsList.GetType().GetElementType() );
		//        //workItemsCachedAcc.Set(resultToWrite);
		//    }
		//    else
		//    {
		//        // write back the original value
		//        //workItemsCachedAcc.Set(oldWorkItemsList);
		//    }
		//    //resetRowsAcc.Invoke(workItemResultGrid);
		//}

		//private void GrabTheWorkItemsList()
		//{
		//    object workItemCollection = workItemsListAcc.Get();
		//    if (workItemCollection == null)
		//        return;
		//    workItemsCachedAcc = new FieldAccesor(workItemCollection, "m_cache");

		//    object existingList = workItemsCachedAcc.Get();

		//    if (oldWorkItemsList == null || oldWorkItemsList != existingList)
		//    {
		//        oldWorkItemsList = existingList as IEnumerable;
		//        currentWorkItemsList = oldWorkItemsList;
		//    }

		//    foreach (DataGridViewColumn col in workItemResultGrid.Columns)
		//    {
		//        if (col.Name == "ID")
		//        {
		//            idColumn = col;
		//            break;
		//        }
		//    }
		//}

		//private string GetTitle(object workItem)
		//{
		//    if (workItem == null)
		//        return String.Empty;

		//    if (titleAcc == null)
		//    {
		//        titleAcc = new PropertyAccesor(workItem, "Title");
		//        idAccessor = new PropertyAccesor(workItem, "Id");
		//    }
			
		//    titleAcc.Target = workItem;
		//    idAccessor.Target = workItem;

		//    return titleAcc.Save().Value.ToString();
		//}
	}
}
