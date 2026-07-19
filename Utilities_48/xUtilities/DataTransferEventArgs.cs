using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class DataTransferEventArgs
    {
        #region Constants

        #endregion

        #region Delegates

        #endregion

        #region Events

        #endregion

        #region Enums

        #endregion

        #region DLL Imports

        #endregion

        #region Fields

        private List<object> _Data = new List<object>();

        #endregion

        #region Properties

        public List<object> Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }

        public string StringData
        {
            get
            {
                StringBuilder s = new StringBuilder();

                foreach (dynamic t in _Data)
                {
                    byte[] bytes = BitConverter.GetBytes(t);

                    s.Append(System.Text.Encoding.UTF8.GetString(bytes));
                }

                return s.ToString();

            }
        }

        #endregion

        #region Constructors and Destructor

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        #endregion

    }
}
