using App2.Components;
using Plugin.Share;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App2
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public int counterLeft = 0, counterRight = 0;

        public void add(string text, bool right_, string path)
        {
            int size = (text.Length >= 10) ? 10 : text.Length;
            Note frame = new Note
            {
                BorderColor = Color.Aqua,
                InnerText = text,
                Path = path
            };
            Label label = new Label();
            label.Text = text.Substring(0, size);
            label.BackgroundColor = Color.Beige;
            label.LineBreakMode = LineBreakMode.WordWrap;
            frame.Content = label;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (tapsender, tape) =>
            {
                EditorPage editor2 = new EditorPage(frame.InnerText);
                editor2.Disappearing += (a, b) =>
                {
                    frame.InnerText = editor2.text;
                    label.Text = editor2.text.Substring(0,10);
                };
                Navigation.PushAsync(editor2);
            };
            label.GestureRecognizers.Add(tapGestureRecognizer);
            var pan = new PanGestureRecognizer();
            double totalX = 0;
            pan.PanUpdated += async (panSender, panArgs) =>
            {                
                switch (panArgs.StatusType)
                {
                    case GestureStatus.Canceled:
                    case GestureStatus.Started:
                        frame.TranslationX = 0;
                        break;
                    case GestureStatus.Completed:
                        if ((right_ && totalX > 0) || (!right_ && totalX < 0))
                        {
                            if (await DisplayAlert("Confirm the deleting", "Are you sure?", "Yes!", "No"))
                            {
                                File.Delete((panSender as Note).Path);
                                if (right_)
                                {
                                    right.Children.Remove(panSender as Note);
                                } 
                                else
                                {
                                    left.Children.Remove(panSender as Note);
                                }
                            }
                            totalX = 0;
                        }
                        frame.TranslationX = 0;
                        break;
                    case GestureStatus.Running:
                        if ((right_ &&  panArgs.TotalX > 0) || (!right_ && panArgs.TotalX < 0))
                        {
                            frame.TranslationX = panArgs.TotalX;
                            totalX = panArgs.TotalX;
                        }
                        break;
                }
            };
            frame.GestureRecognizers.Add(pan);

            var label_pan = new PanGestureRecognizer();
            double totalX_ = 0;
            label_pan.PanUpdated += async (panSender, panArgs) =>
            {
                switch (panArgs.StatusType)
                {
                    case GestureStatus.Canceled:
                    case GestureStatus.Started:
                        frame.TranslationX = 0;
                        break;
                    case GestureStatus.Completed:
                        if ((right_ && totalX_ > 0) || (!right_ && totalX_ < 0))
                        {
                            if (await DisplayAlert("Confirm the deleting", "Are you sure?", "Yes!", "No"))
                            {
                                File.Delete((((panSender as Label).Parent) as Note).Path);
                                if (right_)
                                {
                                    right.Children.Remove(((panSender as Label).Parent) as Note);
                                }
                                else
                                {
                                    left.Children.Remove(((panSender as Label).Parent) as Note);
                                }
                            }
                            totalX = 0;
                        }
                        frame.TranslationX = 0;
                        break;
                    case GestureStatus.Running:
                        if ((right_ && panArgs.TotalX > 0) || (!right_ && panArgs.TotalX < 0))
                        {
                            frame.TranslationX = panArgs.TotalX;
                            totalX_ = panArgs.TotalX;
                        }
                        break;
                }
            };
            label.GestureRecognizers.Add(label_pan);
            if (right_)
            {             
                right.Children.Add(frame);
            }
            else
            {
                left.Children.Add(frame);
            }
        }
        public MainPage()
        {
            InitializeComponent();

            string newName = counterLeft + "Left.txt";
            string newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
            while (File.Exists(newFile))
            {
                string text = File.ReadAllText(newFile);
                newName = ++counterLeft + "Left.txt";
                newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
                add(text, false, newFile);
            }
            //counterLeft = Math.Max(0, counterLeft - 1);

            
            newName = counterRight + "Right.txt";
            newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
            while (File.Exists(newFile))
            {
                string text = File.ReadAllText(newFile);
                newName = ++counterRight + "Right.txt";
                newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
                add(text, true, newFile);

            }
            //counterRight = Math.Max(0, counterRight - 1);
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (CrossShare.IsSupported)
            {
                CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage()
                {
                    Title = "Title",
                    Text = "Body Text"
                });
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            EditorPage editor = new EditorPage();
            editor.Disappearing += (s, _e) => {
               
                string newName;
                string newFile;
                if (left.Height > right.Height)
                {
                    newName = counterLeft++ + "Right.txt";
                    newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
                    add(editor.text, true, newFile);
                }
                else
                {
                    newName = counterLeft++ + "Left.txt";
                    newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
                    add(editor.text, false, newFile);
                }

                File.WriteAllText(newFile, editor.text);
            };
            Navigation.PushAsync(editor);
        }
    }
}
