//---------------------------------------------------------------------
// <copyright file="ProviderServices.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.SQLite
{
	sealed class DdlBuilder
	{
		private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly HashSet<EntitySet> ignoredEntitySets = new HashSet<EntitySet>();
        internal static bool GenerateForeignKeys { get; set; }

		internal static string CreateObjectsScript(StoreItemCollection itemCollection)
		{
			DdlBuilder builder = new DdlBuilder();

			builder.AppendSql(@"
CREATE TABLE IF NOT EXISTS [__MigrationHistory] (
    [MigrationId] [TEXT] NOT NULL,
    [ContextKey] [TEXT] NOT NULL,
    [Model] [BLOB] NOT NULL,
    [ProductVersion] [TEXT] NOT NULL,
    PRIMARY KEY ([MigrationId], [ContextKey])
);
");

			foreach (EntityContainer container in itemCollection.GetItems<EntityContainer>())
			{
                
				var entitySets = container.BaseEntitySets.OfType<EntitySet>().OrderBy(s => s.Name);

				//var schemas = new HashSet<string>(entitySets.Select(s => GetSchemaName(s)));
				//foreach (string schema in schemas.OrderBy(s => s))
				//{
				//    // don't bother creating default schema
				//    if (schema != "dbo")
				//    {
				//        builder.AppendCreateSchema(schema);
				//    }
				//}

				foreach (EntitySet entitySet in container.BaseEntitySets.OfType<EntitySet>().OrderBy(s => s.Name))
				{
					builder.AppendCreateTable(entitySet);
				}

				// SQLITE does not support the ADD CONSTRAINT to the alter table command, but we can fake it with triggers
                if (GenerateForeignKeys)
                {
                    foreach (AssociationSet associationSet in container.BaseEntitySets.OfType<AssociationSet>().OrderBy(s => s.Name))
                    {
                        builder.AppendCreateForeignKeys(associationSet);
                    }
                }
                GenerateForeignKeys = false;
			}
			return builder.GetCommandText();
		}

		//internal static string CreateDatabaseScript(string databaseName, string dataFileName, string logFileName)
		//{
		//    var builder = new DdlBuilder();
		//    builder.AppendSql("create database ");
		//    builder.AppendIdentifier(databaseName);
		//    if (null != dataFileName)
		//    {
		//        builder.AppendSql(" on primary ");
		//        builder.AppendFileName(dataFileName);
		//        builder.AppendSql(" log on ");
		//        builder.AppendFileName(logFileName);
		//    }
		//    return builder.stringBuilder.ToString();
		//}

		//internal static string CreateDatabaseExistsScript(string databaseName)
		//{
		//    var builder = new DdlBuilder();
		//    builder.AppendSql("SELECT Count(*) FROM ");
		//    builder.AppendSql("sys.databases");
		//    builder.AppendSql(" WHERE [name]=");
		//    builder.AppendStringLiteral(databaseName);
		//    return builder.stringBuilder.ToString();
		//}

		//internal static string DropDatabaseScript(string databaseName)
		//{
		//    var builder = new DdlBuilder();
		//    builder.AppendSql("drop database ");
		//    builder.AppendIdentifier(databaseName);
		//    return builder.stringBuilder.ToString();
		//}

		internal string GetCommandText()
		{
			return this.stringBuilder.ToString();
		}

		private static string GetSchemaName(EntitySet entitySet)
		{
			var schemaName = entitySet.MetadataProperties["Schema"].Value as string;
			return schemaName ?? entitySet.EntityContainer.Name;
		}

		private static string GetTableName(EntitySet entitySet)
		{
			string tableName = entitySet.MetadataProperties["Table"].Value as string;
			return tableName ?? entitySet.Name;
		}

		private void AppendCreateForeignKeys(AssociationSet associationSet)
		{
			var constraint = associationSet.ElementType.ReferentialConstraints.Single();
			var principalEnd = associationSet.AssociationSetEnds[constraint.FromRole.Name];
			var dependentEnd = associationSet.AssociationSetEnds[constraint.ToRole.Name];

			// If any of the participating entity sets was skipped, skip the association too
			if (ignoredEntitySets.Contains(principalEnd.EntitySet) || ignoredEntitySets.Contains(dependentEnd.EntitySet))
			{
				AppendSql("-- Ignoring association set with participating entity set with defining query: ");
				AppendIdentifierEscapeNewLine(associationSet.Name);
			}
			else
			{
//				0 = FK Name
//				1 = Parent table
//				2 = Parent FK
//				3 = Child table
//				4 = Child PK
                var sql = @"
CREATE TRIGGER [FK_{0}_Insert]
	BEFORE INSERT ON [{3}]
		FOR EACH ROW BEGIN
		SELECT RAISE(ROLLBACK, 'update on table ""[{1}]"" violates foreign key constraint ""[{0}]""')
		WHERE  (SELECT 1 FROM [{1}] WHERE [{2}] = NEW.[{4}] OR NEW.[{4}] IS NULL) IS NULL;
END;
CREATE TRIGGER [FK_{0}_Update]
	BEFORE UPDATE OF [{4}] ON [{3}]
		FOR EACH ROW BEGIN
		SELECT RAISE(ROLLBACK, 'insert on table ""[{1}]"" violates foreign key constraint ""[{0}]""')
		WHERE  (SELECT 1 FROM [{1}] WHERE [{2}] = NEW.[{4}] OR NEW.[{4}] IS NULL) IS NULL;
END;";
                var trigger = string.Format(sql, associationSet.Name, principalEnd.EntitySet.Table, constraint.ToProperties.First().Name, dependentEnd.EntitySet.Table, constraint.ToProperties.First().Name);
                AppendSql(trigger);
                if (principalEnd.CorrespondingAssociationEndMember.DeleteBehavior == OperationAction.Cascade)
                {
                    sql = @"
CREATE TRIGGER [FK_{0}_Delete]
	BEFORE DELETE ON [{1}]
		FOR EACH ROW BEGIN
		DELETE FROM [{3}] WHERE [{4}] = OLD.[{2}];
END;";
                    trigger = string.Format(sql, associationSet.Name, principalEnd.EntitySet.Table, constraint.ToProperties.First().Name, dependentEnd.EntitySet.Table, constraint.ToProperties.First().Name);
                    AppendSql(trigger);
                }
                else
                {
                    sql = @"
CREATE TRIGGER [FK_{0}_Delete]
	BEFORE DELETE ON [{1}]
		FOR EACH ROW BEGIN
		SELECT RAISE(ROLLBACK, 'delete on table ""[{1}]"" violates foreign key constraint ""[{0}]""')
		WHERE  (SELECT 1 FROM [{3}] WHERE [{4}] = OLD.[{2}]) IS NOT NULL;
END;";
                    trigger = string.Format(sql, associationSet.Name, principalEnd.EntitySet.Table, constraint.ToProperties.First().Name, dependentEnd.EntitySet.Table, constraint.ToProperties.First().Name);
                    AppendSql(trigger);
                }
                //AppendSql("alter table ");
                //AppendIdentifier(dependentEnd.EntitySet);
                //AppendSql(" add constraint ");
                //AppendIdentifier(associationSet.Name);
                //AppendSql(" foreign key (");
                //AppendIdentifiers(constraint.ToProperties);
                //AppendSql(") references ");
                //AppendIdentifier(principalEnd.EntitySet);
                //AppendSql("(");
                //AppendIdentifiers(constraint.FromProperties);
                //AppendSql(")");
                
				//AppendSql(";");
			}
			AppendNewLine();
		}

		private void AppendCreateTable(EntitySet entitySet)
		{
			//If the entity set has defining query, skip it
			if (entitySet.MetadataProperties["DefiningQuery"].Value != null)
			{
				AppendSql("-- Ignoring entity set with defining query: ");
				AppendIdentifier(entitySet, AppendIdentifierEscapeNewLine);
				ignoredEntitySets.Add(entitySet);
			}
			else
			{
				AppendSql("CREATE TABLE IF NOT EXISTS ");
				AppendIdentifier(entitySet);
				AppendSql(" (");
				AppendNewLine();
				bool first = true;
				foreach (EdmProperty column in entitySet.ElementType.Properties)
				{
					if (first) { first = false; }
					else
					{
						AppendSql(",");
						AppendNewLine();
					}
					AppendSql("    ");
					AppendIdentifier(column.Name);
					AppendSql(" ");
					AppendType(column);
				}

				if (entitySet.ElementType.KeyMembers.Count > 1)
				{
                    AppendSql(",");
                    AppendSql(Environment.NewLine);
					AppendSql("    PRIMARY KEY (");
					AppendJoin(entitySet.ElementType.KeyMembers, k => AppendIdentifier(k.Name), ", ");
					AppendSql(")");
					AppendNewLine();
				}
				AppendSql(");");
			}
			AppendNewLine();
		}

		// SQLite does not support schemas
		//private void AppendCreateSchema(string schema)
		//{
		//    AppendSql("if (schema_id(");
		//    AppendStringLiteral(schema);
		//    AppendSql(") is null) exec(");

		//    // need to create a sub-command and escape it as a string literal as well...
		//    DdlBuilder schemaBuilder = new DdlBuilder();
		//    schemaBuilder.AppendSql("create schema ");
		//    schemaBuilder.AppendIdentifier(schema);

		//    AppendStringLiteral(schemaBuilder.stringBuilder.ToString());
		//    AppendSql(");");
		//    AppendNewLine();
		//}

		private void AppendIdentifier(EntitySet table)
		{
			AppendIdentifier(table, AppendIdentifier);
		}

		private void AppendIdentifier(EntitySet table, Action<string> AppendIdentifierEscape)
		{
			//string schemaName = GetSchemaName(table);
			string tableName = GetTableName(table);
			//if (schemaName != null)
			//{
			//    AppendIdentifierEscape(schemaName);
			//    AppendSql(".");
			//}
			AppendIdentifierEscape(tableName);
		}

		private void AppendStringLiteral(string literalValue)
		{
			AppendSql("N'" + literalValue.Replace("'", "''") + "'");
		}

		private void AppendIdentifiers(IEnumerable<EdmProperty> properties)
		{
			AppendJoin(properties, p => AppendIdentifier(p.Name), ", ");
		}

		private void AppendIdentifier(string identifier)
		{
			AppendSql("[" + identifier.Replace("]", "]]") + "]");
		}

		private void AppendIdentifierEscapeNewLine(string identifier)
		{
			AppendIdentifier(identifier.Replace("\r", "\r--").Replace("\n", "\n--"));
		}

		private void AppendFileName(string path)
		{
			AppendSql("(name=");
			AppendStringLiteral(Path.GetFileName(path));
			AppendSql(", filename=");
			AppendStringLiteral(path);
			AppendSql(")");
		}

		private void AppendJoin<T>(IEnumerable<T> elements, Action<T> appendElement, string unencodedSeparator)
		{
			bool first = true;
			foreach (T element in elements)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					AppendSql(unencodedSeparator);
				}
				appendElement(element);
			}
		}

		private void AppendType(EdmProperty column)
		{
			TypeUsage type = column.TypeUsage;

			// check for rowversion-like configurations
			Facet storeGenFacet;
			bool isTimestamp = false;
			if (type.EdmType.Name == "rowversion")
			{
				isTimestamp = true;
				AppendIdentifier("ROWVERSION");
                // row Versions are not set by EF and not supporting in SQLite, so we will fake it
                AppendSql(@" NOT NULL DEFAULT X'01'");
			}
			else
			{
				string typeName = type.EdmType.Name;
				if (typeName == "nvarchar" ||
					typeName == "nchar" ||
					typeName == "varchar" ||
					typeName == "char")
				{
					typeName = "text";
				}

				// Special case: the EDM treats 'nvarchar(max)' as a type name, but SQL Server treats
				// it as a type 'nvarchar' and a type qualifier. As such, we can't escape the entire
				// type name as the EDM sees it.
				const string maxSuffix = "(max)";
				if (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType && typeName.EndsWith(maxSuffix, StringComparison.Ordinal))
				{
					//Debug.Assert(new[] { "nvarchar(max)", "varchar(max)", "varbinary(max)" }.Contains(typeName),
					//    "no other known SQL Server primitive types types accept (max)");
					AppendIdentifier(typeName.Substring(0, typeName.Length - maxSuffix.Length));
					//AppendSql("(max)");
				}
				else
				{
					AppendIdentifier(typeName.ToUpper());
                }
                AppendSql(column.Nullable ? " NULL" : " NOT NULL");
			}

			if (!isTimestamp && column.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out storeGenFacet) &&
				storeGenFacet.Value != null)
			{
				StoreGeneratedPattern storeGenPattern = (StoreGeneratedPattern)storeGenFacet.Value;
				if (storeGenPattern == StoreGeneratedPattern.Identity)
				{
					if (type.EdmType.Name == "uniqueidentifier")
					{
						AppendSql(" default newid()");
					}
					else if (type.EdmType.Name == "integer")
					{
						AppendSql(" PRIMARY KEY AUTOINCREMENT");
					}
					else
					{
						throw new NotSupportedException("Primary Key columns must be of .NET type Int32, Int64, or Guid.  Non-composite keys of other types are not supported.");
					}
				}
			}
		}

		/// <summary>
		/// Appends raw SQL into the string builder.
		/// </summary>
		/// <param name="text">Raw SQL string to append into the string builder.</param>
		private void AppendSql(string text)
		{
			stringBuilder.Append(text);
		}

		/// <summary>
		/// Appends new line for visual formatting or for ending a comment.
		/// </summary>
		private void AppendNewLine()
		{
			stringBuilder.Append(Environment.NewLine);
		}

		/// <summary>
		/// Append raw SQL into the string builder with formatting options and invariant culture formatting.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An array of objects to format.</param>
		private void AppendSqlInvariantFormat(string format, params object[] args)
		{
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, args);
		}

    }

	internal static class MetadataHelper
	{
		internal static byte GetPrecision(this TypeUsage type)
		{
			return type.GetFacetValue<byte>("Precision");
		}

		internal static byte GetScale(this TypeUsage type)
		{
			return type.GetFacetValue<byte>("Scale");
		}

		internal static int GetMaxLength(this TypeUsage type)
		{
			return type.GetFacetValue<int>("MaxLength");
		}

		internal static T GetFacetValue<T>(this TypeUsage type, string facetName)
		{
			return (T)type.Facets[facetName].Value;
		}
	}

}
