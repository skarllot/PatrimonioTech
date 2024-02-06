using PatrimonioTech.Domain.Common.ValueObjects;
using PatrimonioTech.Domain.Escrituradores;
using PatrimonioTech.Domain.TiposAtivos;

namespace PatrimonioTech.Domain.Ativos;

public class Ativo(
    TipoAtivo tipoAtivo,
    B3Ticker ticker,
    Maybe<Cnpj> cnpj = default,
    Maybe<NotEmptyString> razaoSocial = default,
    Maybe<Escriturador> escriturador = default)
{
    public TipoAtivo TipoAtivo { get; } = tipoAtivo;
    public B3Ticker Ticker { get; } = ticker;
    public Maybe<Cnpj> Cnpj { get; } = cnpj;
    public Maybe<NotEmptyString> RazaoSocial { get; } = razaoSocial;
    public Maybe<Escriturador> Escriturador { get; } = escriturador;
}
