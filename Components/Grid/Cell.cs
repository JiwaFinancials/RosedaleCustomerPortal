using JiwaCustomerPortal.Components.Grid.CellType;
using Microsoft.AspNetCore.Components;

namespace JiwaCustomerPortal.Components.Grid
{
    public class Cell
    {
        private object? _value;
        private CellType.ICellType _cellType;
        public CellType.ICellType CellType 
        { 
            get
            {
                return _cellType;
            }
            set
            {
                _cellType = value;

                if (value != null)
                {
                    HorizontalAlignment = _cellType.HorizontalAlignment;                    
                }                
            }
        }        
        public object? Value 
        { 
            get
            {
                return _value;
            }
            set
            {
                switch(CellType)
                {
                    case CheckboxCellType:
                        if (value is bool)
                        {
                            _value = (bool?)value;
                        }
                        else if (value != null && !string.IsNullOrWhiteSpace((string)value))
                        {
                            _value = bool.Parse((string)value);
                        }
                        else
                        {
                            _value = null;
                        }                            
                        break;

                    case ComboCellType:
                        _value = (string?)value;
                        break;

                    case DateCellType:
                        if (value is DateTime)
                        {
                            _value = value;
                        }
                        else if (value != null && !string.IsNullOrWhiteSpace((string)value))
                        {
                            _value = DateTime.Parse((string)value);
                        }
                        else
                        {
                            _value = null;
                        }
                        break;

                    case DecimalCellType:
                        if (value is decimal)
                        {
                            _value = (decimal?)value;
                        }
                        else if (value != null && !string.IsNullOrWhiteSpace((string)value))
                        {
                            _value = decimal.Parse((string)value);
                        }
                        else
                        {
                            _value = null;
                        }
                        break;

                    case IntegerCellType:
                        if (value is Int32)
                        {
                            _value = (int?)value;
                        }
                        else if (value != null && !string.IsNullOrWhiteSpace((string)value))
                        {
                            _value = int.Parse((string)value);
                        }   
                        else 
                        {
                            _value = null;
                        }                            
                        break;

                    case TextCellType:
                        _value = (string?)value;
                        break;
                    default:
                        _value = value;
                        break;
                }                
            }
        }
        public bool IsEditable { get; set; }
        public int Row { get; set; }
        public Column Column { get; set; }
        public CellType.ICellType.HAlignment HorizontalAlignment { get; set; }
        public int? ColSpan { get; set; } = 1;
        public string Key
        {
            get
            {
                return $"{Column.Id}:{Row}";
            }
        }

        public RenderFragment CustomRenderFragment { get; set; }
    }
}
