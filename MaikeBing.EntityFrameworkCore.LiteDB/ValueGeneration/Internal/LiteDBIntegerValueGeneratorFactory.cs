// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microsoft.EntityFrameworkCore.InMemory.ValueGeneration.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class LiteDBIntegerValueGeneratorFactory : ValueGeneratorFactory
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ValueGenerator Create(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            var type = property.ClrType.UnwrapNullableType().UnwrapEnumType();

            if (type == typeof(long))
            {
                return new LiteDBIntegerValueGenerator<long>();
            }

            if (type == typeof(int))
            {
                return new LiteDBIntegerValueGenerator<int>();
            }

            if (type == typeof(short))
            {
                return new LiteDBIntegerValueGenerator<short>();
            }

            if (type == typeof(byte))
            {
                return new LiteDBIntegerValueGenerator<byte>();
            }

            if (type == typeof(ulong))
            {
                return new LiteDBIntegerValueGenerator<ulong>();
            }

            if (type == typeof(uint))
            {
                return new LiteDBIntegerValueGenerator<uint>();
            }

            if (type == typeof(ushort))
            {
                return new LiteDBIntegerValueGenerator<ushort>();
            }

            if (type == typeof(sbyte))
            {
                return new LiteDBIntegerValueGenerator<sbyte>();
            }

            throw new ArgumentException(
                CoreStrings.InvalidValueGeneratorFactoryProperty(
                    nameof(LiteDBIntegerValueGeneratorFactory), property.Name, property.DeclaringEntityType.DisplayName()));
        }
    }
}
