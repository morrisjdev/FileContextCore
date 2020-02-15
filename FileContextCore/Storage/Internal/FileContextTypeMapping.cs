// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FileContextCore.Storage.Internal
{

    public class FileContextTypeMapping : CoreTypeMapping
    {
    
        public FileContextTypeMapping(
            [NotNull] Type clrType,
            [CanBeNull] ValueComparer comparer = null,
            [CanBeNull] ValueComparer keyComparer = null,
            [CanBeNull] ValueComparer structuralComparer = null)
            : base(
                new CoreTypeMappingParameters(
                    clrType,
                    converter: null,
                    comparer,
                    keyComparer,
                    structuralComparer,
                    valueGeneratorFactory: null))
        {
        }

        private FileContextTypeMapping(CoreTypeMappingParameters parameters)
            : base(parameters)
        {
        }

    
        public override CoreTypeMapping Clone(ValueConverter converter)
            => new FileContextTypeMapping(Parameters.WithComposedConverter(converter));
    }
}
