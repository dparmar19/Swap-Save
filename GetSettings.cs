using TNG.Shared.Lib.Intefaces;
using System.Data;
using TNG.Shared.Lib.Mongo.Business;
using MongoDB.Driver;
using TNG.Shared.Lib.Mongo.BarterShop.Models;
using TNG.Shared.Lib.Mongo.BarterShop.Master;


namespace TNG.Shared.Lib
{

    public class GetSettings
    {
      

        /// <summary>
        /// DB Context 
        /// </summary>
        /// <value></value>
        private IMongoLayer _db { get; set; }


        public GetSettings(IMongoLayer db)
        {
    
            this._db = db;
        }




    }
}