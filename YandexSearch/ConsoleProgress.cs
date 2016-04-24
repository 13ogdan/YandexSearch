using System;
using System.Threading.Tasks;

namespace YandexSearch
{
    public class ConsoleProgress : IDisposable
    {
        private const int _progressBarLength = 20;
        private readonly string _operationName;
        private bool _isIndeterminate;
        private bool _isFinish;
        private byte _currentProgressPosition;

        private int _max = 100;
        private int _min = 0;
        private int _progress = 0;

        public ConsoleProgress(string operationName)
        {
            _operationName = operationName;
            _isIndeterminate = true;
            Start();
        }

        public async void Start()
        {
            Console.WriteLine("Operation {0} in progress:", _operationName);
            if (_isIndeterminate)
            {
                for (int i = 0; i < _progressBarLength && !_isFinish && _isIndeterminate; i++)
                {
                    await Task.Delay(100);
                    if (!_isFinish)
                    {
                        if (i == _progressBarLength - 1)
                        {
                            ClearCurrentLine();
                            i = 0;
                        }
                        Console.Write("☺");
                    }

                }
            }
            else
            {
                while (_isFinish && _progress < _max)
                {
                    await Task.Delay(100);
                    RefreshProgressBarView();
                }
            }

        }

        public void SetRange(int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(min), min, "Should be less than max");

            _min = min;
            _max = max;
            _progress = _min;
            _isIndeterminate = false;
            RefreshProgressBarView();
        }

        public void SetProgress(int value)
        {
            if (value > _max)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Should be less than max");
            if (value < _min)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Should be greater than max");

            _isIndeterminate = false;
            _progress = (value);
            RefreshProgressBarView();
        }

        private void RefreshProgressBarView()
        {
            if (_isIndeterminate)
            {
                if (_currentProgressPosition == _progressBarLength - 1)
                {
                    ClearCurrentLine();
                    _currentProgressPosition = 0;
                }
                _currentProgressPosition++;
                Console.Write("☺");
            }
            else
            {
                ClearCurrentLine();
                var progressValue = (((double)_progress) / (_max - _min));
                var position = Math.Floor(progressValue * _progressBarLength);

                for (int i = 0; i < _progressBarLength; i++)
                {
                    Console.Write(i <= position ? "☺" : " ");
                }
                var ceiling = Math.Floor(progressValue * 100);
                Console.Write($" {ceiling}%");
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _isFinish = true;
            ClearCurrentLine();
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            ClearCurrentLine();
        }

        private static void ClearCurrentLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}