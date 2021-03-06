﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherForecastDR
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // We keep this entry point so its easier to debug UI 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // for VS debugging 
            Application.Run(new FindLocationForm(new WeatherDR(),false));
        }
    }
}
