using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CustomControls
{
    public class DataGridViewEx : DataGridView
    {
        private Dictionary<DataGridViewColumn, bool> Checkboxes = new();
        private Bitmap[] CheckBoxImage = new Bitmap[2];

        private bool DirtyValue = true;

        public DataGridViewEx() : base()
        {
            #region CheckBox in the column header

            CheckBox chkTemp = new()
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Size = new Size(16, 16),
                UseVisualStyleBackColor = false
            };

            CheckBoxImage[0] = new Bitmap(chkTemp.Width, chkTemp.Height);
            CheckBoxImage[1] = new Bitmap(chkTemp.Width, chkTemp.Height);

            chkTemp.Checked = false;
            chkTemp.DrawToBitmap(CheckBoxImage[0], new Rectangle(0, 0, chkTemp.Width, chkTemp.Height));

            chkTemp.Checked = true;
            chkTemp.DrawToBitmap(CheckBoxImage[1], new Rectangle(0, 0, chkTemp.Width, chkTemp.Height));

            #endregion
        }

        public void CheckBoxHeader(DataGridViewCheckBoxColumn ColumnToCheck, bool enabled)
        {
            if (enabled == true)
            {
                if (Checkboxes.Any(Column => Column.Key == ColumnToCheck) == false)
                {
                    Checkboxes.Add(ColumnToCheck, false);
                    InvalidateCell(ColumnToCheck.HeaderCell);
                }
            }
            else
            {
                bool unused = Checkboxes.Remove(ColumnToCheck);

                InvalidateCell(ColumnToCheck.HeaderCell);
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs Cell)
        {
            base.OnCellPainting(Cell);

            if (Cell.ColumnIndex >= 0 && Cell.RowIndex == -1 && Checkboxes.Any(f => f.Key == Columns[Cell.ColumnIndex]) == true)
            {
                Bitmap bmp = Checkboxes[Columns[Cell.ColumnIndex]] == true ? CheckBoxImage[1] : CheckBoxImage[0];

                Rectangle imageBounds = new(new Point(Cell.CellBounds.Location.X + (Cell.CellBounds.Width / 2) - (bmp.Size.Width / 2), Cell.CellBounds.Location.Y + (Cell.CellBounds.Height / 2) - (bmp.Size.Height / 2)), bmp.Size);

                Cell.PaintBackground(Cell.CellBounds, true);
                Cell.PaintContent(Cell.CellBounds);
                Cell.Graphics.DrawImage(bmp, imageBounds);
                Cell.Handled = true;
            }
        }

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs Cell)
        {
            base.OnColumnHeaderMouseClick(Cell);

            if (Checkboxes.ContainsKey(Columns[Cell.ColumnIndex]) == true)
            {
                DataGridViewColumnHeaderCell header = Columns[Cell.ColumnIndex].HeaderCell;
                Bitmap img = Checkboxes[Columns[Cell.ColumnIndex]] == true ? CheckBoxImage[1] : CheckBoxImage[0];

                if (Cell.Button == MouseButtons.Left &&
                    Cell.Y >= header.ContentBounds.Y + (header.Size.Height / 2) - (img.Height / 2) &&
                    Cell.Y <= header.ContentBounds.Y + (header.Size.Height / 2) + (img.Height / 2) &&
                    Cell.X >= header.ContentBounds.X + (Columns[Cell.ColumnIndex].Width / 2) - (img.Width / 2) &&
                    Cell.X <= header.ContentBounds.X + (Columns[Cell.ColumnIndex].Width / 2) + (img.Width / 2))
                {
                    Checkboxes[Columns[Cell.ColumnIndex]] = !Checkboxes[Columns[Cell.ColumnIndex]];

                    InvalidateCell(Columns[Cell.ColumnIndex].HeaderCell);

                    DirtyValue = false;
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        Rows[i].Cells[Cell.ColumnIndex].Value = Checkboxes[Columns[Cell.ColumnIndex]];
                        bool unused = RefreshEdit();
                    }
                    DirtyValue = true;
                }

            }
        }

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs Row)
        {
            base.OnRowsAdded(Row);

            List<DataGridViewColumn> ColumnList = Columns.Cast<DataGridViewColumn>().Where(column => column.GetType() == typeof(DataGridViewCheckBoxColumn)).ToList();

            foreach (DataGridViewColumn Column in ColumnList)
            {
                if (Checkboxes.ContainsKey(Column) == true)
                {
                    if (Checkboxes[Column] == true)
                    {
                        DirtyValue = false;

                        Rows[Row.RowIndex].Cells[Column.Index].Value = true;
                        bool unused = RefreshEdit();

                        DirtyValue = true;
                    }
                }
            }
        }

        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs Row)
        {
            base.OnRowsRemoved(Row);

            List<DataGridViewColumn> ColumnList = Columns.Cast<DataGridViewColumn>().Where(column => column.GetType() == typeof(DataGridViewCheckBoxColumn)).ToList();

            foreach (DataGridViewColumn Column in ColumnList)
            {
                if (Checkboxes.ContainsKey(Column) == true)
                {
                    if (Rows.Count == 0)
                    {
                        Checkboxes[Column] = false;
                        InvalidateCell(Column.HeaderCell);
                    }
                    else
                    {
                        int RowCount = Rows.Cast<DataGridViewRow>().Where(row => Convert.ToBoolean(row.Cells[Column.Index].Value) == true).Count();
                        if (RowCount == Rows.Count)
                        {
                            Checkboxes[Column] = true;
                            InvalidateCell(Column.HeaderCell);
                        }
                    }
                }
            }
        }

        protected override void OnCellValueChanged(DataGridViewCellEventArgs Cell)
        {
            base.OnCellValueChanged(Cell);

            if (Cell.ColumnIndex >= 0 && Cell.RowIndex >= 0)
            {
                if (Checkboxes.ContainsKey(Columns[Cell.ColumnIndex]) == true)
                {
                    if (DirtyValue == false)
                    {
                        return;
                    }

                    bool FalseValuesExist = Rows.Cast<DataGridViewRow>().Any(row => Convert.ToBoolean(row.Cells[Cell.ColumnIndex].Value) == false);

                    if (FalseValuesExist == true)
                    {
                        if (Checkboxes[Columns[Cell.ColumnIndex]] == true)
                        {
                            Checkboxes[Columns[Cell.ColumnIndex]] = false;
                            InvalidateCell(Columns[Cell.ColumnIndex].HeaderCell);
                        }
                    }
                    else
                    {
                        Checkboxes[Columns[Cell.ColumnIndex]] = true;
                        InvalidateCell(Columns[Cell.ColumnIndex].HeaderCell);
                    }
                }
            }
        }

        protected override void OnCurrentCellDirtyStateChanged(EventArgs Cell)
        {
            base.OnCurrentCellDirtyStateChanged(Cell);

            if (CurrentCell is DataGridViewCheckBoxCell)
            {
                bool unused = CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
