using PatrimonioTech.Domain.Common.ValueObjects;
using PatrimonioTech.Domain.Escrituradores;
using PatrimonioTech.Domain.TiposAtivos;

namespace PatrimonioTech.Domain.Ativos;

public class Ativo(
    TipoAtivo tipoAtivo,
    B3Ticker ticker,
    Option<Cnpj> cnpj = default,
    Option<NotEmptyString> razaoSocial = default,
    Option<Escriturador> escriturador = default)
{
    public TipoAtivo TipoAtivo { get; } = tipoAtivo;
    public B3Ticker Ticker { get; } = ticker;
    public Option<Cnpj> Cnpj { get; } = cnpj;
    public Option<NotEmptyString> RazaoSocial { get; } = razaoSocial;
    public Option<Escriturador> Escriturador { get; } = escriturador;
}
