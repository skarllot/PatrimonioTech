using LiteDB;
using PatrimonioTech.Domain.TiposAtivos;

namespace PatrimonioTech.Infra.TiposAtivos;

public static class TipoAtivoDbMapper
{
    public static void Map(BsonMapper mapper)
    {
        mapper.Entity<TipoAtivo>();
    }
}
