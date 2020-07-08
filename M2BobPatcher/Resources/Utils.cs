using System;
using System.IO;
using System.Text;

namespace M2BobPatcher.Resources {
    static class Utils {

        public static string PerformPatchDirectorySanityCheck(byte[] data) {
            Uri uriResult;
            string serverMetadata = Encoding.Default.GetString(data);
            string patchDirectory = serverMetadata.Trim().Split(new[] { "\n" }, StringSplitOptions.None)[0];
            if (!Uri.TryCreate(patchDirectory, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                throw new InvalidDataException();
            return Encoding.Default.GetString(data);
        }

        public static bool AggregateContainsObjectDisposedException(AggregateException ex) {
            foreach (Exception exception in ex.InnerExceptions)
                if (exception is ObjectDisposedException)
                    return true;
            return false;
        }
    }
}
