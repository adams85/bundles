﻿using System.Collections.Generic;
using System.Threading;
using Karambolo.AspNetCore.Bundling;
using Karambolo.AspNetCore.Bundling.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomBundleManager
{
    public class CustomBundleManagerFactory : IBundleManagerFactory
    {
        private readonly IEnumerable<IBundleModelFactory> _modelFactories;
        private readonly IBundleCache _cache;
        private readonly IBundleVersionProvider _versionProvider;
        private readonly IBundleUrlHelper _urlHelper;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISystemClock _clock;
        private readonly List<IBundleManager> _instances;
        private readonly CancellationToken _shutdownToken;
        private readonly IOptions<BundleGlobalOptions> _globalOptions;

        public CustomBundleManagerFactory(IEnumerable<IBundleModelFactory> modelFactories, IBundleCache cache, IBundleVersionProvider versionProvider, IBundleUrlHelper urlHelper,
            ILoggerFactory loggerFactory, ISystemClock clock, IHostApplicationLifetime applicationLifetime, IOptions<BundleGlobalOptions> globalOptions)
        {
            _modelFactories = modelFactories;
            _cache = cache;
            _versionProvider = versionProvider;
            _urlHelper = urlHelper;

            _loggerFactory = loggerFactory;
            _clock = clock;

            _shutdownToken = applicationLifetime.ApplicationStopping;

            _globalOptions = globalOptions;

            _instances = new List<IBundleManager>();
        }

        public IReadOnlyList<IBundleManager> Instances => _instances;

        public IBundleManager Create(BundleCollection bundles, IBundlingContext bundlingContext)
        {
            var result = new CustomBundleManager(
                _instances.Count, bundles, bundlingContext, _shutdownToken, _modelFactories, _cache, _versionProvider, _urlHelper,
                _loggerFactory, _clock, _globalOptions);

            _instances.Add(result);
            return result;
        }
    }
}
