using Game.Library.Abstract;

namespace Game.Library.Concrete
{
    internal class Muhendis : Cisim
    {
        public Muhendis(int panelUzunlugu, int panelGenisligi) : base(panelUzunlugu, panelGenisligi)
        {
            var filokomutanıKonum = panelUzunlugu - Height;

            Center = panelGenisligi / 2;
            Top = filokomutanıKonum;

            hareketMesafesi = Width;
        }
    }
}
