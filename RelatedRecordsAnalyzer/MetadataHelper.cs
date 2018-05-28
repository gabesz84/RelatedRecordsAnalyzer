﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using System;
using System.Collections.Generic;

namespace Rappen.XTB.RRA
{
    public static class MetadataHelper
    {
        #region Public Fields

        public static String[] attributeProperties = { "DisplayName", "Description", "AttributeType", "IsManaged", "IsCustomizable", "IsCustomAttribute", "IsValidForCreate", "IsPrimaryName", "SchemaName", "MaxLength" };
        public static String[] entityDetails = { "Attributes" };
        public static String[] entityProperties = { "LogicalName", "DisplayName", "DisplayCollectionName", "PrimaryIdAttribute", "PrimaryNameAttribute", "ObjectTypeCode", "IsManaged", "IsCustomizable", "IsCustomEntity", "IsIntersect", "IsValidForAdvancedFind", "OneToManyRelationships" };

        #endregion Public Fields

            #region Private Fields

        private static Dictionary<string, EntityMetadata> entities = new Dictionary<string, EntityMetadata>();

        #endregion Private Fields

        #region Public Methods

        public static AttributeMetadata GetAttribute(IOrganizationService service, string entity, string attribute, object value)
        {
            if (value is AliasedValue)
            {
                var aliasedValue = value as AliasedValue;
                entity = aliasedValue.EntityLogicalName;
                attribute = aliasedValue.AttributeLogicalName;
            }
            return GetAttribute(service, entity, attribute);
        }

        public static AttributeMetadata GetAttribute(IOrganizationService service, string entity, string attribute)
        {
            if (!entities.ContainsKey(entity))
            {
                var response = LoadEntityDetails(service, entity);
                if (response != null && response.EntityMetadata != null && response.EntityMetadata.Count == 1 && response.EntityMetadata[0].LogicalName == entity)
                {
                    entities.Add(entity, response.EntityMetadata[0]);
                }
            }
            if (entities != null && entities.ContainsKey(entity))
            {
                if (entities[entity].Attributes != null)
                {
                    foreach (var metaattribute in entities[entity].Attributes)
                    {
                        if (metaattribute.LogicalName == attribute)
                        {
                            return metaattribute;
                        }
                    }
                }
            }
            return null;
        }

        public static RetrieveMetadataChangesResponse LoadEntities(IOrganizationService service)
        {
            if (service == null)
            {
                return null;
            }
            var eqe = new EntityQueryExpression();
            eqe.Properties = new MetadataPropertiesExpression(entityProperties);
            var req = new RetrieveMetadataChangesRequest()
            {
                Query = eqe,
                ClientVersionStamp = null
            };
            return service.Execute(req) as RetrieveMetadataChangesResponse;
        }

        public static RetrieveMetadataChangesResponse LoadEntityDetails(IOrganizationService service, string entityName)
        {
            if (service == null)
            {
                return null;
            }
            var eqe = new EntityQueryExpression();
            eqe.Properties = new MetadataPropertiesExpression(entityProperties);
            eqe.Properties.PropertyNames.AddRange(entityDetails);
            eqe.Criteria.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityName));
            var aqe = new AttributeQueryExpression();
            aqe.Properties = new MetadataPropertiesExpression(attributeProperties);
            eqe.AttributeQuery = aqe;
            var req = new RetrieveMetadataChangesRequest()
            {
                Query = eqe,
                ClientVersionStamp = null
            };
            return service.Execute(req) as RetrieveMetadataChangesResponse;
        }

        #endregion Public Methods
    }
}