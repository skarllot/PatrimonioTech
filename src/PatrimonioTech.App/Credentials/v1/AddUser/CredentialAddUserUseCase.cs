﻿using PatrimonioTech.App.Database;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.AddUser;

[GenerateAutomaticInterface]
public sealed class CredentialAddUserUseCase(
    IDatabaseAdmin databaseAdmin,
    IUserCredentialRepository userCredentialRepository,
    IAddUserScenario addUserScenario,
    IKeyDerivation keyDerivation)
    : ICredentialAddUserUseCase
{
    public Task<Result<Unit, CredentialAddUserError>> Execute(
        CredentialAddUserRequest request,
        CancellationToken cancellationToken)
    {
        return from scnRes in addUserScenario
                .Execute(new AddUserCredential(request.Name, request.Password, request.KeySize, request.Iterations))
                .MapErr(CredentialAddUserError.BusinessError.λ)
                .ToTask()
            let model = UserCredential.Create(scnRes)
            from key in keyDerivation
                .TryGetKey(request.Password, model.Salt, model.Key, model.KeySize, model.Iterations)
                .MapErr(CredentialAddUserError.CryptographyError.λ)
                .ToTask()
            from db in databaseAdmin.CreateDatabase(model.Database, key)
                .MapErr(CredentialAddUserError.DatabaseError.λ)
                .ToTask()
            from repoRes in userCredentialRepository.Add(model, cancellationToken)
                .MapErrT(CredentialAddUserError.StorageError.λ)
            select Unit.Default;
    }
}
