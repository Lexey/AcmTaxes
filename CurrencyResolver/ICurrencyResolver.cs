namespace Acm.CurrencyResolver
{
    public interface ICurrencyResolver
    {
        public decimal Resolve(CurrencyCode code, DateOnly date);
    }
}
