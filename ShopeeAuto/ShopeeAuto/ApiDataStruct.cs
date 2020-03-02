using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopeeAuto
{
    public struct TaobaoId
    {
        public string itemId;
        public float accuracy;
        public float profit;
        public TaobaoId(string vitemID, float vimg_percent, float vprofit)
        {
            itemId = vitemID;
            accuracy = vimg_percent;
            profit = vprofit;
        }
    }

    public struct ShopeeId
    {
        public string shopId;
        public string itemId;
        public float accuracy;
        public float profit;

        public ShopeeId(string vShopId, string vItemId, float vAccuracy, float vProfit)
        {
            shopId = vShopId;
            itemId = vItemId;
            accuracy = vAccuracy;
            profit = vProfit;

        }
    }

    public class DSH_hunter_result
    {
        #region Fields
        public List<TaobaoId> taobao_list = new List<TaobaoId> { };
        public List<ShopeeId> shopee_list = new List<ShopeeId> { };
        #endregion

        #region Instructions

        #endregion

        #region Methods
        #endregion
    }
}
