/*
 * Oğuzhan Yavaş
 * Bilişim Sistemleri Mühendisliği 1. sınıf
 * Nesneye Dayalı Programlama
 * Öğretmen: Muhammed Kotan
 */
using System;
using System.Windows.Forms;

namespace Sancar
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AnaForm());
        }
    }
}
