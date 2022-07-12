using System.Collections.Concurrent;
using System.Collections.Immutable;
using Gig.Framework.Analyzer.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Gig.Framework.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MessageContractAnalyzer :
    DiagnosticAnalyzer
{
    public const string StructurallyCompatibleRuleId = "MCA0001";
    public const string ValidMessageContractStructureRuleId = "MCA0002";
    public const string MissingPropertiesRuleId = "MCA0003";

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization

    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor StructurallyCompatibleRule = new(StructurallyCompatibleRuleId,
        "Anonymous type does not map to message contract",
        "Anonymous type does not map to message contract '{0}'. The following properties of the anonymous type are incompatible: {1}."
        ,
        Category, DiagnosticSeverity.Error, true,
        "Anonymous type should map to message contract.");

    private static readonly DiagnosticDescriptor ValidMessageContractStructureRule =
        new(ValidMessageContractStructureRuleId,
            "Message contract does not have a valid structure",
            "Message contract '{0}' does not have a valid structure",
            Category, DiagnosticSeverity.Error, true,
            "Message contract should have a valid structure. Properties should be primitive, string or IReadOnlyList or ImmutableArray of a primitive, string or message contract.");

    private static readonly DiagnosticDescriptor MissingPropertiesRule = new(MissingPropertiesRuleId,
        "Anonymous type is missing properties that are in the message contract",
        "Anonymous type is missing properties that are in the message contract '{0}'. The following properties are missing: {1}."
        ,
        Category, DiagnosticSeverity.Info, true,
        "Anonymous type misses properties that are in the message contract.");

    private readonly ConcurrentDictionary<SemanticModel, Lazy<TypeConversionHelper>> _typeConverterHelpers =
        new();

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(StructurallyCompatibleRule, ValidMessageContractStructureRule, MissingPropertiesRule);

    public override void Initialize(AnalysisContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        context.RegisterSyntaxNodeAction(AnalyzeAnonymousObjectCreationNode,
            SyntaxKind.AnonymousObjectCreationExpression);
    }

    private void AnalyzeAnonymousObjectCreationNode(SyntaxNodeAnalysisContext context)
    {
        var typeConverterHelper = GetTypeConverterHelper(context);

        var anonymousObject = (AnonymousObjectCreationExpressionSyntax)context.Node;

        if (anonymousObject.Parent is ArgumentSyntax argumentSyntax
            && argumentSyntax.IsActivator(context.SemanticModel, out var typeArgument))
        {
            if (typeArgument.HasMessageContract(out var messageContractType))
            {
                var anonymousType = ModelExtensions.GetTypeInfo(context.SemanticModel, anonymousObject).Type;

                var incompatibleProperties = new List<string>();
                if (!TypesAreStructurallyCompatible(typeConverterHelper, messageContractType, anonymousType,
                        string.Empty, incompatibleProperties))
                {
                    var diagnostic = Diagnostic.Create(StructurallyCompatibleRule, anonymousType.Locations[0],
                        messageContractType.Name, string.Join(", ", incompatibleProperties));
                    context.ReportDiagnostic(diagnostic);
                }

                var missingProperties = new List<string>();
                var symbolPath = Enumerable.Empty<ITypeSymbol>();
                if (HasMissingProperties(anonymousType, messageContractType, string.Empty, symbolPath,
                        missingProperties))
                {
                    var diagnostic = Diagnostic.Create(MissingPropertiesRule, anonymousType.Locations[0],
                        messageContractType.Name, string.Join(", ", missingProperties));
                    context.ReportDiagnostic(diagnostic);
                }
            }
            else
            {
                var diagnostic = Diagnostic.Create(ValidMessageContractStructureRule, context.Node.GetLocation(),
                    typeArgument.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private TypeConversionHelper GetTypeConverterHelper(SyntaxNodeAnalysisContext context)
    {
        return _typeConverterHelpers.GetOrAdd(context.SemanticModel,
            model => new Lazy<TypeConversionHelper>(() => new TypeConversionHelper(model))).Value;
    }

    private static bool TypesAreStructurallyCompatible(TypeConversionHelper typeConverterHelper,
        ITypeSymbol contractType, ITypeSymbol inputType,
        string path, ICollection<string> incompatibleProperties)
    {
        if (SymbolEqualityComparer.Default.Equals(inputType, contractType))
            return true;

        var contractProperties = contractType.GetContractProperties();
        var inputProperties = GetInputProperties(inputType);
        var result = true;

        foreach (var inputProperty in inputProperties)
        {
            var contractProperty = contractProperties.FirstOrDefault(m => m.Name == inputProperty.Name);

            var propertyPath = Append(path, inputProperty.Name);

            if (contractProperty == null)
            {
                if (!IsHeaderProperty(typeConverterHelper, inputProperty))
                {
                    incompatibleProperties.Add(propertyPath);
                    result = false;
                }
            }
            else if (!PropertyTypesAreStructurallyCompatible(typeConverterHelper, contractProperty, inputProperty,
                         propertyPath, incompatibleProperties))
            {
                result = false;
            }
        }

        return result;
    }

    private static bool PropertyTypesAreStructurallyCompatible(TypeConversionHelper typeConverterHelper,
        IPropertySymbol contractProperty, IPropertySymbol inputProperty,
        string path, ICollection<string> incompatibleProperties)
    {
        if (typeConverterHelper.CanConvert(contractProperty.Type, inputProperty.Type)) return true;

        var result = AnonymousTypeAndInterfaceAreStructurallyCompatible(typeConverterHelper, contractProperty,
                         inputProperty, path, incompatibleProperties)
                     ?? EnumerableTypesAreStructurallyCompatible(typeConverterHelper, contractProperty,
                         inputProperty, path, incompatibleProperties)
                     ?? DictionaryTypesAreStructurallyCompatible(typeConverterHelper, contractProperty,
                         inputProperty, path, incompatibleProperties);
        if (result.HasValue) return result.Value;

        incompatibleProperties.Add(path);
        return false;
    }

    private static bool? AnonymousTypeAndInterfaceAreStructurallyCompatible(
        TypeConversionHelper typeConverterHelper, IPropertySymbol contractProperty, IPropertySymbol inputProperty,
        string path, ICollection<string> incompatibleProperties)
    {
        if (inputProperty.Type.IsAnonymousType)
        {
            if (contractProperty.Type.TypeKind.IsClassOrInterface())
            {
                if (!TypesAreStructurallyCompatible(typeConverterHelper, contractProperty.Type,
                        inputProperty.Type, path, incompatibleProperties))
                    return false;
            }
            else
            {
                incompatibleProperties.Add(path);
                return false;
            }

            return true;
        }

        return null;
    }

    private static bool? EnumerableTypesAreStructurallyCompatible(TypeConversionHelper typeConverterHelper,
        IPropertySymbol contractProperty, IPropertySymbol inputProperty,
        string path, ICollection<string> incompatibleProperties)
    {
        if (contractProperty.Type.IsImmutableArray(out var contractElementType)
            || contractProperty.Type.IsList(out contractElementType)
            || contractProperty.Type.IsArray(out contractElementType)
            || contractProperty.Type.IsCollection(out contractElementType))
        {
            if (inputProperty.Type.IsImmutableArray(out var inputElementType)
                || inputProperty.Type.IsList(out inputElementType)
                || inputProperty.Type.IsArray(out inputElementType)
                || inputProperty.Type.IsCollection(out inputElementType))
            {
                if (!ElementTypesAreStructurallyCompatible(typeConverterHelper, contractElementType,
                        inputElementType, path, incompatibleProperties)) return false;
            }
            // a single element will be added to a list in the message contract
            else if (!typeConverterHelper.CanConvert(contractElementType, inputProperty.Type))
            {
                incompatibleProperties.Add(path);
                return false;
            }

            return true;
        }

        return null;
    }

    private static bool ElementTypesAreStructurallyCompatible(TypeConversionHelper typeConverterHelper,
        ITypeSymbol contractElementType, ITypeSymbol inputElementType,
        string path, ICollection<string> incompatibleProperties)
    {
        if (typeConverterHelper.CanConvert(contractElementType, inputElementType)) return true;

        if (contractElementType.TypeKind.IsClassOrInterface())
        {
            if (!TypesAreStructurallyCompatible(typeConverterHelper, contractElementType,
                    inputElementType, path, incompatibleProperties))
                return false;
        }
        else
        {
            incompatibleProperties.Add(path);
            return false;
        }

        return true;
    }

    private static bool? DictionaryTypesAreStructurallyCompatible(TypeConversionHelper typeConverterHelper,
        IPropertySymbol contractProperty, IPropertySymbol inputProperty,
        string path, ICollection<string> incompatibleProperties)
    {
        if (contractProperty.Type.IsDictionary(out var contractKeyType, out var contractValueType))
        {
            if (inputProperty.Type.IsDictionary(out var inputKeyType, out var inputValueType))
            {
                if (!KeyValueTypesAreStructurallyCompatible(typeConverterHelper, contractKeyType, contractValueType,
                        inputKeyType, inputValueType, path, incompatibleProperties)) return false;
            }
            else
            {
                incompatibleProperties.Add(path);
                return false;
            }

            return true;
        }

        return null;
    }

    private static bool KeyValueTypesAreStructurallyCompatible(TypeConversionHelper typeConverterHelper,
        ITypeSymbol contractKeyType, ITypeSymbol contractValueType, ITypeSymbol inputKeyType,
        ITypeSymbol inputValueType,
        string path, ICollection<string> incompatibleProperties)
    {
        if (typeConverterHelper.CanConvert(contractKeyType, inputKeyType)
            && typeConverterHelper.CanConvert(contractValueType, inputValueType))
            return true;

        if (contractValueType.TypeKind.IsClassOrInterface())
        {
            if (!TypesAreStructurallyCompatible(typeConverterHelper, contractValueType,
                    inputValueType, path, incompatibleProperties))
                return false;
        }
        else
        {
            incompatibleProperties.Add(path);
            return false;
        }

        return true;
    }

    private static bool IsHeaderProperty(TypeConversionHelper typeConverterHelper, IPropertySymbol messageProperty)
    {
        if (!messageProperty.Name.StartsWith("__"))
            return false;

        if (messageProperty.Name.StartsWith("__Header_"))
            return true;

        return messageProperty.Name switch
        {
            "__SourceAddress" => typeConverterHelper.CanConvert(typeof(Uri), messageProperty.Type),
            "__DestinationAddress" => typeConverterHelper.CanConvert(typeof(Uri), messageProperty.Type),
            "__ResponseAddress" => typeConverterHelper.CanConvert(typeof(Uri), messageProperty.Type),
            "__FaultAddress" => typeConverterHelper.CanConvert(typeof(Uri), messageProperty.Type),
            "__RequestId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
            "__MessageId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
            "__ConversationId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
            "__CorrelationId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
            "__InitiatorId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
            "__ScheduledMessageId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
            "__TimeToLive" => typeConverterHelper.CanConvert(typeof(TimeSpan), messageProperty.Type),
            "__Durable" => typeConverterHelper.CanConvert(typeof(bool), messageProperty.Type),
            _ => false
        };
    }

    private static bool HasMissingProperties(ITypeSymbol inputType, ITypeSymbol contractType,
        string path, IEnumerable<ITypeSymbol> symbolPath, ICollection<string> missingProperties)
    {
        var contractProperties = contractType.GetContractProperties();
        var inputProperties = GetInputProperties(inputType);
        var result = false;

        foreach (var contractProperty in contractProperties)
        {
            var inputProperty = inputProperties.FirstOrDefault(m => m.Name == contractProperty.Name);

            var propertyPath = Append(path, contractProperty.Name);

            if (inputProperty == null)
            {
                missingProperties.Add(propertyPath);
                result = true;
            }
            else if (HasMissingProperties(inputProperty, contractProperty, propertyPath, symbolPath,
                         missingProperties))
            {
                result = true;
            }
        }

        return result;
    }

    private static bool HasMissingProperties(IPropertySymbol inputProperty, IPropertySymbol contractProperty,
        string path, IEnumerable<ITypeSymbol> symbolPath, ICollection<string> missingProperties)
    {
        var result =
            EnumerableTypeHasMissingProperties(inputProperty, contractProperty, path, symbolPath, missingProperties)
            ?? AnonymousTypeHasMissingProperties(inputProperty, contractProperty, path, symbolPath,
                missingProperties);

        return result ?? false;
    }

    private static bool? EnumerableTypeHasMissingProperties(IPropertySymbol inputProperty,
        IPropertySymbol contractProperty,
        string path, IEnumerable<ITypeSymbol> symbolPath, ICollection<string> missingProperties)
    {
        if (contractProperty.Type.IsImmutableArray(out var contractElementType)
            || contractProperty.Type.IsList(out contractElementType)
            || contractProperty.Type.IsArray(out contractElementType))
        {
            if ((inputProperty.Type.IsImmutableArray(out var inputElementType)
                 || inputProperty.Type.IsList(out inputElementType)
                 || inputProperty.Type.IsArray(out inputElementType))
                && contractElementType.TypeKind.IsClassOrInterface()
                && !symbolPath.Contains(contractElementType, SymbolEqualityComparer.Default)
                && HasMissingProperties(inputElementType, contractElementType, path,
                    symbolPath.Concat(new[] { contractElementType }), missingProperties))
                return true;

            return false;
        }

        return null;
    }

    private static bool? AnonymousTypeHasMissingProperties(IPropertySymbol inputProperty,
        IPropertySymbol contractProperty,
        string path, IEnumerable<ITypeSymbol> symbolPath, ICollection<string> missingProperties)
    {
        if (contractProperty.Type.TypeKind.IsClassOrInterface())
        {
            if (inputProperty.Type.IsAnonymousType
                && !symbolPath.Contains(contractProperty.Type, SymbolEqualityComparer.Default)
                && HasMissingProperties(inputProperty.Type, contractProperty.Type, path,
                    symbolPath.Concat(new[] { contractProperty.Type }), missingProperties))
                return true;

            return false;
        }

        return null;
    }

    private static List<IPropertySymbol> GetInputProperties(ITypeSymbol inputType)
    {
        return inputType.GetMembers().OfType<IPropertySymbol>().ToList();
    }

    private static string Append(string path, string propertyName)
    {
        if (string.IsNullOrEmpty(path))
            return propertyName;

        if (path.EndsWith(".", StringComparison.Ordinal))
            return $"{path}{propertyName}";

        return $"{path}.{propertyName}";
    }
}