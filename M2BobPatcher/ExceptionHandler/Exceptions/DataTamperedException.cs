using System;

namespace M2BobPatcher.ExceptionHandler.Exceptions {
    /// <summary>
    /// The exception that is thrown whenever an external entity tampered any data it shouldn't.
    /// </summary>
    [Serializable]
    public class DataTamperedException : Exception {
        /// <summary>
        /// Initializes a new instance of the <c>DataTamperedException</c> class.
        /// </summary>
        public DataTamperedException() {

        }

        /// <summary>
        /// Initializes a new instance of the <c>DataTamperedException</c> class with a specified error message.
        /// </summary>
        public DataTamperedException(string message) : base(message) {

        }

        /// <summary>
        /// Initializes a new instance of the <c>DataTamperedException</c> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public DataTamperedException(string message, Exception innerException) : base(message, innerException) {

        }
    }
}