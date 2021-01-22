using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace AsyncPrimeNumberFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region WPF Numeric Up Down
        //Source: https://stackoverflow.com/a/2752538/11776868

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as Button).Tag as TextBox;
            refInt number = tb.Tag as refInt;
            number.Val += 1;
            tb.Text = (number.Val).ToString();
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as Button).Tag as TextBox;
            if (tb.Text != "1")// cannot go bellow 1
            {
                refInt number = tb.Tag as refInt;
                number.Val -= 1;
                tb.Text = (number.Val).ToString();
            }
        }

        class refInt
        {
            public int Val { get; set; }
        }
        refInt numUpDownNumber1 = new refInt();
        refInt numUpDownNumber2 = new refInt();
        refInt numUpDownNumber3 = new refInt();
        Thread[] workedThreads = new Thread[3];
        private void UpdateNumericUpDowns()
        {
            numUpDownNumber1.Val = Convert.ToInt32(txtNum1.Text);
            numUpDownNumber2.Val = Convert.ToInt32(txtNum2.Text);
            numUpDownNumber3.Val = Convert.ToInt32(txtNum3.Text);

        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            cmdUp1.Tag = txtNum1;
            cmdDown1.Tag = txtNum1;
            cmdUp2.Tag = txtNum2;
            cmdDown2.Tag = txtNum2;
            cmdUp3.Tag = txtNum3;
            cmdDown3.Tag = txtNum3;

            txtNum1.Tag = numUpDownNumber1;
            txtNum2.Tag = numUpDownNumber2;
            txtNum3.Tag = numUpDownNumber3;

            UpdateNumericUpDowns();
        }
        bool isFinished1 = true;
        bool isFinished2 = true;
        bool isFinished3 = true;
        private bool IsNumberPrime()
        {
            int n, i, m = 0;
            Console.Write("Enter the Number to check Prime: ");
            n = numUpDownNumber1.Val;
            m = n / 2;
            for (i = 2; i <= m; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool isPrime(int number)
        {

            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        delegate void CheckMethod();
        private void Method1()
        {
            bool b = IsNumberPrime();

            Dispatcher.Invoke(new Action(() =>
            {
                if (b)
                {
                    resultLabel1.Content = "This is a prime number";
                }
                else
                {
                    resultLabel1.Content = "This is NOT a prime number";
                }
                isFinished1 = true;
                Finished();
            }));
        }
        private void Method2()
        {
            int n = numUpDownNumber2.Val;
            int i = 1;
            while (true)
            {
                if (isPrime(i))
                {
                    string s = i.ToString();
                    int streak = 0;
                    int flag = 0;
                    for (int k = 0; k < s.Length; k++)
                    {
                        if (s[k] == '1')
                            streak++;
                        else
                            streak = 0;
                        if (streak == n)
                            flag = 1;

                    }
                    if (flag == 1)
                    {
                        break;
                    }
                }
                i++;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                resultLabel2.Content = $"The number is {i}";
                isFinished2 = true;
                Finished();
            }));
        }
        private void Method3()
        {
            string n_string = numUpDownNumber3.Val.ToString();
            char n_lastChar = n_string[n_string.Length - 1];
            bool doesExist = false;
            int i = Convert.ToInt32(Math.Pow(10, n_string.Length - 1)); // skippne začátek npř "23" začne až od 11 | numbers of digits gets converted to ... 1 > 1| 2 > 10| 3 > 100
            if (!(n_lastChar == '2' && n_string.Length > 1
                || n_lastChar == '4'
                || n_lastChar == '5' && n_string.Length > 1
                || n_lastChar == '6'
                || n_lastChar == '8'
                || n_lastChar == '0'))
            {
                while (true)
                {
                    if (isPrime(i))
                    {
                        string s = i.ToString();
                        int flag = 0;
                        int streak = 0;
                        for (int k = 0; k < n_string.Length; k++)
                        {
                            if (s[s.Length - n_string.Length + k] == n_string[k])
                                streak++;
                            else
                                break;
                            if (streak == n_string.Length)
                                flag = 1;

                        }
                        if (flag == 1)
                        {
                            break;
                        }
                    }
                    i++;
                }

                doesExist = true;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                if (doesExist)
                {
                    resultLabel3.Content = $"The number is {i}";
                }
                else 
                { 
                    resultLabel3.Content = $"There is no prime number ending with {n_string}";
                }
                isFinished3 = true;
                Finished();
            }));

        }
        private void Finished()
        {
            if (isFinished1 && isFinished2 && isFinished3)
            {
                startButton.IsEnabled = true;
                cancelButton.IsEnabled = false;
            }
        }

        public static List<int> GetAllPrimes(int number)
        {
            List<int> l = new List<int>();
            for (int i = 0; i < number; i++)
            {
                if (isPrime(i)) l.Add(i);
            }
            return l;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckMethod[] checkMethods = new CheckMethod[] { Method1, Method2, Method3 };
            if (checkBox1.IsChecked ?? false)
            {
                resultLabel1.Content = "Loading...";
                 Thread t = new Thread(() => checkMethods[0]());
                workedThreads[0] = t;
                isFinished1 = false;
                t.Start();

            }
            if (checkBox2.IsChecked ?? false)
            {
                resultLabel2.Content = "Loading...";
                Thread t = new Thread(() => checkMethods[1]());
                workedThreads[1] = t;
                isFinished2 = false;
                t.Start();
            }

            if (checkBox3.IsChecked ?? false)
            {
                resultLabel3.Content = "Loading...";
                Thread t = new Thread(() => checkMethods[2]());
                workedThreads[2] = t;
                isFinished3 = false;
                t.Start();
            }
            startButton.IsEnabled = false;
            cancelButton.IsEnabled = true;
        }


        private void txtNum_LostFocus(object sender, RoutedEventArgs e)
        {

            TextBox tb = sender as TextBox;
            (tb.Tag as refInt).Val = Convert.ToInt32(tb.Text);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Thread t in workedThreads)
            {
                t?.Abort();
            }
            isFinished1 = true;
            isFinished2 = true;
            isFinished3 = true;
            resultLabel1.Content = "";
            resultLabel2.Content = "";
            resultLabel3.Content = "";
            Finished();

        }
    }
}
