using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Gig.Framework.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncMethodAnalyzer :
    DiagnosticAnalyzer
{
    public const string MissingAwaitRuleId = "MTA0001";

    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor MissingAwaitRule = new(MissingAwaitRuleId,
        "MassTransit method is not awaited or captured",
        "Method {0} is not awaited or captured and may result in message loss",
        Category, DiagnosticSeverity.Warning, true,
        "MassTransit method is not awaited or captured.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingAwaitRule);

    public override void Initialize(AnalysisContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        var symbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, invocationExpression);
        if (symbol.Symbol?.Kind == SymbolKind.Method)
        {
            var methodSymbol = (IMethodSymbol)symbol.Symbol;

            if (methodSymbol.IsProducerMethod(out _) && methodSymbol.ReturnsTask())
                if (invocationExpression.Parent is ExpressionStatementSyntax)
                    context.ReportDiagnostic(Diagnostic.Create(MissingAwaitRule, invocationExpression.GetLocation(),
                        SymbolDisplay.ToDisplayString(methodSymbol,
                            SymbolDisplayFormat.CSharpShortErrorMessageFormat.WithParameterOptions(
                                SymbolDisplayParameterOptions.None))));
        }
    }
}