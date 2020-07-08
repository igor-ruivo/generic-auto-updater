using System;

namespace M2BobPatcher.ExceptionHandler.Exceptions {

    [Serializable]
    public class DataTamperedException : Exception {

        public DataTamperedException() {

        }

        public DataTamperedException(string message) : base(message) {

        }

        public DataTamperedException(string message, Exception innerException) : base(message, innerException) {

        }
    }
}