using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorApp
{
    delegate void DisplayMessage(string text);

    class BusinessLogic
    {
        DisplayMessage displayMessage;

        public BusinessLogic(DisplayMessage displayMessage)
        {
            this.displayMessage = displayMessage;
        }

        string[] nonZeroDigit = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        string[] digit = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        string[] remove = { "⌫", "C", "CE" };
        string[] zero = { "0" };
        string[] separator = { "," };
        string[] operation = { "+", "-", "×", "÷", "x²", "√x", "1/x", "+/-" };
        string[] compute = { "=", "%" };

        enum State
        {
            Zero,
            AccumulateDigits,
            AccumulateDecimalDigits,
            ComputePending,
            Compute
        }

        State currentState = State.Zero;
        string previousNumber = "";
        string currentNumber = "";
        string currentOperation = "";

        public void ProcessSignal(string message)
        {
            switch (currentState)
            {
                case State.Zero:
                    ProcessZeroState(message, false);
                    break;
                case State.AccumulateDigits:
                    ProcessAccumulateDigits(message, false);
                    break;
                case State.AccumulateDecimalDigits:
                    ProcessAccumulateDigitsDecimal(message, false);
                    break;
                case State.ComputePending:
                    ProcessComputePending(message, false);
                    break;
                case State.Compute:
                    ProcessCompute(message, false);
                    break;
                default:
                    break;
            }
        }

        private void ProcessZeroState(string msg, bool income)
        {
            if (income)
                currentState = State.Zero;
            else
            {
                if (nonZeroDigit.Contains(msg))
                    ProcessAccumulateDigits(msg, true);
                else if (separator.Contains(msg))
                    ProcessAccumulateDigitsDecimal(msg, true);
            }
        }

        private void ProcessAccumulateDigits(string msg, bool income)
        {
            if (income)
            {
                currentState = State.AccumulateDigits;
                if (zero.Contains(currentNumber) && !remove.Contains(msg))
                    currentNumber = msg;
                else if (msg == "⌫" && currentNumber.Length > 0)
                {
                    currentNumber = currentNumber.Remove(currentNumber.Length - 1);
                    if (currentNumber.Length == 0)
                        currentNumber = "0";
                }
                else if (remove.Contains(msg))
                {
                    if (msg == "C")
                        previousNumber = "0";
                    currentNumber = "0";
                }
             
                else
                    currentNumber += msg;

                displayMessage(currentNumber);
            }
            else
            {
                if (digit.Contains(msg) || remove.Contains(msg))
                    ProcessAccumulateDigits(msg, true);
                else if (separator.Contains(msg))
                    ProcessAccumulateDigitsDecimal(msg, true);
                else if (operation.Contains(msg))
                    ProcessComputePending(msg, true);
                else if (compute.Contains(msg))
                    ProcessCompute(msg, true);

            }
        }

        private void ProcessAccumulateDigitsDecimal(string msg, bool income)
        {
            if (income)
            {
                currentState = State.AccumulateDecimalDigits;
                if (zero.Contains(currentNumber) || currentNumber == "")
                    currentNumber = "0" + msg;
                else if (!currentNumber.Contains(msg))
                    currentNumber += msg;
                displayMessage(currentNumber);
            }

            else
            {
                if (digit.Contains(msg) || remove.Contains(msg))
                    ProcessAccumulateDigits(msg, true);
                else if (operation.Contains(msg))
                    ProcessComputePending(msg, true);
                else if (compute.Contains(msg))
                    ProcessCompute(msg, true);
            }
        }

        private void ProcessComputePending(string msg, bool income)
        {
            if (income)
            {

                if (msg == "√x" || msg == "1/x" || msg == "x²" || msg == "+/-")
                    ProcessCompute(msg, true);

                else
                {
                    currentState = State.ComputePending;
                    previousNumber = currentNumber;
                    currentNumber = "";
                    currentOperation = msg;
                }

            }
            else
            {
                if (digit.Contains(msg))
                    ProcessAccumulateDigits(msg, true);
            }
        }

        private void ProcessCompute(string msg, bool income)
        {
            if (income)
            {
                currentState = State.Compute;
                double a = 0;
                if (previousNumber.Length > 0)
                    a = double.Parse(previousNumber);
                double b = double.Parse(currentNumber);

                if (msg == "=")
                {
                    if (currentOperation == "+")
                        currentNumber = (a + b).ToString();
                    else if (currentOperation == "-")
                        currentNumber = (a - b).ToString();
                    else if (currentOperation == "×")
                        currentNumber = (a * b).ToString();
                    else if (currentOperation == "÷")
                        currentNumber = (a / b).ToString();
                }
                else if (msg == "%")
                    currentNumber = (a - (a * (b / 100))).ToString();
                else
                {
                    if (msg == "√x")
                        currentNumber = Math.Sqrt(b).ToString();
                    else if (msg == "x²")
                        currentNumber = Math.Pow(b, 2).ToString();
                    else if (msg == "1/x")
                        currentNumber = (1 / b).ToString();
                    else if (msg == "+/-")
                        currentNumber = (-1 * b).ToString();
                }
                displayMessage(currentNumber);

            }
            else
            {
                if (zero.Contains(msg))
                    ProcessZeroState(msg, true);
                else if (nonZeroDigit.Contains(msg) || remove.Contains(msg))
                    ProcessAccumulateDigits(msg, true);
                else if (operation.Contains(msg))
                    ProcessComputePending(msg, true);
                else if (compute.Contains(msg))
                    ProcessCompute(msg, true);

            }
        }
    }
}
