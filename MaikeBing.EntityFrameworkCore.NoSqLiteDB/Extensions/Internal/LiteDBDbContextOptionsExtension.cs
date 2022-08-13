using MaikeBing.EntityFrameworkCore.NoSqLiteDB.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaikeBing.EntityFrameworkCore.NoSqLiteDB.Extensions.Internal
{
    internal class LiteDBDbContextOptionsExtension : IDbContextOptionsExtension
    {
        public DbContextOptionsExtensionInfo Info => new LiteDBOptionsExtension(this);

        public void ApplyServices(IServiceCollection services)
        {
        
        }

        public void Validate(IDbContextOptions options)
        {
        }
    }
}
