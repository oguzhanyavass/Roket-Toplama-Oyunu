/*
 * Oğuzhan Yavaş
 * Bilişim Sistemleri Mühendisliği 1. sınıf
 * Nesneye Dayalı Programlama
 * Öğretmen: Muhammed Kotan
 */
using System;
using Game.Library.Enum;

namespace Game.Library.Abstract
{
    internal interface IOyun
    {
        event EventHandler KalanSureDegisti;
        bool DevamkeMi { get;}
        int KalanSure { get; }
 
        void Start();
        void HareketEt(Yon yon);
        void Done();
        void StartStop();
       


    }
}