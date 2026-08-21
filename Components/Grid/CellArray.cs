using static ServiceStack.Script.Lisp;

namespace JiwaCustomerPortal.Components.Grid
{
    public class CellArray
    {
        private Dictionary<string, Cell> _Cells { get; set; } = new Dictionary<string, Cell>();

        public Columns Columns { get; set; } = new Columns();

        private int _RowCount = 0;
        public int RowCount
        {
            get => _RowCount;
            set
            {
                if (value < _RowCount)
                {
                    // decreasing row count removes cells - remove all cells for the rows that are being removed
                    for (int row = value; row < _RowCount; row++)
                    {
                        RemoveRow(row);
                    }
                }
                _RowCount = value;
            }
        }

        public CellArray()
        {            
            Columns = new Columns();
        }

        private string cellKey(string columnId, int row)
        {
            return $"{columnId}:{row}";
        }

        public int RowFromCellKeyValue(string columnId, string keyValue)
        {
            for (int row = 0; row < RowCount; row++)
            {
                if (this[columnId, row].Value?.ToString() == keyValue)
                {
                    return row;
                }
            }
            return -1;
        }

        internal void RemoveRow(int Row)
        {
            foreach (Column column in Columns)
            {
                string key = cellKey(column.Id, Row);
                if (_Cells.ContainsKey(key))
                {
                    _Cells.Remove(key);
                }
            }
        }

        public Cell this[string columnId, int row]
        {
            get
            {
                string key = cellKey(columnId, row);
                if (_Cells.ContainsKey(key))
                {
                    return _Cells[key];
                }
                else
                {
                    Column? column = Columns[columnId];
                    if (column is null)
                    {
                        throw new Exception($"Column with Id '{columnId}' not found.");
                    }

                    Cell newCell = new Cell() { Column = column, Row = row, CellType = column.CellType, HorizontalAlignment = column.CellType.HorizontalAlignment };
                    _Cells.Add(key, newCell);

                    return newCell;
                }
            }
        }
    }
}
