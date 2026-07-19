using System.Collections.Concurrent;
using System.Text;

namespace Utilities
{
    public class TriggeredQueue
    {

        #region Delegates

        public delegate void ChangedEventHandler(object sender, DataTransferEventArgs e);
        public delegate void DataAddedEventHandler(object sender, DataTransferEventArgs e);
        public delegate void DataRemovedEventHandler(object sender, DataTransferEventArgs e);
        public delegate void StringAvailableEventHandler(object sender, DataTransferEventArgs e);

        #endregion

        #region Events

        public event EventHandler<DataTransferEventArgs>? ChangedEvent;
        public event DataAddedEventHandler? DataAddedEvent;
        public event DataRemovedEventHandler? DataRemovedEvent;
        public event StringAvailableEventHandler? StringAvailableEvent;

        #endregion


        #region DLL Imports

        #endregion

        #region Fields

        private readonly ConcurrentQueue<byte> _Queue = new ConcurrentQueue<byte>();
        private byte[] _BreakBytes = Array.Empty<byte>();
        private byte _PreviousByte = 0;
        private StringBuilder _Strings = new StringBuilder();
        private object _Lock = new object();

        #endregion

        #region Properties

        public int Count => _Queue.Count;

        #endregion

        #region Constructors and Destructor

        public TriggeredQueue(byte[] BreakBytes)
        {
            _BreakBytes = BreakBytes;
        }

        #endregion


        #region Private Methods

        protected virtual void OnChanged(DataTransferEventArgs e)
        {
            ChangedEvent?.Invoke(this, e);
        }

        protected virtual void OnDataAdded(DataTransferEventArgs e)
        {
            DataAddedEventHandler? Raiser = DataAddedEvent;

            if (Raiser != null)
            {
                Raiser(this, e);
            }
        }

        protected virtual void OnDataRemoved(DataTransferEventArgs e)
        {
            DataRemovedEventHandler? Raiser = DataRemovedEvent;

            if (Raiser != null)
            {
                Raiser(this, e);
            }
        }

        protected virtual void OnStringAvailable(DataTransferEventArgs e)
        {
            StringAvailableEventHandler? Raiser = StringAvailableEvent;

            if (Raiser != null)
            {
                Raiser(this, e);
            }
        }

        private string GetQueueData()
        {
            StringBuilder s = new StringBuilder();

            Debug.WriteLine("Queue length: " + _Queue.Count);

            foreach (dynamic t in _Queue)
            {
                byte[] bytes = BitConverter.GetBytes(t);

                if (bytes[0] != _BreakBytes[0] && bytes[1] != _BreakBytes[1])
                {
                    string New = System.Text.Encoding.UTF8.GetString(bytes);
                    s.Append(New);
                }

                byte X;

                _Queue.TryDequeue(out X);
            }

            return s.ToString();
        }

        #endregion

        #region Public Methods

        [STAThread]
        public virtual void Add(byte IncomingByte)
        {
            lock (_Lock)
            {
                _Strings.Append(System.Text.Encoding.ASCII.GetString(new[] { IncomingByte }).Replace(" ", ""));

                DataTransferEventArgs e = new DataTransferEventArgs();

                if (_PreviousByte == _BreakBytes[0] && IncomingByte == _BreakBytes[1])
                {
                    Debug.WriteLine("Received: '" + _Strings.Replace("\r", "").Replace("\n", "") + "'");
                }

                e.Data.Add(IncomingByte);

                OnChanged(e);
                OnDataAdded(e);

                _PreviousByte = IncomingByte;
            }
        }

        public virtual byte Remove()
        {
            byte item = default(byte);
            bool Success = _Queue.TryDequeue(out item);

            if (Success)
            {
                DataTransferEventArgs e = new DataTransferEventArgs();
                e.Data[0] = item;

                OnChanged(e);
                OnDataRemoved(e);

                return item;
            }
            else
            {
                return default(byte);
            }
        }

        #endregion

    }
}
