using LanguageExt;
using PatrimonioTech.Domain.Common;

namespace PatrimonioTech.Domain.Escrituradores;

public sealed class Escriturador(NotEmptyString nome, Option<Cnpj> cnpj)
{
    public NotEmptyString Nome { get; } = nome;
    public Option<Cnpj> Cnpj { get; } = cnpj;
}
