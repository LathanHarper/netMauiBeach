using Microsoft.Maui.Controls;
using XAMLDebuggingTechniques.Infrastructure.Interfaces;
using XAMLDebuggingTechniques.ViewModels;

namespace XAMLDebuggingTechniques.Selectors
{
    public class TelemetryTileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? LineTemplate { get; set; }
        public DataTemplate? GaugeTemplate { get; set; }
        public DataTemplate? BadgeTemplate { get; set; }

       // public ITemplateRecorder? Recorder { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is TelemetryTile tile)
            {
                var templateName = tile.TileType switch
                {
                    TelemetryTileType.Line => "Line",
                    TelemetryTileType.Gauge => "Gauge",
                    TelemetryTileType.Badge => "Badge",
                    _ => "Line"
                };

               // Recorder?.RecordTemplate(templateName);

                return templateName switch
                {
                    "Line" => LineTemplate ?? GaugeTemplate ?? BadgeTemplate ?? new DataTemplate(),
                    "Gauge" => GaugeTemplate ?? LineTemplate ?? BadgeTemplate ?? new DataTemplate(),
                    "Badge" => BadgeTemplate ?? LineTemplate ?? GaugeTemplate ?? new DataTemplate(),
                    _ => LineTemplate ?? new DataTemplate()
                };
            }

            return LineTemplate;
        }
    }
}
