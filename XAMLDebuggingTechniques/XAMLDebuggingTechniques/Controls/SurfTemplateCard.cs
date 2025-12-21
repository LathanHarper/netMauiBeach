using Microsoft.Maui.Controls;

namespace XAMLDebuggingTechniques.Controls
{
    public class SurfTemplateCard : TemplatedView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title), typeof(string), typeof(SurfTemplateCard), string.Empty);

        public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(
            nameof(Subtitle), typeof(string), typeof(SurfTemplateCard), string.Empty);

        public static readonly BindableProperty IconProperty = BindableProperty.Create(
            nameof(Icon), typeof(string), typeof(SurfTemplateCard), string.Empty);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
