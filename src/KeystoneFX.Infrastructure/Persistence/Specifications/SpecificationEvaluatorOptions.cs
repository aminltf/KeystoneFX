namespace KeystoneFX.Infrastructure.Persistence.Specifications;

public sealed class SpecificationEvaluatorOptions
{
    public bool UseSplitQueryForMultipleIncludes { get; init; } = true;

    public bool ApplyIncludesInProjection { get; init; } = false;
}