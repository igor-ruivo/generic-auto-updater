namespace GenericAutoUpdater.Engine {
    /// <summary>
    /// This interface represents the engine with all its core logic behind the Auto-Updater.
    /// It contains the public API calls that an external application may access regarding it.
    /// </summary>
    public interface IPatcherEngine {
        /// <summary>
        /// Performs the required steps in order to try to fully patch the client.
        /// </summary>
        void Patch();
    }
}