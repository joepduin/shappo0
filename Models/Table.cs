namespace shappo0.Models
{
    public class Table
    {
        public int TableNummer { get; set; }
        public bool Bezet { get; set; } //true is bezet false is vrij

        public Table()
        {
        }

        public Table(int tableNummer, bool bezet)
        {
            TableNummer = tableNummer;
            Bezet = bezet;
        }
    }
}
