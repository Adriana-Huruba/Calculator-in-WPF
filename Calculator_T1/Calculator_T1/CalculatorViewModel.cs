using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Calculator_T1
{
    public class CalculatorViewModel :INotifyPropertyChanged
    {
        private string displayText = "0";
        private double _currentValue;
        private string _currentOperator;
        private bool _isNewEntry = true;
        public string DisplayText
        {  
            get { return displayText; }
            set { displayText = value; OnPropertyChanged(nameof(DisplayText)); }
        }

        public ICommand DigitCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand EqualCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand ClearEntryCommand { get; }

        public ICommand MemoryClearCommand { get; }
        public ICommand MemoryRecallCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemorySubtractCommand { get; }
        public ICommand MemoryStoreCommand { get; }
        public ICommand ShowMemoryStackCommand { get; }

        //pt schimbarea modului
        public ICommand SwitchToStandardCommand { get; }
        public ICommand SwitchToProgrammerCommand { get; }

        public ICommand KeyPressCommand { get; }

        public CalculatorViewModel()
        {
            DigitCommand = new UICommand(param => AppendDigit(param.ToString()));
            OperatorCommand = new UICommand(param => ApplyOperator(param.ToString()));
            EqualCommand = new UICommand(_ => Calculate());
            ClearCommand = new UICommand(_ => Clear());
            ClearEntryCommand = new UICommand(_ => ClearEntry());
            BackspaceCommand = new UICommand(_ => Backspace());

            MemoryClearCommand = new UICommand(_ => MemoryClear());
            MemoryRecallCommand = new UICommand(_ => MemoryRecall());
            MemoryAddCommand = new UICommand(_ => MemoryAdd());
            MemorySubtractCommand = new UICommand(_ => MemorySubtract());
            MemoryStoreCommand = new UICommand(_ => MemoryStore());
            ShowMemoryStackCommand = new UICommand(_ => ShowMemoryStack());

            SwitchToStandardCommand = new UICommand(_ => OpenStandardWindow());
            SwitchToProgrammerCommand = new UICommand(_ => OpenProgrammerWindow());
            //KeyPressCommand = new UICommand<KeyEventArgs>(HandleKeyPress);
            KeyPressCommand = new UICommand(param => HandleKeyPress(param as KeyEventArgs));
        }
        private void HandleKeyPress(KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Calculate();
            }
            else if (e.Key == Key.Back)
            {
                Backspace();
            }
            else if (e.Key == Key.Escape)
            {
                Clear();
            }
            // Handle operators
            else if (e.Key == Key.Add || e.Key == Key.OemPlus)
            {
                ApplyOperator("+");
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                ApplyOperator("-");
            }
            else if (e.Key == Key.Multiply)
            {
                ApplyOperator("*");
            }
            else if (e.Key == Key.Divide)
            {
                ApplyOperator("/");
            }
            
            else if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                AppendDigit(".");
            }
            
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                char digit = e.Key.ToString().Last();
                AppendDigit(digit.ToString());
            }
            
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                int digit = e.Key - Key.NumPad0;
                AppendDigit(digit.ToString());
            }
        }

        private void OpenStandardWindow()
        {
            MainWindow standardWindow = new MainWindow();
            standardWindow.Show();
            CloseCurrentWindow();
        }

        private void OpenProgrammerWindow()
        {
            ProgrammerWindow programmerWindow = new ProgrammerWindow();
            programmerWindow.Show();
            CloseCurrentWindow();
        }
        private void CloseCurrentWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow || window is ProgrammerWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void AppendDigit(string digit)
        {
            if (digit == "." && DisplayText.Contains("."))
                return;
            if (DisplayText == "0" && digit != ".")
                DisplayText = digit;
            else
                 DisplayText += digit;
        }

        private void ApplyOperator(string op)
        {
            double value = double.Parse(DisplayText);

            //logica pentru operatori unari
            switch(op)
            {
                case "%":
                    DisplayText = (_currentValue * value / 100).ToString();
                    return;
                case "1/x":
                    DisplayText = value != 0 ? (1 / value).ToString() : "Eroare";
                    return;
                case "x²":
                    DisplayText = Math.Pow(value, 2).ToString();
                    return;
                case "√":
                    DisplayText = value >= 0 ? Math.Sqrt(value).ToString() : "Eroare";
                    return;
                case "+/-":
                    DisplayText = (-value).ToString();
                    return;
            }
            
             // logica pentru operatori binari
             if (_currentOperator != null )
                Calculate();
            _currentOperator = op;
            _currentValue=double.Parse(DisplayText);
            DisplayText = "0";
        }

        private void Calculate()
        {
            if (_currentOperator == null)
                return;
            double result = 0;
            double secondValue = double.Parse(DisplayText);
            switch (_currentOperator)
            {
                case "+":
                    result = _currentValue + secondValue;
                    break;
                case "-":
                    result = _currentValue - secondValue;
                    break;
                case "*":
                    result = _currentValue * secondValue;
                    break;
                case "/": 
                    if (secondValue != 0)
                        result = _currentValue / secondValue;
                    else
                    { 
                        DisplayText = "Eroare";
                        return;
                    }
                    break;
                default: 
                    break;
            }
            DisplayText=result.ToString();
            _currentOperator = null;
        }

        private void Clear() => DisplayText = "0";
        private void ClearEntry() => DisplayText = "0";
        private void Backspace() => DisplayText = DisplayText.Length > 1 ? DisplayText[..^1] : "0";

        private Stack<double> _memorystack=new Stack<double>();
        private void MemoryClear()
        {
            _memorystack.Clear();
        }
        private void MemoryRecall() //reafisarea val mem pe ecran
        { 
            if (_memorystack.Count > 0)
                DisplayText = _memorystack.Peek().ToString();
            else
                DisplayText = "0";
        }
        private void MemoryAdd() //adaug in mem
        {
             _memorystack.Push(double.Parse(DisplayText)); 
        }
        private void MemorySubtract() 
        {
            if (_memorystack.Count > 0)
                _memorystack.Pop();
        }
        private void MemoryStore() 
        {
            _memorystack.Push(double.Parse(DisplayText));
        }
        private void ShowMemoryStack()
        {

            if (_memorystack.Count > 0)
            {
                string memoryContents = string.Join("\n", _memorystack.Reverse());
                MessageBox.Show($"Memorie:\n{memoryContents}", "Memorie Stocată", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Memoria este goală!", "Memorie", MessageBoxButton.OK, MessageBoxImage.Warning);

        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
