using System.Linq;

namespace YandexSearch
{
    internal class HouseNumber
    {
        private readonly string _name;
        private readonly int number = -1;

        public HouseNumber(string name)
        {
            this._name = name;
            var intString = name.TakeWhile(char.IsDigit).Aggregate("", (current, t) => current + t);

            int.TryParse(intString, out number);
        }

        public int Number => number;

        public string Name => _name;
    }
}