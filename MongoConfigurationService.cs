using TNG.Shared.Lib.Mongo.Base;

namespace TNG.Shared.Lib
{
    public class MongoConfigurationService : IMongoConfigurationService
    {
        private string DataBase { get; set; }
        private MongoOperationsMode OperationsMode { get; set; }

        /// <summary>
        /// Initialize Mongo Configuration Serive
        /// </summary>
        /// <param name="db">Database to connect</param>
        /// <param name="opsMode">Restriction mode</param>
        public MongoConfigurationService(string db, MongoOperationsMode opsMode)
        {
            this.DataBase = db;
            this.OperationsMode = opsMode;
        }

        /// <summary>
        /// Gets Database to connect to
        /// </summary>
        /// <returns></returns>
        public string GetMongoDatabase()
        {
            return this.DataBase;
        }

/// <summary>
/// Get Operations Mode
/// </summary>
/// <returns></returns>
        public MongoOperationsMode GetOperationMode()
        {
            return this.OperationsMode;
        }
    }
}