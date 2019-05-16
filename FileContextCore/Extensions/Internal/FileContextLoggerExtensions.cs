// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using FileContextCore.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Update;

// ReSharper disable once CheckNamespace
namespace FileContextCore.Extensions.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    static class FileContextLoggerExtensions
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void TransactionIgnoredWarning(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> diagnostics)
        {
            //EventDefinition definition = FileContextStrings.LogTransactionsNotSupported;

            //definition.Log(diagnostics, WarningBehavior.Log);

            //if (diagnostics.DiagnosticSource.IsEnabled(definition.EventId.Name))
            //{
            //    diagnostics.DiagnosticSource.Write(
            //        definition.EventId.Name, 
            //        new EventData(
            //            definition,
            //            (d, p) => ((EventDefinition)d).GenerateMessage()));
            //}
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void ChangesSaved(
            [NotNull] this IDiagnosticsLogger<DbLoggerCategory.Update> diagnostics,
            [NotNull] IEnumerable<IUpdateEntry> entries,
            int rowsAffected)
        {
            //var definition = FileContextStrings.LogSavedChanges;

            //definition.Log(diagnostics, rowsAffected);

            //if (diagnostics.DiagnosticSource.IsEnabled(definition.EventId.Name))
            //{
            //    diagnostics.DiagnosticSource.Write(
            //        definition.EventId.Name,
            //        new SaveChangesEventData(
            //            definition,
            //            ChangesSaved,
            //            entries,
            //            rowsAffected));
            //}
        }

        private static string ChangesSaved(EventDefinitionBase definition, EventData payload)
        {
            EventDefinition<int> d = (EventDefinition<int>)definition;
            SaveChangesEventData p = (SaveChangesEventData)payload;
            return d.GenerateMessage(p.RowsAffected);
        }
    }
}
