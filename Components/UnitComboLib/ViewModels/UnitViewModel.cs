namespace UnitComboLib.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using UnitComboLib.Command;
    using UnitComboLib.Local;
    using UnitComboLib.Models;
    using UnitComboLib.Models.Unit;

    /// <summary>
    /// Viewmodel class to manage unit conversion based on default values and typed values.
    /// </summary>
    internal class UnitViewModel : BaseViewModel, IDataErrorInfo, IUnitViewModel
    {
        #region fields
        private ListItem _SelectedItem = null;

        private readonly ObservableCollection<ListItem> _UnitList = null;

        private string _ValueTip = string.Empty;
        private double _Value = 0;
        private string _StrValue = "0.0";

        private readonly Converter _UnitConverter = null;

        private RelayCommand<Itemkey> _SetSelectedItemCommand = null;

        private string _MaxStringLengthValue = "#####";

        /// <summary>
        /// Minimum value to be converted for both percentage and pixels
        /// </summary>
        private const double MinFontSizeValue = 2.0;

        /// <summary>
        /// Maximum value to be converted for both percentage and pixels
        /// </summary>
        private const double MaxFontSizeValue = 399;

        /// <summary>
        /// Minimum value to be converted for both percentage and pixels
        /// </summary>
        private const double MinPercentageSizeValue = 24;

        /// <summary>
        /// Maximum value to be converted for both percentage and pixels
        /// </summary>
        private const double MaxPercentageSizeValue = 3325.0;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor to construct complete viewmodel object from listed parameters.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="unitConverter"></param>
        /// <param name="defaultIndex"></param>
        /// <param name="defaultValue"></param>
        public UnitViewModel(IList<ListItem> list,
                             Converter unitConverter,
                             int defaultIndex = 0,
                             double defaultValue = 100)
        {
            _UnitList = new ObservableCollection<ListItem>(list);
            _SelectedItem = _UnitList[defaultIndex];

            _UnitConverter = unitConverter;

            _Value = defaultValue;
            _StrValue = string.Format("{0:0}", _Value);
        }

        /// <summary>
        /// Standard class constructor is hidden in favour of parameterized constructor.
        /// </summary>
        protected UnitViewModel()
        {
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Currently selected value in screen points. This property is needed because the run-time system
        /// cannot work with percent values directly. Therefore, this property always ensures a correct
        /// font size no matter what the user selected in percent.
        /// </summary>
        public int ScreenPoints
        {
            get
            {
                if (SelectedItem != null)
                    return (int)_UnitConverter.Convert(SelectedItem.Key, _Value, Itemkey.ScreenFontPoints);

                // Fallback to default if all else fails
                return (int)Models.Unit.Screen.ScreenConverter.OneHundretPercentFont;
            }

            set
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.Key == Itemkey.ScreenFontPoints)
                    {
                        if (value != Value)
                            Value = value;
                    }
                    else
                    {
                        if (value != (int)_UnitConverter.Convert(SelectedItem.Key, _Value, Itemkey.ScreenFontPoints))
                            Value = (int)_UnitConverter.Convert(Itemkey.ScreenFontPoints, value, SelectedItem.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Get list of units, their default value lists, itemkeys etc.
        /// </summary>
        public ObservableCollection<ListItem> UnitList
        {
            get
            {
                return _UnitList;
            }
        }

        /// <summary>
        /// Get/set currently selected unit key, converter, and default value list.
        /// </summary>
        public ListItem SelectedItem
        {
            get
            {
                return _SelectedItem;
            }

            set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;

                    RaisePropertyChanged(() => SelectedItem);
                    RaisePropertyChanged(() => ScreenPoints);
                    RaisePropertyChanged(() => MinValue);
                    RaisePropertyChanged(() => MaxValue);
                }
            }
        }

        #region IDataErrorInfo Interface
        /// <summary>
        /// Source: http://joshsmithonwpf.wordpress.com/2008/11/14/using-a-viewmodel-to-provide-meaningful-validation-error-messages/
        /// </summary>
        public string Error
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Standard property that is part of the <seealso cref="IDataErrorInfo"/> interface.
        /// 
        /// Evaluetes whether StringValue parameter represents a value within the expected range
        /// and sets a corresponding errormessage in the ValueTip property if not.
        /// 
        /// Source: http://joshsmithonwpf.wordpress.com/2008/11/14/using-a-viewmodel-to-provide-meaningful-validation-error-messages/
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "StringValue")
                {
                    if (string.IsNullOrEmpty(_StrValue))
                        return SetToolTip(Strings.Integer_Contain_ErrorMessage);

                    if (double.TryParse(_StrValue, out double dValue) == true)
                    {
                        if (IsDoubleWithinRange(dValue, SelectedItem.Key, out string message) == true)
                        {
                            Value = dValue;
                            return SetToolTip(null);
                        }
                        else
                            return SetToolTip(message);
                    }
                    else
                        return SetToolTip(Strings.Integer_Conversion_ErrorMessage);
                }

                return SetToolTip(null);
            }
        }
        #endregion IDataErrorInfo Interface

        /// <summary>
        /// Get a string that indicates the format of the
        /// expected input or a an error if the current input is not valid.
        /// </summary>
        public string ValueTip
        {
            get
            {
                return _ValueTip;
            }

            protected set
            {
                if (_ValueTip != value)
                {
                    _ValueTip = value;
                    RaisePropertyChanged(() => ValueTip);
                }
            }
        }

        /// <summary>
        /// Get/sets a string that represents a convinient maximum length in
        /// characters to measure the width for the displaying control.
        /// </summary>
        public string MaxStringLengthValue
        {
            get
            {
                return _MaxStringLengthValue;
            }

            set
            {
                if (_MaxStringLengthValue != value)
                {
                    _MaxStringLengthValue = value;
                    RaisePropertyChanged(() => MaxStringLengthValue);
                }
            }
        }

        /// <summary>
        /// String representation of the double value that
        /// represents the unit scaled value in this object.
        /// </summary>
        public string StringValue
        {
            get
            {
                return _StrValue;
            }

            set
            {
                if (_StrValue != value)
                {
                    _StrValue = value;
                    RaisePropertyChanged(() => StringValue);
                }
            }
        }

        /// <summary>
        /// Get double value represented in unit as indicated by SelectedItem.Key.
        /// </summary>
        public double Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    _StrValue = string.Format("{0:0}", _Value);

                    RaisePropertyChanged(() => Value);
                    RaisePropertyChanged(() => StringValue);
                    RaisePropertyChanged(() => ScreenPoints);
                }
            }
        }

        /// <summary>
        /// Get the legal minimum value in dependency of the current unit.
        /// </summary>
        public double MinValue
        {
            get
            {
                return GetMinValue(SelectedItem.Key);
            }
        }

        /// <summary>
        /// Get the legal maximum value in dependency of the current unit.
        /// </summary>
        public double MaxValue
        {
            get
            {
                return GetMaxValue(SelectedItem.Key);
            }
        }

        /// <summary>
        /// Get command to be executed when the user has selected a unit
        /// (eg. 'Km' is currently used but user selected 'm' to be used next)
        /// </summary>
        public ICommand SetSelectedItemCommand
        {
            get
            {
                if (_SetSelectedItemCommand == null)
                    _SetSelectedItemCommand = new RelayCommand<Itemkey>(p => SetSelectedItemExecuted(p),
                                                                             p => true);

                return _SetSelectedItemCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Convert current double value from current unit to
        /// unit as indicated by <paramref name="unitKey"/> and
        /// set corresponding SelectedItem.
        /// </summary>
        /// <param name="unitKey">New unit to convert double value into and set SelectedItem to.</param>
        /// <returns></returns>
        private object SetSelectedItemExecuted(Itemkey unitKey)
        {
            // Find the next selected item
            ListItem li = _UnitList.SingleOrDefault(i => i.Key == unitKey);

            // Convert from current item to find the next selected item
            if (li != null)
            {
                if (double.TryParse(_StrValue, out double dValue) == true)
                {
                    double tempValue = _UnitConverter.Convert(SelectedItem.Key, dValue, li.Key);

                    if (tempValue < GetMinValue(unitKey))
                        tempValue = GetMinValue(unitKey);
                    else
                      if (tempValue > GetMaxValue(unitKey))
                        tempValue = GetMaxValue(unitKey);

                    Value = tempValue;
                    _StrValue = string.Format("{0:0}", _Value);

                    SelectedItem = li;
                    ValueTip = SetUnitRangeMessage(unitKey);  // Set standard tool tip about valid range
                }

                RaisePropertyChanged(() => Value);
                RaisePropertyChanged(() => MinValue);
                RaisePropertyChanged(() => MaxValue);
                RaisePropertyChanged(() => StringValue);
                RaisePropertyChanged(() => SelectedItem);
            }

            return null;
        }

        /// <summary>
        /// Check whether the <paramref name="doubleValue"/> is within the expected
        /// range of <paramref name="unitToConvert"/> and output a corresponding
        /// error message via <paramref name="message"/> parameter if not.
        /// </summary>
        /// <param name="doubleValue"></param>
        /// <param name="unitToConvert"></param>
        /// <param name="message"></param>
        /// <returns>False if range is not acceptable, true otherwise</returns>
        private bool IsDoubleWithinRange(double doubleValue,
                                         Itemkey unitToConvert,
                                         out string message)
        {
            message = SetUnitRangeMessage(unitToConvert);

            switch (unitToConvert)
            {
                // Implement a minimum value of 2 in any way (no matter what the unit is)
                case Itemkey.ScreenFontPoints:
                    if (doubleValue < MinFontSizeValue)
                        return false;
                    else
                      if (doubleValue > MaxFontSizeValue)
                        return false;

                    return true;

                // Implement a minimum value of 2 in any way (no matter what the unit is)
                case Itemkey.ScreenPercent:
                    if (doubleValue < MinPercentageSizeValue)
                        return false;
                    else
                      if (doubleValue > MaxPercentageSizeValue)
                        return false;

                    return true;
            }

            return false;
        }

        private string SetUnitRangeMessage(Itemkey unit)
        {
            switch (unit)
            {
                // Implement a minimum value of 2 in any way (no matter what the unit is)
                case Itemkey.ScreenFontPoints:
                    return FontSizeErrorTip();

                // Implement a minimum value of 2 in any way (no matter what the unit is)
                case Itemkey.ScreenPercent:
                    return PercentSizeErrorTip();

                default:
                    throw new System.NotSupportedException(unit.ToString());
            }
        }

        /// <summary>
        /// Generate a standard font message to indicate the expected range value.
        /// </summary>
        /// <returns></returns>
        private string FontSizeErrorTip()
        {
            return string.Format(Strings.Enter_Font_Size_InRange_Message,
                                 string.Format("{0:0}", MinFontSizeValue),
                                 string.Format("{0:0}", MaxFontSizeValue));
        }

        /// <summary>
        /// Generate a standard percent message to indicate the expected range value.
        /// </summary>
        /// <returns></returns>
        private string PercentSizeErrorTip()
        {
            return string.Format(Strings.Enter_Percent_Size_InRange_Message,
                                  string.Format("{0:0}", MinPercentageSizeValue),
                                  string.Format("{0:0}", MaxPercentageSizeValue));
        }

        /// <summary>
        /// Set a tip like string to indicate the expected input format
        /// or input errors (if there are any input errors).
        /// </summary>
        /// <param name="strError"></param>
        /// <returns></returns>
        private string SetToolTip(string strError)
        {
            string standardTip = string.Format(Strings.Enter_Percent_Font_Size_InRange_Message,
                                                string.Format("{0:0}", MinPercentageSizeValue),
                                                string.Format("{0:0}", MaxPercentageSizeValue),
                                                string.Format("{0:0}", MinFontSizeValue),
                                                string.Format("{0:0}", MaxFontSizeValue));

            if (strError == null)
            {
                if (SelectedItem != null)
                {
                    switch (SelectedItem.Key)
                    {
                        case Itemkey.ScreenFontPoints:
                            ValueTip = FontSizeErrorTip();
                            break;

                        case Itemkey.ScreenPercent:
                            ValueTip = PercentSizeErrorTip();
                            break;

                        default:
                            ValueTip = standardTip;
                            break;
                    }
                }
                else
                    ValueTip = standardTip;
            }
            else
                ValueTip = strError;

            return strError;
        }

        private double GetMinValue(Itemkey key)
        {
            if (key == Itemkey.ScreenFontPoints)
                return UnitViewModel.MinFontSizeValue;

            return UnitViewModel.MinPercentageSizeValue;
        }

        private double GetMaxValue(Itemkey key)
        {
            if (key == Itemkey.ScreenFontPoints)
                return UnitViewModel.MaxFontSizeValue;

            return UnitViewModel.MaxPercentageSizeValue;
        }
        #endregion methods
    }
}
