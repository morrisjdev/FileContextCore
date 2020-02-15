// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FileContextCore.Diagnostics.Internal
{

    public class FileContextLoggingDefinitions : LoggingDefinitions
    {
    
        public EventDefinitionBase LogSavedChanges;

    
        public EventDefinitionBase LogTransactionsNotSupported;
    }
}
