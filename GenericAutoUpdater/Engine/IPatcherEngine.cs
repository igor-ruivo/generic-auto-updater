namespace M2BobPatcher.Engine {
    /// <summary>
    /// This interface represents the engine with all its core logic behind the AutoPatcher.
    /// It contains the public API calls that an external application may access regarding it.
    /// </summary>
    public interface IPatcherEngine {
        /// <summary>
        /// Performs the required steps in order to try to fully patch M2Bob.
        /// </summary>
        void Patch();
    }
}