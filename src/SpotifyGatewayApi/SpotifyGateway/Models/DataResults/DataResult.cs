using System.Collections.Generic;
using System.Linq;

namespace SpotifyGateway.Models.DataResults
{
    public class DataResult<T1, T2>
    {
        public T1 First => FirstList.FirstOrDefault();

        public T2 Second => SecondList.FirstOrDefault();

        public List<T1> FirstList { get; set; }

        public List<T2> SecondList { get; set; }
    }

    public class DataResult<T1, T2, T3> : DataResult<T1, T2>
    {
        public T3 Third => ThirdList.FirstOrDefault();

        public List<T3> ThirdList { get; set; }
    }

    public class DataResult<T1, T2, T3, T4> : DataResult<T1, T2, T3>
    {
        public T4 Fourth => FourthList.FirstOrDefault();

        public List<T4> FourthList { get; set; }
    }
}