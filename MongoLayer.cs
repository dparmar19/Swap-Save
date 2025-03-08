using MongoDB.Driver;
using TNG.Shared.Lib.Intefaces;
using System.Reflection;
using TNG.Shared.Lib.Mongo.Base;
using System.Collections;

namespace TNG.Shared.Lib
{
    public class MongoLayer : IMongoLayer
    {
        private IMongoDatabase _db;
        private IAuthenticationService _auth;
        private MongoOperationsMode _operationsMode;
        private ITNGUtiltityLib _utilityLib;

        public MongoLayer(IMongoClient mongoClient, IAuthenticationService authSvc, IMongoConfigurationService config, ITNGUtiltityLib utiltityLib)
        {
            // Create new database connection
            this._operationsMode = config.GetOperationMode();
            this._db = mongoClient.GetDatabase(config.GetMongoDatabase());
            this._auth = authSvc;
            this._utilityLib = utiltityLib;
        }

        /// <summary>
        /// Insert new document into collection
        /// </summary>
        /// <typeparam name="T">Document data type</typeparam>
        /// <param name="collectionName">Collection name</param>
        /// <param name="document">Document</param>
        public void InsertDocument<T>(string collectionName, T document)
        {
            document = this.setBaseFields(document);
            var collection = this._db.GetCollection<T>(collectionName);
            collection.InsertOne(document);
        }

        public void InsertManyDocument<T>(string collectionName, List<T> document)
        {
            this.setBaseFields(document);
            var collection = this._db.GetCollection<T>(collectionName);
            collection.InsertMany(document);
        }

        /// <summary>
        /// Load document by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T LoadDocumentById<T>(string collectionName, string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            var collection = this._db.GetCollection<T>(collectionName);
            var doc = collection.Find(filter).FirstOrDefault();
            doc = setLocalTime(doc);
            return doc;
        }

        /// <summary>
        /// load all documents
        /// </summary>
        /// <param name="collectionName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> LoadDocuments<T>(string collectionName, FilterDefinition<T> filter, int? limit = 100, SortDefinition<T> sort = null, int? skip = null)
        {
            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            var collection = this._db.GetCollection<T>(collectionName);
            var results = collection.Find(filter).Sort(sort).Skip(skip).Limit(limit).ToList();
            results = setDateTimes(results);
            return results;
        }

        /// <summary>
        /// load documents without operationmode
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="limit"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> LoadListOfDocuments<T>(string collectionName, FilterDefinition<T> filter, SortDefinition<T> sort = null)
        {
            filter = this.ApplyDeleteBaseFilters(filter);
            var collection = this._db.GetCollection<T>(collectionName);
            var results = collection.Find(filter).Sort(sort).ToList();
            results = setDateTimes(results);
            return results;
        }
        /// <summary>
        /// Load all documents from a collection (Max 100 records)
        /// </summary>
        /// <param name="collectionName">Collection</param>
        /// <param name="limit">No of records to pull (default limit 100)</param>
        /// <param name="sort">Sort defenition</param>
        /// <param name="skip">Skip n number of records</param>
        /// <typeparam name="T">Record T</typeparam>
        /// <returns></returns>
        public List<T> LoadAll<T>(string collectionName)
        {
            var filter = Builders<T>.Filter.Empty;
            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            var collection = this._db.GetCollection<T>(collectionName);
            var results = collection.Find(filter).Limit(100).ToList();
            results = setDateTimes(results);
            return results;
        }

        /// <summary>
        /// Update document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <param name="document"></param>
        public void UpdateDocument<T>(string collectionName, T document)
        {
            document = updateTimeToUtc(document);
            var id = document.GetType().GetProperty("Id").GetValue(document);
            var filter = Builders<T>.Filter.Eq("Id", id);
            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            document = this.setBaseFields(document);
            var collection = this._db.GetCollection<T>(collectionName);
            var result = collection.ReplaceOne(
                                    filter,
                                    document,
                                    new ReplaceOptions { IsUpsert = false }
                                    );


        }

        public void UpdateDocumentWithOutOperationFilter<T>(string collectionName, T document)
        {
            document = updateTimeToUtc(document);
            var id = document.GetType().GetProperty("Id").GetValue(document);
            var filter = Builders<T>.Filter.Eq("Id", id);
            document = this.setBaseFields(document);
            var collection = this._db.GetCollection<T>(collectionName);
            var result = collection.ReplaceOne(
                                    filter,
                                    document,
                                    new ReplaceOptions { IsUpsert = false }
                                    );


        }

        /// <summary>
        /// update the document,if no documents match, insert a new document
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="document"></param>
        /// <typeparam name="T"></typeparam>
        public void UpdateOrInsertDocument<T>(string collectionName, T document)
        {
            var id = document.GetType().GetProperty("Id").GetValue(document);
            var filter = Builders<T>.Filter.Eq("Id", id);
            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            document = this.setBaseFields(document);
            var collection = this._db.GetCollection<T>(collectionName)
                        .ReplaceOne(
                                    filter,
                                    document,
                                    new ReplaceOptions { IsUpsert = true }
                                    );
        }
        /// <summary>
        /// Delete document by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        public void DeleteDocument<T>(string collectionName, T document)
        {
            var id = document.GetType().GetProperty("Id").GetValue(document);
            var filter = Builders<T>.Filter.Eq("Id", id);
            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            var collection = this._db.GetCollection<T>(collectionName).FindOneAndDelete(filter);
        }

        /// <summary>
        /// Delete document based on filter
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        public void DeleteDocument<T>(string collectionName, FilterDefinition<T> filter)
        {
            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            var collection = this._db.GetCollection<T>(collectionName).DeleteMany(filter);
        }

        /// <summary>
        /// Get the last document
        /// </summary>
        /// <param name="collectionName">Collection</param>
        /// <param name="filter">Optional filter</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The last insterted document</returns>
        public T GetLastDoc<T>(string collectionName, FilterDefinition<T> filter = null)
        {
            if (filter == null)
                filter = Builders<T>.Filter.Empty;

            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            var doc = this._db.GetCollection<T>(collectionName)
                    .Find(filter)
                    .SortByDescending(d => d)
                    .Limit(1)
                    .First();
            doc = setLocalTime(doc);
            return doc;
        }

        /// <summary>
        /// Count documents
        /// </summary>
        /// <param name="collectionName">Collection</param>
        /// <param name="filter">Optional filter criteria</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Count of records</returns>
        public long GetCount<T>(string collectionName, FilterDefinition<T> filter = null)
        {
            try
            {
                if (filter == null)
                    filter = Builders<T>.Filter.Empty;

                filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
                return this._db.GetCollection<T>(collectionName).CountDocuments(filter);
            }
            catch
            {
                return 0;
            }

        }
        public long GetDistinctCount<T>(string collectionName, FilterDefinition<T> filter = null)
        {
            if (filter == null)
                filter = Builders<T>.Filter.Empty;

            filter = this.ApplyOperationModeRestrictionsAndBaseFilters(filter);
            return this._db.GetCollection<T>(collectionName).CountDocuments(filter);
        }

        #region Private Methods

        /// <summary>
        /// To set Client and Store Id's from auth service
        /// </summary>
        /// <param name="document"></param>
        /// <typeparam name="T"></typeparam>
        private T setBaseFields<T>(T document)
        {
            // Use when there is a requirement to set a basefield
            return document;
        }

        /// <summary>
        /// To apply operation mode restrictions
        /// </summary>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private FilterDefinition<T> ApplyOperationModeRestrictionsAndBaseFilters<T>(FilterDefinition<T> filter)
        {
            // Removing deleted records
            filter = filter & Builders<T>.Filter.Eq(TNGBaseFields.ISDELETED, false);
            return filter;
        }

        private FilterDefinition<T> ApplyDeleteBaseFilters<T>(FilterDefinition<T> filter)
        {

            // Removing deleted records
            filter = filter & Builders<T>.Filter.Eq(TNGBaseFields.ISDELETED, false);

            return filter;
        }


        private T setLocalTime<T>(T document, Type type = null)
        {
            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                if (properties.Count() <= 0)
                {
                    properties = type != null ? type.GetProperties() : properties;
                }
                foreach (PropertyInfo property in properties)
                {
                    var fieldType = property.PropertyType;
                    var dateTimeType = typeof(DateTime);
                    Type tColl = typeof(ICollection<>);

                    if (fieldType == dateTimeType)
                    {
                        var time = property.GetValue(document);
                        var storeTime = this._utilityLib.convertDateTimeFromUtc(time);
                        property.SetValue(document, storeTime);
                    }
                    else if (fieldType.IsGenericType && fieldType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tColl))
                    {
                        var objects = Activator.CreateInstance(fieldType);
                        objects = property.GetValue(document);
                        if (objects != null)
                        {
                            var objs = objects as IEnumerable;
                            foreach (var obj in objs)
                            {
                                var objType = obj.GetType();
                                if (objType != typeof(string) && objType != typeof(int) && objType != typeof(double) && objType != typeof(bool))
                                {
                                    this.setLocalTime(obj, objType);
                                }
                            }
                            property.SetValue(document, objects);
                        }
                    }
                    else if (fieldType.IsClass && fieldType != typeof(string) && fieldType != typeof(int) && fieldType != typeof(double) && fieldType != typeof(bool))
                    {
                        var obj = Activator.CreateInstance(fieldType);
                        obj = property.GetValue(document);
                        if (obj != null)
                        {
                            this.setLocalTime(obj, fieldType);
                            property.SetValue(document, obj);
                        }

                    }
                }

            }
            catch
            {

            }
            return document;
        }

        private List<T> setDateTimes<T>(List<T> documents)
        {

            foreach (var doc in documents)
            {
                this.setLocalTime(doc);
            }

            return documents;
        }

        private T updateTimeToUtc<T>(T document, Type type = null)
        {
            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                if (properties.Count() <= 0)
                {
                    properties = type != null ? type.GetProperties() : properties;
                }
                foreach (PropertyInfo property in properties)
                {
                    var fieldType = property.PropertyType;
                    var dateTimeType = typeof(DateTime);
                    Type tColl = typeof(ICollection<>);
                    var ws = fieldType.GetInterfaces().Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition());
                    if (fieldType == dateTimeType)
                    {
                        var time = property.GetValue(document);
                        var storeTime = this._utilityLib.ConvertTimeToUtc(time);
                        property.SetValue(document, storeTime);
                    }
                    else if (fieldType.IsGenericType && fieldType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tColl))
                    {
                        var objects = Activator.CreateInstance(fieldType);
                        objects = property.GetValue(document);
                        if (objects != null)
                        {
                            var objs = objects as IEnumerable;
                            foreach (var obj in objs)
                            {
                                var objType = obj.GetType();
                                if (objType != typeof(string) && objType != typeof(int) && objType != typeof(double))
                                {
                                    this.updateTimeToUtc(obj, objType);
                                }
                            }
                            property.SetValue(document, objects);
                        }
                    }
                    else if (fieldType.IsClass && fieldType != typeof(string) && fieldType != typeof(int) && fieldType != typeof(double) && fieldType != typeof(bool))
                    {
                        var obj = Activator.CreateInstance(fieldType);
                        obj = property.GetValue(document);
                        if (obj != null)
                        {
                            this.updateTimeToUtc(obj, fieldType);
                            property.SetValue(document, obj);
                        }

                    }
                }

                return document;
            }
            catch
            {
                return document;
            }
        }
        #endregion
    }
}
