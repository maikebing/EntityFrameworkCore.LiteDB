using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Storage.Internal
{
    public static class LiteDBStorageExtension
    {
        public static string TableName([NotNull] this IEntityType type) => type.ShortName().Replace('<', '_').Replace('>', '_');
    }
}
