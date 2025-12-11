using Dapper;
using System.Data;

namespace BookCatalog.Infrastructure.Persistence
{
    public class DecimalTypeHandler : SqlMapper.TypeHandler<decimal>
    {
        public override decimal Parse(object value)
        {
            if (value == null || value is DBNull)
                return 0m;
            
            return Convert.ToDecimal(value);
        }

        public override void SetValue(IDbDataParameter parameter, decimal value)
        {
            parameter.Value = value;
            parameter.DbType = DbType.Decimal;
        }
    }
}
