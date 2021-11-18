namespace Solcery.Services.Resources.Patterns.Widgets
{
    public sealed class PatternWidgetData : PatternData
    {
        public readonly string WidgetResourcePath;

        public static PatternWidgetData Create(string widgetResourcePath)
        {
            return new PatternWidgetData(widgetResourcePath);
        }

        private PatternWidgetData(string widgetResourcePath)
        {
            WidgetResourcePath = widgetResourcePath;
        }
        
        public override void Cleanup() { }
    }
}