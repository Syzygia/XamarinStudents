using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App2.Components
{
    public class Note : Frame
    {
        public string InnerText { get; set; }
        public string Path { get; set; }
        public bool Right { get; set; }
    }
}
