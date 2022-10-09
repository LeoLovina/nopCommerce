﻿using System.Threading.Tasks;
﻿using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Services.Caching;

namespace Nop.Services.Directory.Caching
{
    /// <summary>
    /// Represents a city cache event consumer
    /// </summary>
    public partial class CityCacheEventConsumer : CacheEventConsumer<City>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(City entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(NopEntityCacheDefaults<City>.Prefix);
        }
    }
}
