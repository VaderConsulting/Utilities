using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Reflection;

namespace Utilities
{
    public class Serial : IDisposable
    {

        #region Constants

        #endregion

        #region Delegates

        //public delegate void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e);
        public delegate void DataReceivedHandler(object sender, DataTransferEventArgs e);
        //public delegate void PinChangedHandler(object sender, SerialPinChangedEventHandler e);
        //public delegate void DisposedHandler();

        #endregion

        #region Events

        //public event ErrorReceivedHandler ErrorReceived;
        public event DataReceivedHandler DataReceived;
        //public event PinChangedHandler PinChanged;
        //public event DisposedHandler Disposed;

        #endregion

        #region Enums

        #endregion

        #region DLL Imports

        #endregion

        #region Fields

        private short _BufferSize = 10;
        private byte[] _Buffer = null;
        private Action _ReadData = null;
        private System.IO.Ports.SerialPort _Port = new System.IO.Ports.SerialPort();
        private TriggeredQueue _InternalQueue = new TriggeredQueue(new byte[] { 13, 10 });

        #endregion

        #region Properties

        #endregion

        #region Constructors and Destructor

        public Serial()
        {

        }

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        private void HandleSerialError(IOException exc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read Serial Data.  Adapted from: http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
        /// </summary>
        /// <param name="PortName">The COM Port to open</param>
        /// <param name="BufferSize">The buffer size to use</param>
        public void Start(string PortName, short BufferSize)
        {
            _BufferSize = BufferSize;
            _Buffer = new byte[_BufferSize];
            _Port.PortName = PortName;

            try
            {
                _Port.Open();

                _ReadData = delegate
                {
                    _Port.BaseStream.BeginRead(_Buffer,
                                                0,
                                                _Buffer.Length,
                                                delegate (IAsyncResult ar)
                                                {
                                                    try
                                                    {
                                                        int actualLength = _Port.BaseStream.EndRead(ar);
                                                        byte[] ReceivedBytes = new byte[actualLength];                                                       
                                                        DataTransferEventArgs Args = new DataTransferEventArgs();

                                                        Buffer.BlockCopy(_Buffer, 0, ReceivedBytes, 0, actualLength);

                                                        foreach (byte DataByte in ReceivedBytes)
                                                        {
                                                            _InternalQueue.Add(DataByte);
                                                        }

                                                        //Args.Data = (object)ReceivedBytes;

                                                        //RaiseSerialDataReceived(Args);
                                                    }
                                                    catch (IOException exc)
                                                    {
                                                        HandleSerialError(exc);
                                                    }

                                                    _ReadData();

                                                }, null);
                };

                _ReadData();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //public virtual void RaiseSerialErrorReceived(SerialErrorReceivedEventArgs e)
        //{
        //    ErrorReceivedHandler Raiser = ErrorReceived;

        //    if (Raiser != null)
        //    {
        //        Raiser(this, e);
        //    }
        //}

        public virtual void RaiseSerialDataReceived(DataTransferEventArgs e)
        {
            DataReceivedHandler Raiser = DataReceived;

            if (Raiser != null)
            {
                Raiser(this, e);
            }
        }

        //public virtual void RaiseSerialPinChanged(SerialPinChangedEventHandler e)
        //{
        //    PinChangedHandler Raiser = PinChanged;

        //    if (Raiser != null)
        //    {
        //        Raiser(this, e);
        //    }
        //}

        //public virtual void RaiseDisposed()
        //{
        //    DisposedHandler Raiser = Disposed;

        //    if (Raiser != null)
        //    {
        //        Raiser();
        //    }
        //}

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Serial() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion


        // ************** TODO:  Add data to the Triggered Queue.  When complete messages are received, raise an event for each message
    }
}
