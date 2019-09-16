using BiliClient.UI.Models;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliClient.UI
{
    class ViewModel
    {
        public ViewModel()
        {
            this.AccentColors = ThemeManager.Accents
                                .Select(a => new AccentColor() { Name = a.Name, ColorBrush = new SolidBrush(Color.Blue) })
                                .ToList();
        }
        public List<AccentColor> AccentColors { get; set; }
    }
}
