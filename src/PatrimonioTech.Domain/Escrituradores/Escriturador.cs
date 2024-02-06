using PatrimonioTech.Domain.Common.ValueObjects;

namespace PatrimonioTech.Domain.Escrituradores;

public sealed class Escriturador(NotEmptyString nome, Maybe<Cnpj> cnpj)
{
    public NotEmptyString Nome { get; } = nome;
    public Maybe<Cnpj> Cnpj { get; } = cnpj;
}
