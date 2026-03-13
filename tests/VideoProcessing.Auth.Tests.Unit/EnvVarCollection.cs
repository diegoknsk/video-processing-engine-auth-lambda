namespace VideoProcessing.Auth.Tests.Unit;

/// <summary>
/// Collection para testes que alteram variáveis de ambiente (GATEWAY_STAGE, GATEWAY_PATH_PREFIX).
/// Evita execução paralela entre esses testes para não haver interferência.
/// </summary>
[CollectionDefinition("EnvVar", DisableParallelization = true)]
public sealed class EnvVarCollection;
