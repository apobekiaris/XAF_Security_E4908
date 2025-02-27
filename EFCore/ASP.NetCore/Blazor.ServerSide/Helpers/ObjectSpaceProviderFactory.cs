﻿using BusinessObjectsLibrary.BusinessObjects;
using DevExpress.EntityFrameworkCore.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using Microsoft.EntityFrameworkCore;

namespace Blazor.ServerSide.Services {
    public class ObjectSpaceProviderFactory : IObjectSpaceProviderFactory {
        readonly ISecurityStrategyBase security;
        readonly ITypesInfo typesInfo;
        readonly IDbContextFactory<ApplicationDbContext> dbFactory;

        public ObjectSpaceProviderFactory(ISecurityStrategyBase security, ITypesInfo typesInfo, IDbContextFactory<ApplicationDbContext> dbFactory) {
            this.security = security;
            this.typesInfo = typesInfo;
            this.dbFactory = dbFactory;
        }

        IEnumerable<IObjectSpaceProvider> IObjectSpaceProviderFactory.CreateObjectSpaceProviders() {
            yield return new SecuredEFCoreObjectSpaceProvider<ApplicationDbContext>((ISelectDataSecurityProvider)security, dbFactory, typesInfo);
        }
    }
}
