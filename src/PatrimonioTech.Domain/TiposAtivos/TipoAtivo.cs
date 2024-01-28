using PatrimonioTech.Domain.Common;
using PatrimonioTech.Domain.Common.ValueObjects;

namespace PatrimonioTech.Domain.TiposAtivos;

public sealed class TipoAtivo(NotEmptyString nome)
{
    public NotEmptyString Nome { get; } = nome;

    public static IEnumerable<TipoAtivo> Seed()
    {
        return new[]
            {
                NotEmptyString.Create("Não Especificado"),
                NotEmptyString.Create("Ação"),
                NotEmptyString.Create("FII"),
                NotEmptyString.Create("FI-Infra"),
                NotEmptyString.Create("Fundo")
            }
            .Select(r => r.TryGetValue())
            .Choose(v => new TipoAtivo(v));
    }
}
