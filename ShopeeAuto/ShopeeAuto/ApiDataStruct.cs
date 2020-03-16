using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTaobaoProduct
{
    #region Taobao Product
    public partial class TaobaoProduct
    {
        public string Api { get; set; }
        public string V { get; set; }
        public List<string> Ret { get; set; }
        public Data Data { get; set; }
    }

    public partial class Data
    {
        public NSTaobaoProductDetail.TaobaoProductDetails Details { get; set; }

        public List<ApiStack> ApiStack { get; set; }
        public Seller Seller { get; set; }
        public string PropsCut { get; set; }
        public Item Item { get; set; }
        public Debug Debug { get; set; }
        public Resource Resource { get; set; }
        public Vertical Vertical { get; set; }
        public DataParams Params { get; set; }
        public Props Props { get; set; }
        public string MockData { get; set; }
        public Feature Feature { get; set; }
        public Rate Rate { get; set; }
        public Props2 Props2 { get; set; }
        public SkuBase SkuBase { get; set; }

        [JsonProperty("skuCore")]
        public SkuCore SkuCore { get; set; }
    }

    public partial class SkuCore
    {
        [JsonProperty("skuItem")]
        public SkuItem SkuItem { get; set; }

        [JsonProperty("sku2info")]
        public Dictionary<string, Sku2Info> Sku2Info { get; set; }

    }
    public partial class Sku2Info
    {
        [JsonProperty("price")]
        public Sku2InfoPrice Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

    public partial class Sku2InfoPrice
    {
        [JsonProperty("priceMoney")]
        public long PriceMoney { get; set; }

        [JsonProperty("priceText")]
        public string PriceText { get; set; }

        [JsonProperty("showTitle")]
        public bool ShowTitle { get; set; }

        [JsonProperty("sugProm")]
        public bool SugProm { get; set; }
    }

    public partial class SkuItem
    {
        [JsonProperty("showAddress")]
        public bool ShowAddress { get; set; }

        [JsonProperty("hideQuantity")]
        public bool HideQuantity { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }
    public partial class ApiStack
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public partial class Debug
    {
        public string Host { get; set; }
        public string App { get; set; }
    }

    public partial class Feature
    {
        public bool ShowSkuProRate { get; set; }
    }

    public partial class Item
    {
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public List<string> Images { get; set; }
        public long CategoryId { get; set; }
        public long RootCategoryId { get; set; }
        public long BrandValueId { get; set; }
        public string SkuText { get; set; }
        public List<object> CountMultiple { get; set; }
        public Props2 ExParams { get; set; }
        public long CommentCount { get; set; }
        public long Favcount { get; set; }
        public string TaobaoDescUrl { get; set; }
        public string TmallDescUrl { get; set; }
        public string TaobaoPcDescUrl { get; set; }
        public string ModuleDescUrl { get; set; }
        public bool OpenDecoration { get; set; }
        public ModuleDescParams ModuleDescParams { get; set; }
        public string H5ModuleDescUrl { get; set; }
        public Uri CartUrl { get; set; }
    }

    public partial class Props2
    {
    }

    public partial class ModuleDescParams
    {
        public string F { get; set; }
        public string Id { get; set; }
    }

    public partial class DataParams
    {
        public PurpleTrackParams TrackParams { get; set; }
    }

    public partial class PurpleTrackParams
    {
        public long BrandId { get; set; }
        public string BcType { get; set; }
        public long CategoryId { get; set; }
    }

    public partial class Props
    {
        [JsonProperty("groupProps")]
        public List<GroupProp> GroupProps { get; set; }
    }
    public partial class GroupProp
    {
        [JsonProperty("基本信息")]
        public List<Dictionary<string, string>> BasicInfo { get; set; }
    }

    public partial class Rate
    {
        public long TotalCount { get; set; }
        public List<Keyword> Keywords { get; set; }
        public List<RateList> RateList { get; set; }
        public List<PropRate> PropRate { get; set; }
    }

    public partial class Keyword
    {
        public string Attribute { get; set; }
        public string Word { get; set; }
        public long Count { get; set; }
        public long Type { get; set; }
    }

    public partial class PropRate
    {
        public string PropName { get; set; }
        public Uri Avatar { get; set; }
        public string Comment { get; set; }
        public string CommentCount { get; set; }
        public string Image { get; set; }
        public string FeedId { get; set; }
        public long SkuVids { get; set; }
    }

    public partial class RateList
    {
        public string Content { get; set; }
        public string UserName { get; set; }
        public string HeadPic { get; set; }
        public long MemberLevel { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string SkuInfo { get; set; }
        public List<string> Images { get; set; }
        public long TmallMemberLevel { get; set; }
        public bool IsVip { get; set; }
        public string FeedId { get; set; }
    }

    public partial class Resource
    {
        public Entrances Entrances { get; set; }
    }

    public partial class Entrances
    {
        public EntrancesAskAll AskAll { get; set; }
    }

    public partial class EntrancesAskAll
    {
        public Uri Icon { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
    }

    public partial class Seller
    {
        public string UserId { get; set; }
        public long ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopUrl { get; set; }
        public string TaoShopUrl { get; set; }
        public Uri ShopIcon { get; set; }
        public string Fans { get; set; }
        public long AllItemCount { get; set; }
        public long NewItemCount { get; set; }
        public bool ShowShopLinkIcon { get; set; }
        public string ShopCard { get; set; }
        public string SellerType { get; set; }
        public string ShopType { get; set; }
        public List<Evaluate> Evaluates { get; set; }
        public List<Evaluates2> Evaluates2 { get; set; }
        public string SellerNick { get; set; }
        public long CreditLevel { get; set; }
        public string CreditLevelIcon { get; set; }
        public string BrandIcon { get; set; }
        public string BrandIconRatio { get; set; }
        public DateTimeOffset Starts { get; set; }
        public string GoodRatePercentage { get; set; }
        public string Fbt2User { get; set; }
        public long SimpleShopDoStatus { get; set; }
        public long ShopVersion { get; set; }
        public Uri AtmosphereImg { get; set; }
        public string AtmosphereColor { get; set; }
        public string ShopTextColor { get; set; }
        public List<EntranceList> EntranceList { get; set; }
        public bool AtmophereMask { get; set; }
        public string AtmosphereMaskColor { get; set; }
    }

    public partial class EntranceList
    {
        public string Text { get; set; }
        public string TextColor { get; set; }
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }
        public List<Action> Action { get; set; }
    }

    public partial class Action
    {
        public string Key { get; set; }
        public ActionParams Params { get; set; }
    }

    public partial class ActionParams
    {
        public string Url { get; set; }
        public FluffyTrackParams TrackParams { get; set; }
        public string TrackName { get; set; }
    }

    public partial class FluffyTrackParams
    {
        public string Spm { get; set; }
    }

    public partial class Evaluate
    {
        public string Title { get; set; }
        public string Score { get; set; }
        public string Type { get; set; }
        public long Level { get; set; }
        public string LevelText { get; set; }
        public string LevelTextColor { get; set; }
        public string LevelBackgroundColor { get; set; }
        public string TmallLevelTextColor { get; set; }
        public string TmallLevelBackgroundColor { get; set; }
    }

    public partial class Evaluates2
    {
        public string TitleColor { get; set; }
        public string ScoreTextColor { get; set; }
        public string Title { get; set; }
        public string Score { get; set; }
        public string Type { get; set; }
        public long Level { get; set; }
        public string LevelText { get; set; }
        public string LevelTextColor { get; set; }
    }

    public partial class SkuBase
    {
        public List<Skus> Skus { get; set; }
        public List<Prop> Props { get; set; }
    }

    public partial class Prop
    {
        public long Pid { get; set; }
        public string Name { get; set; }
        public List<Value> Values { get; set; }
    }

    public partial class Value
    {
        public long Vid { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public partial class Skus
    {
        public string SkuId { get; set; }
        public string PropPath { get; set; }
    }

    public partial class Vertical
    {
        public VerticalAskAll AskAll { get; set; }
    }

    public partial class VerticalAskAll
    {
        public string AskText { get; set; }
        public Uri AskIcon { get; set; }
        public string LinkUrl { get; set; }
        public string Title { get; set; }
        public long QuestNum { get; set; }
        public long ShowNum { get; set; }
        public List<ModelList> ModelList { get; set; }
        public List<Model4XList> Model4XList { get; set; }
    }

    public partial class Model4XList
    {
        public string AskText { get; set; }
        public string AnswerCountText { get; set; }
        public string AskIcon { get; set; }
        public string AskTextColor { get; set; }
    }

    public partial class ModelList
    {
        public string AskText { get; set; }
        public string AnswerCountText { get; set; }
    }
    #endregion    
}

namespace NSApiProduct
{
    public partial class ProductList
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("shopee_ids")]
        public List<ShopeeId> ShopeeIds { get; set; }

        [JsonProperty("taobao_ids")]
        public List<TaobaoId> TaobaoIds { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("company_id")]
        public string CompanyId { get; set; }

        [JsonProperty("tactic")]
        public Tactic Tactic { get; set; }

        [JsonProperty("shops")]
        public List<Shop> Shops { get; set; }

        [JsonProperty("original_product")]
        public string OriginalProduct { get; set; }

        [JsonProperty("rival_product")]
        public string RivalProduct { get; set; }

        [JsonProperty("selling_product")]
        public string SeliingProduct { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }

    public partial class Tactic
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public partial class ShopeeId
    {
        [JsonProperty("shop_id")]
        public string ShopId { get; set; }

        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        [JsonProperty("is_the_best")]
        public bool IsTheBest { get; set; }
    }

    public partial class Shop
    {
        [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("shopee_item_id")]
        public string ShopeeItemId { get; set; }

        [JsonProperty("shopee_shop_id")]
        public string ShopeeShopId { get; set; }
    }

    public partial class TaobaoId
    {
        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        [JsonProperty("is_the_best")]
        public bool IsTheBest { get; set; }
    }
}

namespace NSTaobaoProductDetail
{
    public partial class TaobaoProductDetails
    {
        [JsonProperty("feature")]
        public Dictionary<string, bool> Feature { get; set; }

        [JsonProperty("trade")]
        public Trade Trade { get; set; }

        [JsonProperty("item")]
        public TaobaoProductDetailsItem Item { get; set; }

        [JsonProperty("buyer")]
        public Buyer Buyer { get; set; }

        [JsonProperty("price")]
        public TaobaoProductDetailsPrice Price { get; set; }

        [JsonProperty("consumerProtection")]
        public ConsumerProtection ConsumerProtection { get; set; }

        [JsonProperty("resource")]
        public Resource Resource { get; set; }

        [JsonProperty("vertical")]
        public Vertical Vertical { get; set; }

        [JsonProperty("delivery")]
        public Delivery Delivery { get; set; }

        [JsonProperty("skuBase")]
        public SkuBase SkuBase { get; set; }

        [JsonProperty("skuCore")]
        public SkuCore SkuCore { get; set; }

        [JsonProperty("promotionFloatingData")]
        public PromotionFloatingData PromotionFloatingData { get; set; }

        [JsonProperty("skuVertical")]
        public Diversion SkuVertical { get; set; }

        [JsonProperty("params")]
        public Params Params { get; set; }

        [JsonProperty("skuTransform")]
        public SkuTransform SkuTransform { get; set; }

        [JsonProperty("diversion")]
        public Diversion Diversion { get; set; }

        [JsonProperty("layout")]
        public Diversion Layout { get; set; }

        [JsonProperty("otherInfo")]
        public OtherInfo OtherInfo { get; set; }

        [JsonProperty("modules")]
        public List<object> Modules { get; set; }
    }

    public partial class Buyer
    {
        [JsonProperty("tmallMemberLevel")]
        public long TmallMemberLevel { get; set; }
    }

    public partial class ConsumerProtection
    {
        [JsonProperty("serviceProtection")]
        public ServiceProtection ServiceProtection { get; set; }

        [JsonProperty("passValue")]
        public string PassValue { get; set; }

        [JsonProperty("strength")]
        public string Strength { get; set; }

        [JsonProperty("items")]
        public List<ItemElement> Items { get; set; }

        [JsonProperty("params")]
        public string Params { get; set; }
    }

    public partial class ItemElement
    {
        [JsonProperty("serviceId")]
        public long ServiceId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }
    }

    public partial class ServiceProtection
    {
        [JsonProperty("basicService")]
        public BasicService BasicService { get; set; }
    }

    public partial class BasicService
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("services")]
        public List<Service> Services { get; set; }
    }

    public partial class Service
    {
        [JsonProperty("serviceId")]
        public long ServiceId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("desc")]
        public List<string> Desc { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }
    }

    public partial class Delivery
    {
        [JsonProperty("extras")]
        public Diversion Extras { get; set; }

        [JsonProperty("postage")]
        public string Postage { get; set; }

        [JsonProperty("completedTo")]
        public string CompletedTo { get; set; }

        [JsonProperty("overseaContraBandFlag")]
        
        public bool OverseaContraBandFlag { get; set; }

        [JsonProperty("areaSell")]
        
        public bool AreaSell { get; set; }

        [JsonProperty("areaId")]
        public long AreaId { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }

    public partial class Diversion
    {
    }

    public partial class TaobaoProductDetailsItem
    {
        [JsonProperty("extraMap")]
        public Diversion ExtraMap { get; set; }

        [JsonProperty("itemPoint")]
        public long ItemPoint { get; set; }

        [JsonProperty("infoText")]
        public Diversion InfoText { get; set; }

        [JsonProperty("sellCount")]
        public long SellCount { get; set; }

        [JsonProperty("vagueSellCount")]
        public long VagueSellCount { get; set; }

        [JsonProperty("skuText")]
        public string SkuText { get; set; }

        [JsonProperty("videos")]
        public List<object> Videos { get; set; }

        [JsonProperty("descType")]
        public long DescType { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("spuId")]
        public long SpuId { get; set; }

        [JsonProperty("itemId")]
        public string ItemId { get; set; }
    }

    public partial class OtherInfo
    {
        [JsonProperty("bucketId")]
        public long BucketId { get; set; }

        [JsonProperty("systemTime")]
        public string SystemTime { get; set; }
    }

    public partial class Params
    {
        [JsonProperty("trackParams")]
        public TrackParams TrackParams { get; set; }

        [JsonProperty("umbParams")]
        public UmbParams UmbParams { get; set; }
    }

    public partial class TrackParams
    {
        [JsonProperty("shop_id")]
        public long ShopId { get; set; }

        [JsonProperty("detailabtestdetail")]
        public string Detailabtestdetail { get; set; }
    }

    public partial class UmbParams
    {
        [JsonProperty("aliBizName")]
        public string AliBizName { get; set; }

        [JsonProperty("aliBizCode")]
        public string AliBizCode { get; set; }
    }

    public partial class TaobaoProductDetailsPrice
    {
        [JsonProperty("newExtraPrices")]
        public List<ExtraPrice> NewExtraPrices { get; set; }

        [JsonProperty("extraPrices")]
        public List<ExtraPrice> ExtraPrices { get; set; }

        [JsonProperty("priceTag")]
        public List<PriceTag> PriceTag { get; set; }

        [JsonProperty("shopProm")]
        public List<ShopProm> ShopProm { get; set; }

        [JsonProperty("transmitPrice")]
        public TransmitPriceClass TransmitPrice { get; set; }

        [JsonProperty("shopPromTitle")]
        public string ShopPromTitle { get; set; }

        [JsonProperty("superMarketShopProm")]
        public List<object> SuperMarketShopProm { get; set; }

        [JsonProperty("price")]
        public TransmitPriceClass Price { get; set; }
    }

    public partial class ExtraPrice
    {
        [JsonProperty("priceText")]
        public string PriceText { get; set; }

        [JsonProperty("priceTitle")]
        public string PriceTitle { get; set; }

        [JsonProperty("showTitle")]
        
        public bool ShowTitle { get; set; }

        [JsonProperty("lineThrough")]
        
        public bool LineThrough { get; set; }

        [JsonProperty("sugProm")]
        
        public bool SugProm { get; set; }
    }

    public partial class TransmitPriceClass
    {
        [JsonProperty("priceText")]
        public string PriceText { get; set; }

        [JsonProperty("sugProm")]
        
        public bool SugProm { get; set; }
    }

    public partial class PriceTag
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("bigmarkdownTag")]
        
        public bool BigmarkdownTag { get; set; }
    }

    public partial class ShopProm
    {
        [JsonProperty("iconText")]
        public string IconText { get; set; }

        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("content")]
        public List<string> Content { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }
    }

    public partial class PromotionFloatingData
    {
        [JsonProperty("showWarm")]
        
        public bool ShowWarm { get; set; }

        [JsonProperty("showNow")]
        
        public bool ShowNow { get; set; }
    }

    public partial class Resource
    {
        [JsonProperty("indexCouponData")]
        public Diversion IndexCouponData { get; set; }

        [JsonProperty("entrances")]
        public Diversion Entrances { get; set; }

        [JsonProperty("entrancesBizsContent")]
        public string EntrancesBizsContent { get; set; }

        [JsonProperty("promsCalcInfo")]
        public PromsCalcInfo PromsCalcInfo { get; set; }

        [JsonProperty("coupon")]
        public Diversion Coupon { get; set; }
    }

    public partial class PromsCalcInfo
    {
        [JsonProperty("cheapestMoney")]
        public long CheapestMoney { get; set; }

        [JsonProperty("hasCoupon")]
        
        public bool HasCoupon { get; set; }

        [JsonProperty("needReqCouDan")]
        
        public bool NeedReqCouDan { get; set; }
    }

    public partial class SkuBase
    {
        [JsonProperty("skus")]
        public List<Skus> Skus { get; set; }

        [JsonProperty("props")]
        public List<Prop> Props { get; set; }
    }

    public partial class Prop
    {
        [JsonProperty("pid")]
        public long Pid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("values")]
        public List<Value> Values { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("vid")]
        public long Vid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }

        [JsonProperty("sortOrder")]
        public long SortOrder { get; set; }
    }

    public partial class Skus
    {
        [JsonProperty("skuId")]
        public string SkuId { get; set; }

        [JsonProperty("propPath")]
        public string PropPath { get; set; }
    }

    public partial class SkuCore
    {
        [JsonProperty("skuItem")]
        public SkuItem SkuItem { get; set; }

        [JsonProperty("sku2info")]
        public Dictionary<string, Sku2Info> Sku2Info { get; set; }
    }

    public partial class Sku2Info
    {
        [JsonProperty("price")]
        public Sku2InfoPrice Price { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }
    }

    public partial class Sku2InfoPrice
    {
        [JsonProperty("priceMoney")]
        public long PriceMoney { get; set; }

        [JsonProperty("priceText")]
        public string PriceText { get; set; }

        [JsonProperty("showTitle")]
        
        public bool ShowTitle { get; set; }

        [JsonProperty("sugProm")]
        
        public bool SugProm { get; set; }
    }

    public partial class SkuItem
    {
        [JsonProperty("showAddress")]
        public bool ShowAddress { get; set; }

        [JsonProperty("hideQuantity")]
        public bool HideQuantity { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
    }

    public partial class SkuTransform
    {
        [JsonProperty("skuContents")]
        public List<SkuContent> SkuContents { get; set; }

        [JsonProperty("extraText")]
        public string ExtraText { get; set; }
    }

    public partial class SkuContent
    {
        [JsonProperty("img")]
        public string Img { get; set; }
    }

    public partial class Trade
    {
        [JsonProperty("cartEnable")]
        public bool CartEnable { get; set; }

        [JsonProperty("cartParam")]
        public Param CartParam { get; set; }

        [JsonProperty("outerCartParam")]
        public Diversion OuterCartParam { get; set; }

        [JsonProperty("cartConfirmEnable")]
        public bool CartConfirmEnable { get; set; }

        [JsonProperty("tradeParams")]
        public Diversion TradeParams { get; set; }

        [JsonProperty("isWap")]
        public bool IsWap { get; set; }

        [JsonProperty("buyEnable")]
        public bool BuyEnable { get; set; }

        [JsonProperty("useWap")]
        public bool UseWap { get; set; }

        [JsonProperty("buyParam")]
        public Param BuyParam { get; set; }

        [JsonProperty("auctionStatus")]
        public long AuctionStatus { get; set; }
    }

    public partial class Param
    {
        [JsonProperty("areaId")]
        public long AreaId { get; set; }
    }

    public partial class Vertical
    {
        [JsonProperty("askAll")]
        public AskAll AskAll { get; set; }

        [JsonProperty("freshFood")]
        public FreshFood FreshFood { get; set; }

        [JsonProperty("tmallLeaseData")]
        public TmallLeaseData TmallLeaseData { get; set; }

        [JsonProperty("degrade")]
        public string Degrade { get; set; }
    }

    public partial class AskAll
    {
        [JsonProperty("askIcon")]
        public Uri AskIcon { get; set; }

        [JsonProperty("askText")]
        public string AskText { get; set; }

        [JsonProperty("questNum")]
        public long QuestNum { get; set; }

        [JsonProperty("linkUrl")]
        public string LinkUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class FreshFood
    {
        [JsonProperty("nationalFlag")]
        public string NationalFlag { get; set; }
    }

    public partial class TmallLeaseData
    {
        [JsonProperty("rentOfficialPriceFen")]
        public long RentOfficialPriceFen { get; set; }

        [JsonProperty("rentGuaranteePriceFen")]
        public long RentGuaranteePriceFen { get; set; }

        [JsonProperty("zhiMaSigned")]
        public bool ZhiMaSigned { get; set; }
    }
}

namespace NSClientInfo
{
    public partial class ClientInfo
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("shopee_username")]
        public string ShopeeUsername { get; set; }

        [JsonProperty("shopee_password")]
        public string ShopeePassword { get; set; }

        [JsonProperty("shopee_url")]
        public Uri ShopeeUrl { get; set; }

        [JsonProperty("shopee_min_revenue")]
        public int ShopeeMinRevenue { get; set; }

        [JsonProperty("shopee_max_revenue")]
        public int ShopeeMaxRevenue { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("company_id")]
        public string CompanyId { get; set; }

        [JsonProperty("categories")]
        public dynamic Categories { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("client_name")]
        public string ClientName { get; set; }
    }
}

namespace NSShopeeProduct
{
    public partial class ShopeeProduct
    {
        [JsonProperty("item")]
        public Item Item { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("error_msg")]
        public object ErrorMsg { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("itemid")]
        public long Itemid { get; set; }

        [JsonProperty("price_max_before_discount")]
        public long PriceMaxBeforeDiscount { get; set; }

        [JsonProperty("item_status")]
        public string ItemStatus { get; set; }

        [JsonProperty("can_use_wholesale")]
        public bool CanUseWholesale { get; set; }

        [JsonProperty("show_free_shipping")]
        public bool ShowFreeShipping { get; set; }

        [JsonProperty("estimated_days")]
        public long EstimatedDays { get; set; }

        [JsonProperty("is_hot_sales")]
        public object IsHotSales { get; set; }

        [JsonProperty("is_slash_price_item")]
        public bool IsSlashPriceItem { get; set; }

        [JsonProperty("upcoming_flash_sale")]
        public object UpcomingFlashSale { get; set; }

        [JsonProperty("slash_lowest_price")]
        public object SlashLowestPrice { get; set; }

        [JsonProperty("condition")]
        public long Condition { get; set; }

        [JsonProperty("add_on_deal_info")]
        public object AddOnDealInfo { get; set; }

        [JsonProperty("is_non_cc_installment_payment_eligible")]
        public bool IsNonCcInstallmentPaymentEligible { get; set; }

        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }

        [JsonProperty("ctime")]
        public long Ctime { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("show_shopee_verified_label")]
        public bool ShowShopeeVerifiedLabel { get; set; }

        [JsonProperty("size_chart")]
        public object SizeChart { get; set; }

        [JsonProperty("is_pre_order")]
        public bool IsPreOrder { get; set; }

        [JsonProperty("service_by_shopee_flag")]
        public long ServiceByShopeeFlag { get; set; }

        [JsonProperty("historical_sold")]
        public long HistoricalSold { get; set; }

        [JsonProperty("reference_item_id")]
        public string ReferenceItemId { get; set; }

        [JsonProperty("recommendation_info")]
        public object RecommendationInfo { get; set; }

        [JsonProperty("bundle_deal_info")]
        public object BundleDealInfo { get; set; }

        [JsonProperty("price_max")]
        public long PriceMax { get; set; }

        [JsonProperty("has_lowest_price_guarantee")]
        public bool HasLowestPriceGuarantee { get; set; }

        [JsonProperty("shipping_icon_type")]
        public long ShippingIconType { get; set; }

        [JsonProperty("images")]
        public List<string> Images { get; set; }

        [JsonProperty("price_before_discount")]
        public long PriceBeforeDiscount { get; set; }

        [JsonProperty("cod_flag")]
        public long CodFlag { get; set; }

        [JsonProperty("catid")]
        public long Catid { get; set; }

        [JsonProperty("is_official_shop")]
        public bool IsOfficialShop { get; set; }

        [JsonProperty("coin_earn_label")]
        public object CoinEarnLabel { get; set; }

        [JsonProperty("hashtag_list")]
        public List<object> HashtagList { get; set; }

        [JsonProperty("sold")]
        public long Sold { get; set; }

        [JsonProperty("makeup")]
        public object Makeup { get; set; }

        [JsonProperty("item_rating")]
        public ItemRating ItemRating { get; set; }

        [JsonProperty("show_official_shop_label_in_title")]
        public bool ShowOfficialShopLabelInTitle { get; set; }

        [JsonProperty("discount")]
        public object Discount { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("label_ids")]
        public List<long> LabelIds { get; set; }

        [JsonProperty("has_group_buy_stock")]
        public bool HasGroupBuyStock { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; }

        [JsonProperty("badge_icon_type")]
        public long BadgeIconType { get; set; }

        [JsonProperty("liked")]
        public bool Liked { get; set; }

        [JsonProperty("cmt_count")]
        public long CmtCount { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("is_cc_installment_payment_eligible")]
        public bool IsCcInstallmentPaymentEligible { get; set; }

        [JsonProperty("shopid")]
        public long Shopid { get; set; }

        [JsonProperty("normal_stock")]
        public long NormalStock { get; set; }

        [JsonProperty("video_info_list")]
        public List<object> VideoInfoList { get; set; }

        [JsonProperty("installment_plans")]
        public object InstallmentPlans { get; set; }

        [JsonProperty("view_count")]
        public object ViewCount { get; set; }

        [JsonProperty("current_promotion_has_reserve_stock")]
        public bool CurrentPromotionHasReserveStock { get; set; }

        [JsonProperty("liked_count")]
        public long LikedCount { get; set; }

        [JsonProperty("show_official_shop_label")]
        public bool ShowOfficialShopLabel { get; set; }

        [JsonProperty("price_min_before_discount")]
        public long PriceMinBeforeDiscount { get; set; }

        [JsonProperty("show_discount")]
        public long ShowDiscount { get; set; }

        [JsonProperty("preview_info")]
        public object PreviewInfo { get; set; }

        [JsonProperty("flag")]
        public long Flag { get; set; }

        [JsonProperty("current_promotion_reserved_stock")]
        public long CurrentPromotionReservedStock { get; set; }

        [JsonProperty("wholesale_tier_list")]
        public List<object> WholesaleTierList { get; set; }

        [JsonProperty("group_buy_info")]
        public object GroupBuyInfo { get; set; }

        [JsonProperty("shopee_verified")]
        public bool ShopeeVerified { get; set; }

        [JsonProperty("hidden_price_display")]
        public object HiddenPriceDisplay { get; set; }

        [JsonProperty("transparent_background_image")]
        public string TransparentBackgroundImage { get; set; }

        [JsonProperty("welcome_package_info")]
        public object WelcomePackageInfo { get; set; }

        [JsonProperty("coin_info")]
        public CoinInfo CoinInfo { get; set; }

        [JsonProperty("is_adult")]
        public object IsAdult { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("raw_discount")]
        public long RawDiscount { get; set; }

        [JsonProperty("is_category_failed")]
        public bool IsCategoryFailed { get; set; }

        [JsonProperty("price_min")]
        public long PriceMin { get; set; }

        [JsonProperty("can_use_bundle_deal")]
        public bool CanUseBundleDeal { get; set; }

        [JsonProperty("cb_option")]
        public long CbOption { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("stock")]
        public long Stock { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("bundle_deal_id")]
        public long BundleDealId { get; set; }

        [JsonProperty("is_group_buy_item")]
        public object IsGroupBuyItem { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("flash_sale")]
        public object FlashSale { get; set; }

        [JsonProperty("models")]
        public List<Model> Models { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("shop_location")]
        public string ShopLocation { get; set; }

        [JsonProperty("tier_variations")]
        public List<object> TierVariations { get; set; }

        [JsonProperty("makeups")]
        public object Makeups { get; set; }

        [JsonProperty("welcome_package_type")]
        public long WelcomePackageType { get; set; }

        [JsonProperty("show_official_shop_label_in_normal_position")]
        public object ShowOfficialShopLabelInNormalPosition { get; set; }

        [JsonProperty("item_type")]
        public long ItemType { get; set; }
    }

    public partial class Model
    {
        public long Itemid { get; set; }
        public long Status { get; set; }
        public long CurrentPromotionReservedStock { get; set; }
        public string Name { get; set; }
        public long Promotionid { get; set; }
        public long Price { get; set; }
        public bool CurrentPromotionHasReserveStock { get; set; }
        public string Currency { get; set; }
        public long NormalStock { get; set; }
        public Extinfo Extinfo { get; set; }
        public long PriceBeforeDiscount { get; set; }
        public long Modelid { get; set; }
        public long Sold { get; set; }
        public long Stock { get; set; }
    }
    public partial class Extinfo
    {
        public object SellerPromotionLimit { get; set; }
        public object HasShopeePromo { get; set; }
        public object GroupBuyInfo { get; set; }
        public object HolidayModeOldStock { get; set; }
        public List<long> TierIndex { get; set; }
        public long SellerPromotionRefreshTime { get; set; }
    }
    public partial class Attribute
    {
        [JsonProperty("is_pending_qc")]
        public bool IsPendingQc { get; set; }

        [JsonProperty("idx")]
        public long Idx { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_timestamp")]
        public bool IsTimestamp { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Category
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("catid")]
        public long Catid { get; set; }

        [JsonProperty("image")]
        public object Image { get; set; }

        [JsonProperty("no_sub")]
        public bool NoSub { get; set; }

        [JsonProperty("is_default_subcat")]
        public bool IsDefaultSubcat { get; set; }

        [JsonProperty("block_buyer_platform")]
        public object BlockBuyerPlatform { get; set; }
    }

    public partial class CoinInfo
    {
        [JsonProperty("spend_cash_unit")]
        public long SpendCashUnit { get; set; }

        [JsonProperty("coin_earn_items")]
        public List<object> CoinEarnItems { get; set; }
    }

    public partial class ItemRating
    {
        [JsonProperty("rating_star")]
        public long RatingStar { get; set; }

        [JsonProperty("rating_count")]
        public List<long> RatingCount { get; set; }

        [JsonProperty("rcount_with_image")]
        public long RcountWithImage { get; set; }

        [JsonProperty("rcount_with_context")]
        public long RcountWithContext { get; set; }
    }
}

namespace NSShopeeCreateProduct {
    public partial class CreateProduct
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("images")]
        public List<string> Images { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("model_list")]
        public List<ModelList> ModelList { get; set; }

        [JsonProperty("category_path")]
        public List<int> CategoryPath { get; set; }

        [JsonProperty("attribute_model")]
        public AttributeModel AttributeModel { get; set; }

        [JsonProperty("category_recommend")]
        public List<object> CategoryRecommend { get; set; }

        [JsonProperty("stock")]
        public long Stock { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("price_before_discount")]
        public string PriceBeforeDiscount { get; set; }

        [JsonProperty("parent_sku")]
        public string ParentSku { get; set; }

        [JsonProperty("wholesale_list")]
        public List<string> WholesaleList { get; set; }

        [JsonProperty("installment_tenures")]
        public InstallmentTenures InstallmentTenures { get; set; }

        [JsonProperty("weight")]
        public string Weight { get; set; }

        [JsonProperty("dimension")]
        public Dimension Dimension { get; set; }

        [JsonProperty("pre_order")]
        public bool PreOrder { get; set; }

        [JsonProperty("days_to_ship")]
        public long DaysToShip { get; set; }

        [JsonProperty("condition")]
        public long Condition { get; set; }

        [JsonProperty("size_chart")]
        public string SizeChart { get; set; }

        [JsonProperty("tier_variation")]
        public List<TierVariation> TierVariation { get; set; }

        [JsonProperty("logistics_channels")]
        public List<LogisticsChannel> LogisticsChannels { get; set; }

        [JsonProperty("unlisted")]
        public bool Unlisted { get; set; }

        [JsonProperty("add_on_deal")]
        public List<object> AddOnDeal { get; set; }

        [JsonProperty("ds_cat_rcmd_id")]
        public string DsCatRcmdId { get; set; }
    }

    public partial class AttributeModel
    {
        [JsonProperty("attribute_model_id")]
        public long AttributeModelId { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; }
    }

    public partial class Attribute
    {
        [JsonProperty("attribute_id")]
        public long AttributeId { get; set; }

        [JsonProperty("prefill")]
        public bool Prefill { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Dimension
    {
        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("length")]
        public long Length { get; set; }
    }

    public partial class InstallmentTenures
    {
    }

    public partial class LogisticsChannel
    {
        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("cover_shipping_fee")]
        public bool CoverShippingFee { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("channelid")]
        public long Channelid { get; set; }

        [JsonProperty("sizeid")]
        public long Sizeid { get; set; }
    }

    public partial class ModelList
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("stock")]
        public long Stock { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("tier_index")]
        public List<int> TierIndex { get; set; }
    }

    public partial class TierVariation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("options")]
        public List<string> Options { get; set; }

        [JsonProperty("images")]
        public List<string> Images { get; set; }
    }
}

namespace NSShopeeOrders
{
    public partial class ShopeeOrders
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("user_message")]
        public string UserMessage { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("orders")]
        public List<Order> Orders { get; set; }
    }

    public partial class Meta
    {
        [JsonProperty("from_seller_data")]
        public bool FromSellerData { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }
    }

    public partial class Order
    {
        public string MaVanDon { get; set; } // Cái này ko có trỏng JSON trả về, tự add tay vào thoy

        [JsonProperty("comm_fee")]
        public string CommFee { get; set; }

        [JsonProperty("shipping_method")]
        public long ShippingMethod { get; set; }

        [JsonProperty("payment_method")]
        public long PaymentMethod { get; set; }

        [JsonProperty("wallet_discount")]
        public string WalletDiscount { get; set; }

        [JsonProperty("shop_id")]
        public long ShopId { get; set; }

        [JsonProperty("add_on_deal_id")]
        public long AddOnDealId { get; set; }

        [JsonProperty("buyer_address_name")]
        public string BuyerAddressName { get; set; }

        [JsonProperty("complete_time")]
        public long CompleteTime { get; set; }

        [JsonProperty("actual_shipping_fee")]
        public string ActualShippingFee { get; set; }

        [JsonProperty("ship_by_date")]
        public long ShipByDate { get; set; }

        [JsonProperty("cancel_time")]
        public long CancelTime { get; set; }

        [JsonProperty("return_id")]
        public long ReturnId { get; set; }

        [JsonProperty("checkout_id")]
        public string CheckoutId { get; set; }

        [JsonProperty("voucher_code")]
        public string VoucherCode { get; set; }

        [JsonProperty("rate_comment")]
        public string RateComment { get; set; }

        [JsonProperty("total_price")]
        public string TotalPrice { get; set; }

        [JsonProperty("tax_amount")]
        public string TaxAmount { get; set; }

        [JsonProperty("first_item_is_wholesale")]
        public bool FirstItemIsWholesale { get; set; }

        [JsonProperty("is_buyercancel_toship")]
        public bool IsBuyercancelToship { get; set; }

        [JsonProperty("list_type")]
        public long ListType { get; set; }

        [JsonProperty("first_item_count")]
        public long FirstItemCount { get; set; }

        [JsonProperty("shipping_confirm_time")]
        public long ShippingConfirmTime { get; set; }

        [JsonProperty("payby_date")]
        public long PaybyDate { get; set; }

        [JsonProperty("seller_service_fee")]
        public string SellerServiceFee { get; set; }

        [JsonProperty("status_ext")]
        public long StatusExt { get; set; }

        [JsonProperty("first_item_name")]
        public string FirstItemName { get; set; }

        [JsonProperty("first_item_return")]
        public bool FirstItemReturn { get; set; }

        [JsonProperty("logistics_status")]
        public int LogisticsStatus { get; set; }

        [JsonProperty("create_time")]
        public long CreateTime { get; set; }

        [JsonProperty("auto_cancel_3pl_ack_date")]
        public long AutoCancel3PlAckDate { get; set; }

        [JsonProperty("price_before_discount")]
        public string PriceBeforeDiscount { get; set; }

        [JsonProperty("seller_due_date")]
        public long SellerDueDate { get; set; }

        [JsonProperty("coins_cash_by_voucher")]
        public string CoinsCashByVoucher { get; set; }

        [JsonProperty("item_count")]
        public long ItemCount { get; set; }

        [JsonProperty("shipment_config")]
        public bool ShipmentConfig { get; set; }

        [JsonProperty("logistics_channel")]
        public long LogisticsChannel { get; set; }

        [JsonProperty("coin_used")]
        public long CoinUsed { get; set; }

        [JsonProperty("origin_shipping_fee")]
        public string OriginShippingFee { get; set; }

        [JsonProperty("actual_price")]
        public string ActualPrice { get; set; }

        [JsonProperty("order_id")]
        public long OrderId { get; set; }

        [JsonProperty("pickup_time")]
        public long PickupTime { get; set; }

        [JsonProperty("voucher_price")]
        public string VoucherPrice { get; set; }

        [JsonProperty("trans_detail_shipping_fee")]
        public string TransDetailShippingFee { get; set; }

        [JsonProperty("buyer_is_rated")]
        public long BuyerIsRated { get; set; }

        [JsonProperty("express_channel")]
        public long ExpressChannel { get; set; }

        [JsonProperty("logistics_extra_data")]
        public string LogisticsExtraData { get; set; }

        [JsonProperty("dropshipping_info")]
        public DropshippingInfo DropshippingInfo { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("buyer_txn_fee")]
        public string BuyerTxnFee { get; set; }

        [JsonProperty("cancel_reason_ext")]
        public long CancelReasonExt { get; set; }

        [JsonProperty("coin_offset")]
        public string CoinOffset { get; set; }

        [JsonProperty("shipping_proof_status")]
        public long ShippingProofStatus { get; set; }

        [JsonProperty("buyer_address_phone")]
        public string BuyerAddressPhone { get; set; }

        [JsonProperty("cancellation_end_date")]
        public object CancellationEndDate { get; set; }

        [JsonProperty("buyer_paid_amount")]
        public string BuyerPaidAmount { get; set; }

        [JsonProperty("ratecancel_by_date")]
        public long RatecancelByDate { get; set; }

        [JsonProperty("first_item_model")]
        public string FirstItemModel { get; set; }

        [JsonProperty("buyer_user")]
        public BuyerUser BuyerUser { get; set; }

        [JsonProperty("estimated_shipping_rebate")]
        public string EstimatedShippingRebate { get; set; }

        [JsonProperty("instant_buyercancel_toship")]
        public bool InstantBuyercancelToship { get; set; }

        [JsonProperty("auto_cancel_arrange_ship_date")]
        public long AutoCancelArrangeShipDate { get; set; }

        [JsonProperty("is_request_cancellation")]
        public bool IsRequestCancellation { get; set; }

        [JsonProperty("order_sn")]
        public string OrderSn { get; set; }

        [JsonProperty("cancel_by")]
        public string CancelBy { get; set; }

        [JsonProperty("note_mtime")]
        public long NoteMtime { get; set; }

        [JsonProperty("delivery_time")]
        public long DeliveryTime { get; set; }

        [JsonProperty("pickup_attempts")]
        public long PickupAttempts { get; set; }

        [JsonProperty("seller_userid")]
        public long SellerUserid { get; set; }

        [JsonProperty("logistics_flag")]
        public long LogisticsFlag { get; set; }

        [JsonProperty("voucher_absorbed_by_seller")]
        public bool VoucherAbsorbedBySeller { get; set; }

        [JsonProperty("shipping_address")]
        public string ShippingAddress { get; set; }

        [JsonProperty("shipping_fee")]
        public string ShippingFee { get; set; }

        [JsonProperty("rate_star")]
        public long RateStar { get; set; }

        [JsonProperty("shipping_traceno")]
        public string ShippingTraceno { get; set; }

        [JsonProperty("pickup_cutoff_time")]
        public long PickupCutoffTime { get; set; }

        [JsonProperty("buyer_last_change_address_time")]
        public long BuyerLastChangeAddressTime { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("escrow_release_time")]
        public long EscrowReleaseTime { get; set; }

        [JsonProperty("shipping_proof")]
        public string ShippingProof { get; set; }

        [JsonProperty("logid")]
        public long Logid { get; set; }

        [JsonProperty("order_type")]
        public long OrderType { get; set; }

        [JsonProperty("paid_amount")]
        public string PaidAmount { get; set; }

        [JsonProperty("credit_card_promotion_discount")]
        public string CreditCardPromotionDiscount { get; set; }

        [JsonProperty("carrier_shipping_fee")]
        public long CarrierShippingFee { get; set; }

        [JsonProperty("rate_by_date")]
        public long RateByDate { get; set; }

        [JsonProperty("credit_card_number")]
        public string CreditCardNumber { get; set; }

        [JsonProperty("order_ratable")]
        public bool OrderRatable { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("shipping_fee_discount")]
        public long ShippingFeeDiscount { get; set; }

        [JsonProperty("card_txn_fee_info")]
        public CardTxnFeeInfo CardTxnFeeInfo { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("pay_by_credit_card")]
        public bool PayByCreditCard { get; set; }

        [JsonProperty("buyer_cancel_reason")]
        public long BuyerCancelReason { get; set; }

        [JsonProperty("order_items")]
        public List<OrderItem> OrderItems { get; set; }

        [JsonProperty("actual_carrier")]
        public string ActualCarrier { get; set; }

        [JsonProperty("shipping_rebate")]
        public string ShippingRebate { get; set; }

        [JsonProperty("used_voucher")]
        public long UsedVoucher { get; set; }

        [JsonProperty("seller_address_id")]
        public long SellerAddressId { get; set; }

        [JsonProperty("cancel_userid")]
        public long CancelUserid { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("seller_address")]
        public SellerAddress SellerAddress { get; set; }

        [JsonProperty("coins_by_voucher")]
        public long CoinsByVoucher { get; set; }

        [JsonProperty("arrange_pickup_by_date")]
        public long ArrangePickupByDate { get; set; }

        [JsonProperty("pay_by_wallet")]
        public bool PayByWallet { get; set; }
    }

    public partial class BuyerUser
    {
        [JsonProperty("phone_public")]
        public bool PhonePublic { get; set; }

        [JsonProperty("buyer_rating")]
        public long BuyerRating { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("hide_likes")]
        public long HideLikes { get; set; }

        [JsonProperty("rating_star")]
        public long RatingStar { get; set; }

        [JsonProperty("cb_option")]
        public long CbOption { get; set; }

        [JsonProperty("followed")]
        public bool Followed { get; set; }

        [JsonProperty("ext_info")]
        public BuyerUserExtInfo ExtInfo { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("rating_count")]
        public List<long> RatingCount { get; set; }

        [JsonProperty("delivery_order_count")]
        public long DeliveryOrderCount { get; set; }

        [JsonProperty("shop_id")]
        public long ShopId { get; set; }

        [JsonProperty("portrait")]
        public string Portrait { get; set; }

        [JsonProperty("delivery_succ_count")]
        public long DeliverySuccCount { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public partial class BuyerUserExtInfo
    {
        [JsonProperty("disable_new_device_login_otp")]
        public bool DisableNewDeviceLoginOtp { get; set; }

        [JsonProperty("delivery_address_id")]
        public long DeliveryAddressId { get; set; }

        [JsonProperty("address_info")]
        public AddressInfo AddressInfo { get; set; }

        [JsonProperty("holiday_mode_on")]
        public bool HolidayModeOn { get; set; }

        [JsonProperty("smid_status")]
        public long SmidStatus { get; set; }

        [JsonProperty("gender")]
        public long Gender { get; set; }

        [JsonProperty("tos_accepted_time")]
        public long TosAcceptedTime { get; set; }

        [JsonProperty("feed_private")]
        public bool FeedPrivate { get; set; }

        [JsonProperty("wallet_password")]
        public string WalletPassword { get; set; }

        [JsonProperty("access")]
        public Access Access { get; set; }

        [JsonProperty("birth_timestamp")]
        public long BirthTimestamp { get; set; }

        [JsonProperty("payment_password")]
        public string PaymentPassword { get; set; }

        [JsonProperty("ba_check_status")]
        public long BaCheckStatus { get; set; }
    }

    public partial class Access
    {
        [JsonProperty("wallet_setting")]
        public long WalletSetting { get; set; }

        [JsonProperty("seller_coin_setting")]
        public long SellerCoinSetting { get; set; }

        [JsonProperty("seller_ads_setting")]
        public long SellerAdsSetting { get; set; }

        [JsonProperty("seller_wholesale_setting")]
        public long SellerWholesaleSetting { get; set; }

        [JsonProperty("hide_likes")]
        public long HideLikes { get; set; }
    }

    public partial class AddressInfo
    {
        [JsonProperty("in_white_list")]
        public bool InWhiteList { get; set; }

        [JsonProperty("mcount")]
        public long Mcount { get; set; }

        [JsonProperty("mcount_create_time")]
        public long McountCreateTime { get; set; }
    }

    public partial class CardTxnFeeInfo
    {
        [JsonProperty("card_txn_fee")]
        public string CardTxnFee { get; set; }

        [JsonProperty("rule_id")]
        public long RuleId { get; set; }
    }

    public partial class DropshippingInfo
    {
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("enabled")]
        public long Enabled { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class OrderItem
    {
        [JsonProperty("is_add_on_sub_item")]
        public bool IsAddOnSubItem { get; set; }

        [JsonProperty("item_price")]
        public string ItemPrice { get; set; }

        [JsonProperty("bundle_deal_product")]
        public List<object> BundleDealProduct { get; set; }

        [JsonProperty("comm_fee_rate")]
        public string CommFeeRate { get; set; }

        [JsonProperty("shop_id")]
        public long ShopId { get; set; }

        [JsonProperty("snapshot_id")]
        public long SnapshotId { get; set; }

        [JsonProperty("add_on_deal_id")]
        public long AddOnDealId { get; set; }

        [JsonProperty("model_id")]
        public long ModelId { get; set; }

        [JsonProperty("item_list")]
        public List<object> ItemList { get; set; }

        [JsonProperty("is_wholesale")]
        public bool IsWholesale { get; set; }

        [JsonProperty("item_model")]
        public ItemModel ItemModel { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("bundle_deal_id")]
        public long BundleDealId { get; set; }

        [JsonProperty("product")]
        public Product Product { get; set; }

        [JsonProperty("item_id")]
        public long ItemId { get; set; }

        [JsonProperty("bundle_deal_model")]
        public List<object> BundleDealModel { get; set; }

        [JsonProperty("order_item_id")]
        public long OrderItemId { get; set; }

        [JsonProperty("price_before_bundle")]
        public string PriceBeforeBundle { get; set; }

        [JsonProperty("bundle_deal")]
        public object BundleDeal { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("order_price")]
        public string OrderPrice { get; set; }

        [JsonProperty("group_id")]
        public long GroupId { get; set; }
    }

    public partial class ItemModel
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("model_id")]
        public long ModelId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("ctime")]
        public long Ctime { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("mtime")]
        public long Mtime { get; set; }

        [JsonProperty("item_id")]
        public long ItemId { get; set; }

        [JsonProperty("promotion_id")]
        public long PromotionId { get; set; }

        [JsonProperty("price_before_discount")]
        public string PriceBeforeDiscount { get; set; }

        [JsonProperty("rebate_price")]
        public string RebatePrice { get; set; }

        [JsonProperty("sold")]
        public long Sold { get; set; }

        [JsonProperty("stock")]
        public long Stock { get; set; }
    }

    public partial class Product
    {
        [JsonProperty("cmt_count")]
        public long CmtCount { get; set; }

        [JsonProperty("cat_id")]
        public long CatId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("shop_id")]
        public long ShopId { get; set; }

        [JsonProperty("snapshot_id")]
        public long SnapshotId { get; set; }

        [JsonProperty("images")]
        public List<string> Images { get; set; }

        [JsonProperty("price_before_discount")]
        public string PriceBeforeDiscount { get; set; }

        [JsonProperty("estimated_days")]
        public long EstimatedDays { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("liked_count")]
        public long LikedCount { get; set; }

        [JsonProperty("branch")]
        public string Branch { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("stock")]
        public long Stock { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("sold")]
        public long Sold { get; set; }

        [JsonProperty("item_id")]
        public long ItemId { get; set; }

        [JsonProperty("condition")]
        public long Condition { get; set; }

        [JsonProperty("ctime")]
        public long Ctime { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("is_pre_order")]
        public bool IsPreOrder { get; set; }
    }

    public partial class SellerAddress
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("address_id")]
        public long AddressId { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("town")]
        public string Town { get; set; }

        [JsonProperty("mtime")]
        public long Mtime { get; set; }

        [JsonProperty("logistics_status")]
        public long LogisticsStatus { get; set; }

        [JsonProperty("full_address")]
        public string FullAddress { get; set; }

        [JsonProperty("ext_info")]
        public SellerAddressExtInfo ExtInfo { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("def_time")]
        public long DefTime { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("icno")]
        public string Icno { get; set; }

        [JsonProperty("zip_code")]
        public string ZipCode { get; set; }

        [JsonProperty("ctime")]
        public long Ctime { get; set; }
    }

    public partial class SellerAddressExtInfo
    {
        [JsonProperty("geo_info")]
        public string GeoInfo { get; set; }
    }
}