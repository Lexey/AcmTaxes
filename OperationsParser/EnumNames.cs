using System.Collections.ObjectModel;
using CodeJam;

namespace Acm.OperationsParser
{
    public class EnumNames<T>(IEnumerable<KeyValuePair<T, string>> names)
        where T : struct, Enum
    {
        private readonly Dictionary<T, string> names_ = new(names);

        public string this[T enumValue]
        {
            get => names_[enumValue];
            set
            {
                EnumCode.Defined(enumValue, nameof(enumValue));
                names_[enumValue] = value;
            }
        }

        public IReadOnlyDictionary<T, string> ToReadOnlyDictionary()
        {
            return new ReadOnlyDictionary<T, string>(names_);
        }
    }
}
