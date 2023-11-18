using PatrimonioTech.Domain.Common;

namespace PatrimonioTech.Domain.TiposAtivos;

public sealed class TipoAtivo(NotEmptyString nome)
{
    public NotEmptyString Nome { get; } = nome;

    public static IEnumerable<TipoAtivo> Seed()
    {
        yield return new TipoAtivo(NotEmptyString.From("Não Especificado"));
        yield return new TipoAtivo(NotEmptyString.From("Ação"));
        yield return new TipoAtivo(NotEmptyString.From("FII"));
        yield return new TipoAtivo(NotEmptyString.From("FI-Infra"));
        yield return new TipoAtivo(NotEmptyString.From("Fundo"));
    }
}
