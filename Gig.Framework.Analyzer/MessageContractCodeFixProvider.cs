using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace Gig.Framework.Analyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MessageContractCodeFixProvider))]
[Shared]
public class MessageContractCodeFixProvider :
    CodeFixProvider
{
    private const string Title = "Add missing properties";

    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(MessageContractAnalyzer.MissingPropertiesRuleId);

    public sealed override FixAllProvider GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic.
        var anonymousObject = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
            .OfType<AnonymousObjectCreationExpressionSyntax>().First();

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                Title,
                cancellationToken => AddMissingProperties(context.Document, anonymousObject, cancellationToken),
                Title),
            diagnostic);
    }

    private static async Task<Document> AddMissingProperties(Document document,
        AnonymousObjectCreationExpressionSyntax anonymousObject, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        if (anonymousObject.Parent is ArgumentSyntax argumentSyntax &&
            argumentSyntax.IsActivator(semanticModel, out var typeArgument) &&
            typeArgument.HasMessageContract(out var contractType))
        {
            var dictionary = new Dictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol>();

            await FindAnonymousTypesWithMessageContractsInTree(dictionary, anonymousObject, contractType, semanticModel)
                .ConfigureAwait(false);

            var newRoot = AddMissingProperties(root, dictionary);

            var formattedRoot = Formatter.Format(newRoot, Formatter.Annotation, document.Project.Solution.Workspace,
                document.Project.Solution.Workspace.Options);

            return document.WithSyntaxRoot(formattedRoot);
        }

        return document;
    }

    private static async Task FindAnonymousTypesWithMessageContractsInTree(
        IDictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol> dictionary,
        AnonymousObjectCreationExpressionSyntax anonymousObject, ITypeSymbol contractType, SemanticModel semanticModel)
    {
        var contractProperties = contractType.GetContractProperties();

        foreach (var initializer in anonymousObject.Initializers)
        {
            var name = GetName(initializer);

            var contractProperty = contractProperties.FirstOrDefault(p => p.Name == name);

            if (contractProperty != null)
                await FindAnonymousTypesWithMessageContractsInTree(dictionary, initializer, contractProperty,
                    semanticModel).ConfigureAwait(false);
        }

        dictionary.Add(anonymousObject, contractType);
    }

    private static async Task FindAnonymousTypesWithMessageContractsInTree(
        IDictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol> dictionary,
        AnonymousObjectMemberDeclaratorSyntax initializer, IPropertySymbol contractProperty,
        SemanticModel semanticModel)
    {
        if (initializer.Expression is ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax)
        {
            if (contractProperty.Type.IsImmutableArray(out var contractElementType)
                || contractProperty.Type.IsList(out contractElementType)
                || contractProperty.Type.IsArray(out contractElementType))
                await FindAnonymousTypesWithMessageContractsInTree(dictionary, implicitArrayCreationExpressionSyntax,
                        contractElementType, semanticModel)
                    .ConfigureAwait(false);
        }
        else if (initializer.Expression is AnonymousObjectCreationExpressionSyntax anonymousObjectProperty)
        {
            await FindAnonymousTypesWithMessageContractsInTree(dictionary, anonymousObjectProperty,
                    contractProperty.Type, semanticModel)
                .ConfigureAwait(false);
        }
        else if (initializer.Expression is InvocationExpressionSyntax invocationExpressionSyntax
                 && ModelExtensions.GetSymbolInfo(semanticModel, invocationExpressionSyntax).Symbol is IMethodSymbol
                     method
                 && method.ReturnType.IsList(out var methodReturnTypeArgument)
                 && methodReturnTypeArgument.IsAnonymousType)
        {
            if (contractProperty.Type.IsImmutableArray(out var contractElementType) ||
                contractProperty.Type.IsList(out contractElementType) ||
                contractProperty.Type.IsArray(out contractElementType))
                await FindAnonymousTypesWithMessageContractsInTree(dictionary, methodReturnTypeArgument,
                        contractElementType, semanticModel)
                    .ConfigureAwait(false);
        }
    }

    private static async Task FindAnonymousTypesWithMessageContractsInTree(
        IDictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol> dictionary,
        ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax, ITypeSymbol contractElementType,
        SemanticModel semanticModel)
    {
        var expressions = implicitArrayCreationExpressionSyntax.Initializer.Expressions;
        foreach (var expression in expressions)
            if (expression is AnonymousObjectCreationExpressionSyntax anonymousObjectArrayInitializer)
                await FindAnonymousTypesWithMessageContractsInTree(dictionary, anonymousObjectArrayInitializer,
                    contractElementType, semanticModel).ConfigureAwait(false);
    }

    private static async Task FindAnonymousTypesWithMessageContractsInTree(
        IDictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol> dictionary,
        ITypeSymbol methodReturnTypeArgument, ITypeSymbol contractElementType, SemanticModel semanticModel)
    {
        var syntax = await methodReturnTypeArgument.DeclaringSyntaxReferences[0].GetSyntaxAsync().ConfigureAwait(false);
        if (syntax is AnonymousObjectCreationExpressionSyntax anonymousObjectTypeArgument)
            await FindAnonymousTypesWithMessageContractsInTree(dictionary, anonymousObjectTypeArgument,
                contractElementType,
                semanticModel).ConfigureAwait(false);
    }

    private static string GetName(AnonymousObjectMemberDeclaratorSyntax initializer)
    {
        string name;
        if (initializer.NameEquals == null)
        {
            var expression = (MemberAccessExpressionSyntax)initializer.Expression;
            name = expression.Name.Identifier.Text;
        }
        else
        {
            name = initializer.NameEquals.Name.Identifier.Text;
        }

        return name;
    }

    private static SyntaxNode AddMissingProperties(SyntaxNode root,
        IDictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol> dictionary)
    {
        var newRoot = root.TrackNodes(dictionary.Keys);

        foreach (var keyValuePair in dictionary)
        {
            var anonymousObject = newRoot.GetCurrentNode(keyValuePair.Key);
            var contractType = keyValuePair.Value;
            newRoot = AddMissingProperties(newRoot, anonymousObject, contractType);
        }

        return newRoot;
    }

    private static SyntaxNode AddMissingProperties(SyntaxNode root,
        AnonymousObjectCreationExpressionSyntax anonymousObject, ITypeSymbol contractType)
    {
        var newRoot = root;

        var contractProperties = contractType.GetContractProperties();

        var propertiesToAdd = new List<AnonymousObjectMemberDeclaratorSyntax>();
        foreach (var messageContractProperty in contractProperties)
        {
            var initializer =
                anonymousObject.Initializers.FirstOrDefault(i => GetName(i) == messageContractProperty.Name);
            if (initializer == null)
            {
                var path = Enumerable.Empty<ITypeSymbol>();
                var propertyToAdd = CreateProperty(messageContractProperty, path);
                propertiesToAdd.Add(propertyToAdd);
            }
        }

        if (propertiesToAdd.Any())
        {
            var newAnonymousObject = anonymousObject
                .AddInitializers(propertiesToAdd.ToArray())
                .WithAdditionalAnnotations(Formatter.Annotation);
            newRoot = newRoot.ReplaceNode(anonymousObject, newAnonymousObject);
        }

        return newRoot;
    }

    private static AnonymousObjectMemberDeclaratorSyntax[] CreateProperties(ITypeSymbol contractType,
        IEnumerable<ITypeSymbol> path)
    {
        var contractProperties = contractType.GetContractProperties();

        var propertiesToAdd = new List<AnonymousObjectMemberDeclaratorSyntax>();
        foreach (var contractProperty in contractProperties)
        {
            var propertyToAdd = CreateProperty(contractProperty, path);
            propertiesToAdd.Add(propertyToAdd);
        }

        return propertiesToAdd.ToArray();
    }

    private static AnonymousObjectMemberDeclaratorSyntax CreateProperty(IPropertySymbol contractProperty,
        IEnumerable<ITypeSymbol> path)
    {
        ExpressionSyntax expression;

        if (contractProperty.Type.IsImmutableArray(out var contractElementType) ||
            contractProperty.Type.IsList(out contractElementType) ||
            contractProperty.Type.IsArray(out contractElementType))
        {
            if (path.Contains(contractElementType, SymbolEqualityComparer.Default))
                expression = CreateEmptyArray(contractElementType);
            else
                expression = CreateImplicitArray(contractElementType, path.Concat(new[] { contractElementType }));
        }
        else if (contractProperty.Type.TypeKind == TypeKind.Interface)
        {
            if (path.Contains(contractProperty.Type, SymbolEqualityComparer.Default))
                expression = CreateDefault(contractProperty.Type);
            else
                expression = CreateAnonymousObject(contractProperty.Type, path.Concat(new[] { contractProperty.Type }));
        }
        else if (contractProperty.Type.IsNullable(out var nullableTypeArgument))
        {
            expression = CreateDefaultNullable(nullableTypeArgument);
        }
        else
        {
            expression = CreateDefault(contractProperty.Type);
        }

        return SyntaxFactory
            .AnonymousObjectMemberDeclarator(SyntaxFactory.NameEquals(contractProperty.Name), expression)
            .WithAdditionalAnnotations(Formatter.Annotation)
            .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);
    }

    private static ExpressionSyntax CreateEmptyArray(ITypeSymbol type)
    {
        return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("Array"),
                    SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier("Empty"))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                    SyntaxFactory.IdentifierName(type.Name))))))
            .NormalizeWhitespace();
    }

    private static ImplicitArrayCreationExpressionSyntax CreateImplicitArray(ITypeSymbol type,
        IEnumerable<ITypeSymbol> path)
    {
        ExpressionSyntax node;
        if (type.TypeKind == TypeKind.Interface)
            node = CreateAnonymousObject(type, path);
        else
            node = CreateDefault(type);

        ExpressionSyntax[] nodes = { node };
        var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)
            .WithExpressions(SyntaxFactory.SeparatedList(nodes));
        return SyntaxFactory.ImplicitArrayCreationExpression(initializer)
            .WithAdditionalAnnotations(Formatter.Annotation);
    }

    private static AnonymousObjectCreationExpressionSyntax CreateAnonymousObject(ITypeSymbol type,
        IEnumerable<ITypeSymbol> path)
    {
        var propertiesToAdd = CreateProperties(type, path);
        return SyntaxFactory.AnonymousObjectCreationExpression()
            .WithInitializers(SyntaxFactory.SeparatedList(propertiesToAdd))
            .WithAdditionalAnnotations(Formatter.Annotation);
    }

    private static DefaultExpressionSyntax CreateDefault(ITypeSymbol type)
    {
        return SyntaxFactory.DefaultExpression(SyntaxFactory.ParseTypeName(type.Name)
            .WithAdditionalAnnotations(Simplifier.Annotation));
    }

    private static DefaultExpressionSyntax CreateDefaultNullable(ITypeSymbol type)
    {
        return SyntaxFactory.DefaultExpression(
            SyntaxFactory.NullableType(SyntaxFactory.ParseTypeName(type.Name)
                .WithAdditionalAnnotations(Simplifier.Annotation)));
    }
}