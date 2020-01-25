using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditorPage : ContentPage
    {
        public string text { private set; get; }
        public EditorPage()
        {
            InitializeComponent();
            text = null;
            TextEditor.Focus();
        }
        Label label;
        public EditorPage(string x)
        {
            label = new Label
            {
                Text = $"Symbols :{x.Count()}"
            };
            NavigationPage.SetTitleView(this, label);
            InitializeComponent();
            TextEditor.Text = x;
            text = x;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            text = TextEditor.Text;
            Navigation.PopAsync() ;
        }

        private void TextEditor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            text = (sender as Editor).Text;
            if (text != null)
            {
                label.Text = $"Symbols :{text.Count()}";
            }
        }
    }
}