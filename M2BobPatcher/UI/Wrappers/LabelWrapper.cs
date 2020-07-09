namespace M2BobPatcher.UI.Wrappers {
    class LabelWrapper : IWidgetWrapper {

        public ProgressiveWidgetsEnum.Label Label { get; }

        public string Value { get; }

        public LabelWrapper(ProgressiveWidgetsEnum.Label label, string value) {
            Label = label;
            Value = value;
        }
    }
}
