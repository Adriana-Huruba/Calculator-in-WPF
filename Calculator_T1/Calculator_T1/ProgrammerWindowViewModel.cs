using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Windows.Input;

namespace Calculator_T1
{
    public class ProgrammerWindowViewModel : INotifyPropertyChanged
    {
        private string _displayText;
        private string _currentBase = "DEC";  // baza default este DEC
        private string _hexValue;
        private string _decValue;
        private string _octValue;
        private string _binValue;

        public ICommand DigitCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand EqualCommand { get; }
        public ICommand ChangeBaseCommand { get; }
        public ICommand SwitchToStandardCommand { get; }

        private string _currentOperation;
        private double _previousValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProgrammerWindowViewModel()
        {
            DigitCommand = new UICommand(AddDigit);
            OperatorCommand = new UICommand(PerformOperation);

            ClearCommand = new UICommand(_ => Clear());
            ClearEntryCommand = new UICommand(_ => ClearEntry());
            BackspaceCommand = new UICommand(_ => Backspace());
            EqualCommand = new UICommand(_ => CalculateResult());
            ChangeBaseCommand = new UICommand(ChangeBase);
            DisplayText = "0";  // Set initial value for the display

            SwitchToStandardCommand = new UICommand(_ => OpenStandardWindow());
        }

        private void OpenStandardWindow()
        {
            MainWindow standardWindow = new MainWindow();
            standardWindow.Show();

            // Close the current window
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window is ProgrammerWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
        private void AddDigit(object parameter)
        {
            string digit = parameter as string; // Cast the object to string
            if (digit != null)
            {
                if (DisplayText == "0")
                    DisplayText = digit;
                else
                    DisplayText += digit;
            }
        }

        private void PerformOperation(object parameter)
        {
            string operation = parameter as string; // Cast the object to string
            if (operation != null)
            {
                _currentOperation = operation;
                double.TryParse(DisplayText, out _previousValue);
                DisplayText = "0"; // Reset display for next input
            }
        }
        private void CalculateResult()
        {
            if (_currentOperation != null)
            {
                double currentValue;
                if (double.TryParse(DisplayText, out currentValue))
                {
                    double result = 0;
                    switch (_currentOperation)
                    {
                        case "+":
                            result = _previousValue + currentValue;
                            break;
                        case "-":
                            result = _previousValue - currentValue;
                            break;
                        case "*":
                            result = _previousValue * currentValue;
                            break;
                        case "/":
                            if (currentValue != 0)
                                result = _previousValue / currentValue;
                            else
                                DisplayText = "Error";
                            break;
                    }

                    if (DisplayText != "Error")
                        DisplayText = result.ToString();

                    _currentOperation = null;
                }
            }
        }
        private void Clear()
        {
            DisplayText = "0";
            _currentOperation = null;
            _previousValue = 0;
        }

        private void ClearEntry()
        {
            DisplayText = "0";
        }

        private void Backspace()
        {
            if (DisplayText.Length > 1)
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            else
                DisplayText = "0";
        }

        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                _displayText = value;
                OnPropertyChanged(nameof(DisplayText));
                UpdateDisplayText();
            }
        }

        public string HexValue
        {
            get { return _hexValue; }
            set
            {
                _hexValue = value;
                OnPropertyChanged(nameof(HexValue));
            }
        }

        public string DecValue
        {
            get { return _decValue; }
            set
            {
                _decValue = value;
                OnPropertyChanged(nameof(DecValue));
            }
        }

        public string OctValue
        {
            get { return _octValue; }
            set
            {
                _octValue = value;
                OnPropertyChanged(nameof(OctValue));
            }
        }

        public string BinValue
        {
            get { return _binValue; }
            set
            {
                _binValue = value;
                OnPropertyChanged(nameof(BinValue));
            }
        }
        private void ChangeBase(object parameter)
        {
            string newBase = parameter as string; // Cast the object to string
            if (!string.IsNullOrEmpty(newBase))
            {
                _currentBase = newBase;
                UpdateDisplayText();
            }
        }

        private void UpdateDisplayText()
        {
            if (!int.TryParse(DisplayText, out int value))
            {
                HexValue = DecValue = OctValue = BinValue = string.Empty;
                return;
            }

            HexValue = Convert.ToString(value, 16).ToUpper();
            DecValue = value.ToString();
            OctValue = Convert.ToString(value, 8);
            BinValue = Convert.ToString(value, 2);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
