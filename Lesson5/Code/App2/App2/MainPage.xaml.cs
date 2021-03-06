﻿using App2.Components;
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
        public void Rename(Note note, bool right_)
        {
            if (right_)
            {
                
                File.Move(note.Path, note.Path.Replace($"{note.Path.Split(System.IO.Path.DirectorySeparatorChar).Last()}",
                        $"{right.Children.Count}Right.txt"));
                note.Path = note.Path.Replace($"{note.Path.Split(System.IO.Path.DirectorySeparatorChar).Last()}",
                        $"{right.Children.Count}Right.txt");
            }
            else
            {
                File.Move(note.Path, note.Path.Replace($"{note.Path.Split(System.IO.Path.DirectorySeparatorChar).Last()}",
                         $"{left.Children.Count}Left.txt"));
                note.Path = note.Path.Replace($"{note.Path.Split(System.IO.Path.DirectorySeparatorChar).Last()}",
                        $"{left.Children.Count}Left.txt");
            }
            note.Right = right_;
        }
        public void BigRename()
        {
            int i = 0;
            Note note;
            foreach(var child in left.Children)
            {
                note = (child as Note);
                File.Move(note.Path, note.Path.Replace($"{note.Path.Split(System.IO.Path.DirectorySeparatorChar).Last()}",
                       $"{i}Left.txt"));
                note.Path = note.Path.Replace($"{note.Path.Split(System.IO.Path.DirectorySeparatorChar).Last()}",
                        $"{i}Left.txt");
                ++i;
            }
            i = 0;
            foreach (var child in right.Children)
            {
                note = (child as Note);
                File.Move(note.Path, note.Path.Replace($"{note.Path.Split(System.IO.Path.DirectorySeparatorChar).Last()}",
                       $"{i}Right.txt"));
                note.Path = note.Path.Replace($"{note.Path.Split(System.IO.Path.DirectorySeparatorChar).Last()}",
                        $"{i}Right.txt");
                ++i;
            }
        }
        public void Balance ()
        {
            double lheight = 0;
            foreach(var child in left.Children)
            {
                lheight += child.Height;
            }
            double rheight = 0;
            foreach (var child in right.Children)
            {
                rheight += child.Height;
            }
            if (lheight == rheight)
            {
                return;
            }
            if (lheight > rheight)
            {
                if (left.Children.Count == 0)
                {
                    return;
                }
                var frame = left.Children.Last();
                Rename((frame as Note), true);
                left.Children.Remove(frame);
                right.Children.Add(frame);
            }
            else
            {
                if (right.Children.Count == 0)
                {
                    return;
                }
                var frame = right.Children.Last();
                Rename((frame as Note), false);
                right.Children.Remove(frame);
                left.Children.Add(frame);
            }
        }       
        public void add(string text, bool right_, string path)
        {
            if (text == null)
            {
                return;
            }
            int size = (text.Length >= 10) ? 10 : text.Length;
            Note frame = new Note
            {
                BorderColor = Color.Aqua,
                InnerText = text,
                Path = path,               
                Right = right_
            };
            StackLayout box = new StackLayout();          
            Label label = new Label();
            label.Text = text.Substring(0, size);
            label.BackgroundColor = Color.Beige;
            label.LineBreakMode = LineBreakMode.WordWrap;
            Label label1 = new Label
            {
                Text = File.GetCreationTime(path).ToString()
            };
            box.Children.Add(label);
            box.Children.Add(label1);
            frame.Content = box;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (tapsender, tape) =>
            {
                EditorPage editor2 = new EditorPage(frame.InnerText);
                editor2.Disappearing += (a, b) =>
                {
                    frame.InnerText = editor2.text;
                    int length = (editor2.text.Length > 10) ? 10 : editor2.text.Length; 
                    label.Text = editor2.text.Substring(0,length);
                    File.WriteAllText(frame.Path, editor2.text);
                    label1.Text = File.GetLastWriteTime(path).ToString();

                };
                Navigation.PushAsync(editor2);
            };
            label.GestureRecognizers.Add(tapGestureRecognizer);
            box.GestureRecognizers.Add(tapGestureRecognizer);
            label1.GestureRecognizers.Add(tapGestureRecognizer);
            frame.GestureRecognizers.Add(tapGestureRecognizer);
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
                        if (((panSender as Note).Right && totalX > 0) || (!(panSender as Note).Right && totalX < 0))
                        {
                            if (await DisplayAlert("Confirm the deleting", "Are you sure?", "Yes!", "No"))
                            {
                                File.Delete((panSender as Note).Path);
                                if ((panSender as Note).Right)
                                {
                                    right.Children.Remove(panSender as Note);
                                } 
                                else
                                {
                                    left.Children.Remove(panSender as Note);
                                }
                                BigRename();
                                Balance();
                            }
                            totalX = 0;
                        }
                        frame.TranslationX = 0;
                        break;
                    case GestureStatus.Running:
                        if (((panSender as Note).Right &&  panArgs.TotalX > 0) || (!(panSender as Note).Right && panArgs.TotalX < 0))
                        {
                            (panSender as Note).TranslationX = panArgs.TotalX;
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
                        if (((((panSender as Label).Parent).Parent as Note).Right && totalX_ > 0) ||
                            (!(((panSender as Label).Parent).Parent as Note).Right && totalX_ < 0))
                        {
                            if (await DisplayAlert("Confirm the deleting", "Are you sure?", "Yes!", "No"))
                            {
                                File.Delete((((panSender as Label).Parent).Parent as Note).Path);
                                if ((((panSender as Label).Parent).Parent as Note).Right)
                                {
                                    right.Children.Remove(((panSender as Label).Parent).Parent as Note);
                                }
                                else
                                {
                                    left.Children.Remove(((panSender as Label).Parent).Parent as Note);
                                }
                                BigRename();
                                Balance();
                            }
                            totalX_ = 0;
                        }
                        (((panSender as Label).Parent).Parent as Note).TranslationX = 0;
                        break;
                    case GestureStatus.Running:
                        if (((((panSender as Label).Parent).Parent as Note).Right && panArgs.TotalX > 0) ||
                            (!(((panSender as Label).Parent).Parent as Note).Right && panArgs.TotalX < 0))
                        {
                            (((panSender as Label).Parent).Parent as Note).TranslationX = panArgs.TotalX;
                            totalX_ = panArgs.TotalX;
                        }
                        break;
                }
            };
            label.GestureRecognizers.Add(label_pan);
            label1.GestureRecognizers.Add(label_pan);
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
            /*for (int i = 0; i < 100; ++i)
            {
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{i}Left.txt"));
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{i}Right.txt"));
            }*/
            foreach (var file in Directory.GetFiles(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "*.txt"))
            {
                add(File.ReadAllText(file),
                    (file.Contains("Right.txt")) ? true : false,
                    file);                
            }
            //Balance();
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
                    newName = right.Children.Count + "Right.txt";
                    newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
                    add(editor.text, true, newFile);
                }
                else
                {
                    newName = left.Children.Count + "Left.txt";
                    newFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newName);
                    add(editor.text, false, newFile);
                }
                File.WriteAllText(newFile, editor.text);
            };
            Navigation.PushAsync(editor);
        }
        public bool balance = true;       
    }
}
