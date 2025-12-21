using Prism;
using Prism.Ioc;

namespace XAMLDebuggingTechniques
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        // Prism handles app startup and navigation via UsePrism(OnAppStart)
        // so we don't override CreateWindow or set MainPage here.
    }
}