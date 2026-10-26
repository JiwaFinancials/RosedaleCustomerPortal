using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections;

namespace JiwaCustomerPortal.Components.Grid
{
    public class Columns : IEnumerable<Column>
    {
        public delegate void AddedEventHandler(Column column);
        public event AddedEventHandler? Added;

        public delegate void RemovedEventHandler(Column column);
        public event RemovedEventHandler? Removed;

        private Dictionary<string, Column> _columns = new Dictionary<string, Column>();
        
        public void Add(Column column)
        {
            if (_columns.ContainsKey(column.Id))
            {
                throw new Exception($"Column with ID '{column.Id}' already exists.");
            }

            _columns.Add(column.Id, column);
            Added?.Invoke(column);
        }

        public void Remove(string columnId)
        {
            if (!_columns.ContainsKey(columnId))
            {
                throw new Exception($"Column with ID '{columnId}' does not exist.");
            }

            Column column = _columns[columnId];
            _columns.Remove(columnId);
            Removed?.Invoke(column);
        }

        public IEnumerator<Column> GetEnumerator()
        {
            return this._columns.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Column? this[string columnId]
        {
            get
            {
                if (_columns.ContainsKey(columnId))
                {
                    return _columns[columnId];
                }
                else
                {
                    return null;
                }
            }

        }        
    }
}
