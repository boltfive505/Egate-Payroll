﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace System.Data.SQLite.EF6.Configuration
{
    internal class SQLiteDbProviderFactoryResolver : IDbProviderFactoryResolver
    {
        public static readonly SQLiteDbProviderFactoryResolver Instance = new SQLiteDbProviderFactoryResolver();
        private SQLiteDbProviderFactoryResolver() { }
        public DbProviderFactory ResolveProviderFactory(DbConnection connection)
        {
            if (connection is SQLiteConnection) return SQLiteProviderFactory.Instance;
            if (connection is EntityConnection) return EntityProviderFactory.Instance;
            return null;
        }
    }
}
