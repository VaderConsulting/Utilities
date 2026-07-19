using System.Collections;

namespace Utilities
{
    [Serializable]
    public class StringToObjectMapping<T>
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

        private string _SourceValue = "";
        private int _SourceIndex = -1;
        private T? _DestinationObject = default(T);
        private Transform.MethodType _TransformMethodType = Transform.MethodType.None;
        private string[] _Parameters;

        #endregion

        #region Properties

        public string SourceValue
        {
            get => _SourceValue;
            set => _SourceValue = value;
        }

        public int SourceIndex
        {
            get => _SourceIndex;
            set => _SourceIndex = value;
        }

        public T? DestinationObject
        {
            get => _DestinationObject;
            set => _DestinationObject = value;
        }

        public Transform.MethodType TransformMethodType
        {
            get => _TransformMethodType;
            set => _TransformMethodType = value;
        }

        public string[] Parameters
        {
            get => _Parameters;
            set => _Parameters = value;
        }

        #endregion

        #region Constructors and Destructor

        public StringToObjectMapping(string SourceValue, int SourceIndex, T DestinationObject, Transform.MethodType TransformType, params string[] Parameters)
        {
            _SourceValue = SourceValue;
            _SourceIndex = SourceIndex;
            _DestinationObject = DestinationObject;
            _TransformMethodType = TransformType;
            _Parameters = Parameters;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_SourceValue).GetEnumerator();
        }

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{_SourceValue} -> {_DestinationObject}";
        }

        #endregion

        #region Classes

        // By my own convention all classes should be in their own file, however sometimes it makes sense to include a class within the same file as it's parent Namespace

        #endregion
    }
}
