﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    internal static class ValidationExtensions
    {
        internal static async Task<IEnumerable<IError>> ValidateAsync<TRequest>(this IServiceProvider serviceProvider, IExecutionContext context, TRequest request, ILogger logger)
        {
            var validators = serviceProvider.GetServices<IValidatorAsync<TRequest>>()
                                ?.Where(s => s.IsApplicable(context, request))
                                ?? Enumerable.Empty<IValidatorAsync<TRequest>>();

            foreach (var val in validators)
            {
                var errors = await ExecutionHelper.Execute(() => val.Validate(context, request), val, logger);

                if (errors != null && errors.Any())
                {
                    return errors;
                }
            }

            return Enumerable.Empty<IError>();
        }
    }
}