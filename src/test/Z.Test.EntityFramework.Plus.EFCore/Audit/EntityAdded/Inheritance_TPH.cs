﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if EF5 || EF6
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Z.EntityFramework.Plus;
#if EF5 || EF6
using System.Data.Entity;

#elif EFCORE
using Microsoft.EntityFrameworkCore;

#endif

namespace Z.Test.EntityFramework.Plus
{
    public partial class Audit_EntityAdded
    {
        [TestMethod]
        public void Inheritance_TPH()
        {
            var identitySeed = TestContext.GetIdentitySeed(x => x.Inheritance_TPH_Animals, Inheritance_TPH_Dog.Create);

            TestContext.DeleteAll(x => x.AuditEntryProperties);
            TestContext.DeleteAll(x => x.AuditEntries);
            TestContext.DeleteAll(x => x.Inheritance_TPH_Animals);

            var audit = AuditHelper.AutoSaveAudit();

            using (var ctx = new TestContext())
            {
                TestContext.Insert(ctx, x => x.Inheritance_TPH_Animals, Inheritance_TPH_Cat.Create, 1);
                TestContext.Insert(ctx, x => x.Inheritance_TPH_Animals, Inheritance_TPH_Dog.Create, 2);
                ctx.SaveChanges(audit);
            }

            // UnitTest - Audit
            {
                var entries = audit.Entries;

                // Entries
                {
                    // Entries Count
                    Assert.AreEqual(3, entries.Count);

                    // Entries State
                    Assert.AreEqual(AuditEntryState.EntityAdded, entries[0].State);
                    Assert.AreEqual(AuditEntryState.EntityAdded, entries[1].State);
                    Assert.AreEqual(AuditEntryState.EntityAdded, entries[2].State);

                    // Entries EntitySetName
                    Assert.AreEqual(TestContext.TypeName(x => x.Inheritance_TPH_Animals), entries[0].EntitySetName);
                    Assert.AreEqual(TestContext.TypeName(x => x.Inheritance_TPH_Animals), entries[1].EntitySetName);
                    Assert.AreEqual(TestContext.TypeName(x => x.Inheritance_TPH_Animals), entries[2].EntitySetName);

                    // Entries TypeName
                    Assert.AreEqual(typeof (Inheritance_TPH_Cat).Name, entries[0].EntityTypeName);
                    Assert.AreEqual(typeof (Inheritance_TPH_Dog).Name, entries[1].EntityTypeName);
                    Assert.AreEqual(typeof (Inheritance_TPH_Dog).Name, entries[2].EntityTypeName);
                }

                // Properties
                {
                    var propertyIndex = -1;

                    // Properties Count
                    Assert.AreEqual(3, entries[0].Properties.Count);
                    Assert.AreEqual(3, entries[1].Properties.Count);
                    Assert.AreEqual(3, entries[2].Properties.Count);

                    // ID
                    propertyIndex = 0;
                    Assert.AreEqual("ID", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("ID", entries[1].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("ID", entries[2].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(null, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(null, entries[1].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(null, entries[2].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(identitySeed + 1, entries[0].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(identitySeed + 2, entries[1].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(identitySeed + 3, entries[2].Properties[propertyIndex].NewValue);

                    // ColumnInt
                    propertyIndex = 1;
                    Assert.AreEqual("ColumnInt", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("ColumnInt", entries[1].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("ColumnInt", entries[2].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(null, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(null, entries[1].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(null, entries[2].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(0, entries[0].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(0, entries[1].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(1, entries[2].Properties[propertyIndex].NewValue);

                    // ColumnCat | ColumnDog
                    propertyIndex = 2;
                    Assert.AreEqual("ColumnCat", entries[0].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("ColumnDog", entries[1].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual("ColumnDog", entries[2].Properties[propertyIndex].PropertyName);
                    Assert.AreEqual(null, entries[0].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(null, entries[1].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(null, entries[2].Properties[propertyIndex].OldValue);
                    Assert.AreEqual(0, entries[0].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(0, entries[1].Properties[propertyIndex].NewValue);
                    Assert.AreEqual(1, entries[2].Properties[propertyIndex].NewValue);
                }
            }

            // UnitTest - Audit (Database)
            {
                using (var ctx = new TestContext())
                {
                    // ENSURE order
                    var entries = ctx.AuditEntries.OrderBy(x => x.AuditEntryID).Include(x => x.Properties).ToList();
                    entries.ForEach(x => x.Properties = x.Properties.OrderBy(y => y.AuditEntryPropertyID).ToList());

                    // Entries
                    {
                        // Entries Count
                        Assert.AreEqual(3, entries.Count);

                        // Entries State
                        Assert.AreEqual(AuditEntryState.EntityAdded, entries[0].State);
                        Assert.AreEqual(AuditEntryState.EntityAdded, entries[1].State);
                        Assert.AreEqual(AuditEntryState.EntityAdded, entries[2].State);

                        // Entries EntitySetName
                        Assert.AreEqual(TestContext.TypeName(x => x.Inheritance_TPH_Animals), entries[0].EntitySetName);
                        Assert.AreEqual(TestContext.TypeName(x => x.Inheritance_TPH_Animals), entries[1].EntitySetName);
                        Assert.AreEqual(TestContext.TypeName(x => x.Inheritance_TPH_Animals), entries[2].EntitySetName);

                        // Entries TypeName
                        Assert.AreEqual(typeof (Inheritance_TPH_Cat).Name, entries[0].EntityTypeName);
                        Assert.AreEqual(typeof (Inheritance_TPH_Dog).Name, entries[1].EntityTypeName);
                        Assert.AreEqual(typeof (Inheritance_TPH_Dog).Name, entries[2].EntityTypeName);
                    }

                    // Properties
                    {
                        var propertyIndex = -1;

                        // Properties Count
                        Assert.AreEqual(3, entries[0].Properties.Count);
                        Assert.AreEqual(3, entries[1].Properties.Count);
                        Assert.AreEqual(3, entries[2].Properties.Count);

                        // ID
                        propertyIndex = 0;
                        Assert.AreEqual("ID", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("ID", entries[1].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("ID", entries[2].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual(null, entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(null, entries[1].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(null, entries[2].Properties[propertyIndex].OldValue);
                        Assert.AreEqual((identitySeed + 1).ToString(), entries[0].Properties[propertyIndex].NewValue);
                        Assert.AreEqual((identitySeed + 2).ToString(), entries[1].Properties[propertyIndex].NewValue);
                        Assert.AreEqual((identitySeed + 3).ToString(), entries[2].Properties[propertyIndex].NewValue);

                        // ColumnInt
                        propertyIndex = 1;
                        Assert.AreEqual("ColumnInt", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("ColumnInt", entries[1].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("ColumnInt", entries[2].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual(null, entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(null, entries[1].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(null, entries[2].Properties[propertyIndex].OldValue);
                        Assert.AreEqual("0", entries[0].Properties[propertyIndex].NewValue);
                        Assert.AreEqual("0", entries[1].Properties[propertyIndex].NewValue);
                        Assert.AreEqual("1", entries[2].Properties[propertyIndex].NewValue);

                        // ColumnCat | ColumnDog
                        propertyIndex = 2;
                        Assert.AreEqual("ColumnCat", entries[0].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("ColumnDog", entries[1].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual("ColumnDog", entries[2].Properties[propertyIndex].PropertyName);
                        Assert.AreEqual(null, entries[0].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(null, entries[1].Properties[propertyIndex].OldValue);
                        Assert.AreEqual(null, entries[2].Properties[propertyIndex].OldValue);
                        Assert.AreEqual("0", entries[0].Properties[propertyIndex].NewValue);
                        Assert.AreEqual("0", entries[1].Properties[propertyIndex].NewValue);
                        Assert.AreEqual("1", entries[2].Properties[propertyIndex].NewValue);
                    }
                }
            }
        }
    }
}

#endif