﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpGen.Model;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpGen.Generator.Marshallers
{
    internal class ValueTypeArrayMarshaller : MarshallerBase, IMarshaller
    {
        public ValueTypeArrayMarshaller(GlobalNamespaceProvider globalNamespace) : base(globalNamespace)
        {
        }

        public bool CanMarshal(CsMarshalBase csElement) => csElement.IsValueType && csElement.IsArray &&
                                                           !csElement.MappedToDifferentPublicType &&
                                                           csElement is not CsField;

        public ArgumentSyntax GenerateManagedArgument(CsParameter csElement) =>
            Argument(IdentifierName(csElement.Name));

        public ParameterSyntax GenerateManagedParameter(CsParameter csElement) =>
            GenerateManagedArrayParameter(csElement);

        public StatementSyntax GenerateManagedToNative(CsMarshalBase csElement, bool singleStackFrame)
        {
            if (csElement is CsParameter parameter && (parameter.IsRef || parameter.IsOut))
            {
                return GenerateCopyBlock(parameter, CopyBlockDirection.FixedArrayToUnmanaged);
            }

            return null;
        }

        public IEnumerable<StatementSyntax> GenerateManagedToNativeProlog(CsMarshalCallableBase csElement) =>
            Enumerable.Empty<StatementSyntax>();

        public ArgumentSyntax GenerateNativeArgument(CsMarshalCallableBase csElement) =>
            Argument(GetMarshalStorageLocation(csElement));

        public StatementSyntax GenerateNativeCleanup(CsMarshalBase csElement, bool singleStackFrame) => null;

        public StatementSyntax GenerateNativeToManaged(CsMarshalBase csElement, bool singleStackFrame)
        {
            if (csElement is CsParameter parameter && (parameter.IsRef || parameter.IsRefIn))
            {
                return GenerateCopyBlock(parameter, CopyBlockDirection.UnmanagedToFixedArray);
            }

            return null;
        }

        public IEnumerable<StatementSyntax> GenerateNativeToManagedExtendedProlog(CsMarshalCallableBase csElement)
        {
            yield return GenerateArrayNativeToManagedExtendedProlog(csElement);
        }

        public FixedStatementSyntax GeneratePin(CsParameter csElement) => FixedStatement(
            VariableDeclaration(
                GetMarshalTypeSyntax(csElement),
                SingletonSeparatedList(
                    VariableDeclarator(GetMarshalStorageLocationIdentifier(csElement)).WithInitializer(
                        EqualsValueClause(IdentifierName(csElement.Name))
                    )
                )
            ),
            EmptyStatement()
        );

        public bool GeneratesMarshalVariable(CsMarshalCallableBase csElement) => true;

        public TypeSyntax GetMarshalTypeSyntax(CsMarshalBase csElement) =>
            PointerType(ParseTypeName(csElement.PublicType.QualifiedName));

        private enum CopyBlockDirection
        {
            UnmanagedToFixedArray,
            FixedArrayToUnmanaged,
        }

        private StatementSyntax GenerateCopyBlock(CsMarshalBase parameter, CopyBlockDirection direction)
        {
            var arrayIdentifier = IdentifierName(parameter.Name);
            var pointerIdentifier = GetMarshalStorageLocationIdentifier(parameter);
            var fixedName = $"{pointerIdentifier}_";
            
            var fixedInitializer = VariableDeclarator(fixedName)
                .WithInitializer(EqualsValueClause(arrayIdentifier));
            
            var unsafeName = GlobalNamespaceProvider.GetTypeNameSyntax(BuiltinType.Unsafe);

            var destination = direction switch
            {
                CopyBlockDirection.UnmanagedToFixedArray => Identifier(fixedName),
                CopyBlockDirection.FixedArrayToUnmanaged => pointerIdentifier,
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
            
            var source = direction switch
            {
                CopyBlockDirection.UnmanagedToFixedArray => pointerIdentifier,
                CopyBlockDirection.FixedArrayToUnmanaged => Identifier(fixedName),
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
            
            var invokeArguments = ArgumentList(
                SeparatedList(
                    new[]
                    {
                        Argument(IdentifierName(destination)),
                        Argument(IdentifierName(source)),
                        Argument(
                            CastExpression(
                                PredefinedType(Token(SyntaxKind.UIntKeyword)),
                                ParenthesizedExpression(
                                    BinaryExpression(
                                        SyntaxKind.MultiplyExpression,
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            arrayIdentifier,
                                            IdentifierName("Length")
                                        ),
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                unsafeName,
                                                GenericName(
                                                    Identifier(nameof(Unsafe.SizeOf)),
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            IdentifierName(parameter.PublicType.QualifiedName)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    }
                )
            );

            return FixedStatement(
                VariableDeclaration(
                    VoidPtrType,
                    SingletonSeparatedList(fixedInitializer)
                ),
                ExpressionStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            unsafeName,
                            IdentifierName(nameof(Unsafe.CopyBlockUnaligned))
                        ),
                        invokeArguments
                    )
                )
            );
        }
    }
}
