namespace XAMLDebuggingTechniques.Views
{
    public partial class BindingContextLostPage : ContentPage
    {
        public BindingContextLostPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var Fred = sender as Button;
            if (Fred != null)
            {
                var bc = Fred.BindingContext;
                var BC = Fred.BindingContext as ViewModels.BindingContextLostPageViewModel;
            }
        }
    }
}
