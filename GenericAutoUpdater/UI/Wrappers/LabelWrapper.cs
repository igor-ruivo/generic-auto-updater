namespace GenericAutoUpdater.UI.Wrappers {
    /// <summary>
    /// The class used to model the wrapper of a <c>Label</c>.
    /// </summary>
    class LabelWrapper : IWidgetWrapper {
        /// <summary>
        /// The Label's representation as an enum
        /// </summary>
        public ProgressiveWidgetsEnum.Label Label { get; }

        /// <summary>
        /// The text value of the Label.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <c>LabelWrapper</c> class with the specified <c>ProgressiveWidgetsEnum.Label</c> and value.
        /// </summary>
        public LabelWrapper(ProgressiveWidgetsEnum.Label label, string value) {
            Label = label;
            Value = value;
        }
    }
}
