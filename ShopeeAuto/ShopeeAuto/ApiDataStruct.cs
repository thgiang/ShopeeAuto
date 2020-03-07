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
        public List<Dictionary<string, List<Dictionary<string, string>>>> GroupProps { get; set; }
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
        public string Id { get; set; }
        public List<ShopeeId> ShopeeIds { get; set; }
        public List<TaobaoId> TaobaoIds { get; set; }
        public string Status { get; set; }
        public string CompanyId { get; set; }
        public long Tactic { get; set; }
        public List<Shop> Shops { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public partial class ShopeeId
    {
        public long ShopId { get; set; }
        public string ItemId { get; set; }
        public bool IsTheBest { get; set; }
    }

    public partial class Shop
    {
        public bool IsPrimary { get; set; }
        public string ClientId { get; set; }
        public long Price { get; set; }
        public string ShopeeItemId { get; set; }
        public string ShopeeShopId { get; set; }
    }

    public partial class TaobaoId
    {
        public string ItemId { get; set; }
        public bool IsTheBest { get; set; }
    }
}
/*
namespace NSTaobaoProductDetail
{
    public partial class TaobaoProductDetails
    {
        public Dictionary<string, bool> Feature { get; set; }
        public Trade Trade { get; set; }
        public TaobaoProductDetailsItem Item { get; set; }
        public Buyer Buyer { get; set; }
        public PriceClass Price { get; set; }
        public ConsumerProtection ConsumerProtection { get; set; }
        public Resource Resource { get; set; }
        public Vertical Vertical { get; set; }
        public Delivery Delivery { get; set; }
        public SkuBase SkuBase { get; set; }
        public SkuCore SkuCore { get; set; }
        public PromotionFloatingData PromotionFloatingData { get; set; }
        public SkuVertical SkuVertical { get; set; }
        public Params Params { get; set; }
        public Diversion Diversion { get; set; }
        public OtherInfo OtherInfo { get; set; }
        public Diversion Layout { get; set; }
        public List<object> Modules { get; set; }
    }

    public partial class Buyer
    {
        public long TmallMemberLevel { get; set; }
    }

    public partial class ConsumerProtection
    {
        public ServiceProtection ServiceProtection { get; set; }
        public List<ItemElement> Items { get; set; }
        public string Params { get; set; }
        public string PassValue { get; set; }
        public string Strength { get; set; }
    }

    public partial class ItemElement
    {
        public long ServiceId { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public long Type { get; set; }
        public long Priority { get; set; }
    }

    public partial class ServiceProtection
    {
        public BasicService BasicService { get; set; }
    }

    public partial class BasicService
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Service> Services { get; set; }
    }

    public partial class Service
    {
        public long ServiceId { get; set; }
        public string Name { get; set; }
        public List<string> Desc { get; set; }
        public string Icon { get; set; }
        public long Priority { get; set; }
        public string Link { get; set; }
    }

    public partial class Delivery
    {
        public string Postage { get; set; }
        public Diversion Extras { get; set; }
        public string CompletedTo { get; set; }
        public bool OverseaContraBandFlag { get; set; }
        public string From { get; set; }
        public bool AreaSell { get; set; }
        public long AreaId { get; set; }
        public string To { get; set; }
    }

    public partial class Diversion
    {
    }

    public partial class TaobaoProductDetailsItem
    {
        public Diversion ExtraMap { get; set; }
        public List<object> Videos { get; set; }
        public string SkuText { get; set; }
        public long SellCount { get; set; }
        public long VagueSellCount { get; set; }
        public long DescType { get; set; }
        public Diversion InfoText { get; set; }
        public string Title { get; set; }
        public long SpuId { get; set; }
        public string ItemId { get; set; }
        public long ItemPoint { get; set; }
    }

    public partial class OtherInfo
    {
        public long BucketId { get; set; }
        public string SystemTime { get; set; }
    }

    public partial class Params
    {
        public TrackParams TrackParams { get; set; }
        public UmbParams UmbParams { get; set; }
    }

    public partial class TrackParams
    {
        public long ShopId { get; set; }
        public string Detailabtestdetail { get; set; }
    }

    public partial class UmbParams
    {
        public string AliBizName { get; set; }
        public string AliBizCode { get; set; }
    }

    public partial class PriceClass
    {
        public List<PurplePrice> NewExtraPrices { get; set; }
        public List<PurplePrice> ExtraPrices { get; set; }
        public PurplePrice TransmitPrice { get; set; }
        public string ShopPromTitle { get; set; }
        public List<object> SuperMarketShopProm { get; set; }
        public List<ShopProm> ShopProm { get; set; }
        public PurplePrice Price { get; set; }
        public List<PriceTag> PriceTag { get; set; }
    }

    public partial class PurplePrice
    {
        public long PriceText { get; set; }
        public string PriceTitle { get; set; }
        public bool ShowTitle { get; set; }
        public bool? LineThrough { get; set; }
        public bool SugProm { get; set; }
        public long? PriceMoney { get; set; }
    }

    public partial class PriceTag
    {
        public string Icon { get; set; }
        public bool BigmarkdownTag { get; set; }
    }

    public partial class ShopProm
    {
        public string IconText { get; set; }
        public string Period { get; set; }
        public List<string> Content { get; set; }
        public long Type { get; set; }
    }

    public partial class PromotionFloatingData
    {
        public bool ShowWarm { get; set; }
        public bool ShowNow { get; set; }
    }

    public partial class Resource
    {
        public Diversion IndexCouponData { get; set; }
        public Entrances Entrances { get; set; }
        public string EntrancesBizsContent { get; set; }
        public Diversion Coupon { get; set; }
        public PromsCalcInfo PromsCalcInfo { get; set; }
        public BigPromotion BigPromotion { get; set; }
    }

    public partial class BigPromotion
    {
        public string PicUrl { get; set; }
        public string BgPicUrl { get; set; }
        public List<Memo> Memo { get; set; }
    }

    public partial class Memo
    {
        public string Text { get; set; }
        public string TextColor { get; set; }
    }

    public partial class Entrances
    {
        public Double11Banner Double11Banner { get; set; }
    }

    public partial class Double11Banner
    {
        public string Icon { get; set; }
    }

    public partial class PromsCalcInfo
    {
        public long CheapestMoney { get; set; }
        public bool HasCoupon { get; set; }
        public bool NeedReqCouDan { get; set; }
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
        public long SortOrder { get; set; }
    }

    public partial class Skus
    {
        public string SkuId { get; set; }
        public string PropPath { get; set; }
    }

    public partial class SkuCore
    {
        public SkuItem SkuItem { get; set; }
        public Dictionary<string, Sku2Info> Sku2Info { get; set; }
    }

    public partial class Sku2Info
    {
        public PurplePrice Price { get; set; }
        public long Quantity { get; set; }
    }

    public partial class SkuItem
    {
        public bool ShowAddress { get; set; }
        public bool HideQuantity { get; set; }
        public string Location { get; set; }
    }

    public partial class SkuVertical
    {
        public Installment Installment { get; set; }
    }

    public partial class Installment
    {
        public string Title { get; set; }
        public List<Period> Period { get; set; }
        public bool Enable { get; set; }
    }

    public partial class Period
    {
        public long Count { get; set; }
        public string Ratio { get; set; }
        public long CouponPrice { get; set; }
    }

    public partial class Trade
    {
        public bool BuyEnable { get; set; }
        public bool UseWap { get; set; }
        public bool CartEnable { get; set; }
        public Diversion TradeParams { get; set; }
        public Param BuyParam { get; set; }
        public long AuctionStatus { get; set; }
        public bool IsWap { get; set; }
        public Param CartParam { get; set; }
        public Diversion OuterCartParam { get; set; }
        public bool CartConfirmEnable { get; set; }
    }

    public partial class Param
    {
        public long AreaId { get; set; }
    }

    public partial class Vertical
    {
        public AskAll AskAll { get; set; }
        public FreshFood FreshFood { get; set; }
        public TmallLeaseData TmallLeaseData { get; set; }
        public string Degrade { get; set; }
    }

    public partial class AskAll
    {
        public long ShowNum { get; set; }
        public Uri AskIcon { get; set; }
        public string AskText { get; set; }
        public long QuestNum { get; set; }
        public string LinkUrl { get; set; }
        public List<ModelList> ModelList { get; set; }
        public string Title { get; set; }
    }

    public partial class ModelList
    {
        public string AskText { get; set; }
        public string AnswerCountText { get; set; }
    }

    public partial class FreshFood
    {
        public string NationalFlag { get; set; }
    }

    public partial class TmallLeaseData
    {
        public long RentOfficialPriceFen { get; set; }
        public long RentGuaranteePriceFen { get; set; }
        public bool ZhiMaSigned { get; set; }
    }
}
*/
namespace NSClientInfo
{
    public partial class ClientInfo
    {
        public string Status { get; set; }
        public Data Data { get; set; }
    }

    public partial class Data
    {
        public string Id { get; set; }
        public string ShopeeUsername { get; set; }
        public string ShopeePassword { get; set; }
        public Uri ShopeeUrl { get; set; }
        public long ShopeeMinRevenue { get; set; }
        public long ShopeeMaxRevenue { get; set; }
        public string AccessToken { get; set; }
        public string CompanyId { get; set; }
        public string ClientName { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

    }
}

namespace NSShopeeProduct
{
    public partial class ShopeeProduct
    {
        public Item Item { get; set; }
        public string Version { get; set; }
        public object Data { get; set; }
        public object ErrorMsg { get; set; }
        public object Error { get; set; }
    }

    public partial class Item
    {
        public long Itemid { get; set; }
        public long PriceMaxBeforeDiscount { get; set; }
        public string ItemStatus { get; set; }
        public bool CanUseWholesale { get; set; }
        public bool ShowFreeShipping { get; set; }
        public long EstimatedDays { get; set; }
        public object IsHotSales { get; set; }
        public bool IsSlashPriceItem { get; set; }
        public object UpcomingFlashSale { get; set; }
        public object SlashLowestPrice { get; set; }
        public long Condition { get; set; }
        public object AddOnDealInfo { get; set; }
        public bool IsNonCcInstallmentPaymentEligible { get; set; }
        public List<Category> Categories { get; set; }
        public long Ctime { get; set; }
        public string Name { get; set; }
        public bool ShowShopeeVerifiedLabel { get; set; }
        public object SizeChart { get; set; }
        public bool IsPreOrder { get; set; }
        public long ServiceByShopeeFlag { get; set; }
        public long HistoricalSold { get; set; }
        public string ReferenceItemId { get; set; }
        public object RecommendationInfo { get; set; }
        public object BundleDealInfo { get; set; }
        public long PriceMax { get; set; }
        public bool HasLowestPriceGuarantee { get; set; }
        public long ShippingIconType { get; set; }
        public List<string> Images { get; set; }
        public long PriceBeforeDiscount { get; set; }
        public long CodFlag { get; set; }
        public long Catid { get; set; }
        public bool IsOfficialShop { get; set; }
        public object CoinEarnLabel { get; set; }
        public List<object> HashtagList { get; set; }
        public long Sold { get; set; }
        public object Makeup { get; set; }
        public ItemRating ItemRating { get; set; }
        public bool ShowOfficialShopLabelInTitle { get; set; }
        public object Discount { get; set; }
        public string Reason { get; set; }
        public List<long> LabelIds { get; set; }
        public bool HasGroupBuyStock { get; set; }
        public List<Attribute> Attributes { get; set; }
        public long BadgeIconType { get; set; }
        public bool Liked { get; set; }
        public long CmtCount { get; set; }
        public string Image { get; set; }
        public bool IsCcInstallmentPaymentEligible { get; set; }
        public long Shopid { get; set; }
        public long NormalStock { get; set; }
        public List<object> VideoInfoList { get; set; }
        public object InstallmentPlans { get; set; }
        public object ViewCount { get; set; }
        public bool CurrentPromotionHasReserveStock { get; set; }
        public long LikedCount { get; set; }
        public bool ShowOfficialShopLabel { get; set; }
        public long PriceMinBeforeDiscount { get; set; }
        public long ShowDiscount { get; set; }
        public object PreviewInfo { get; set; }
        public long Flag { get; set; }
        public long CurrentPromotionReservedStock { get; set; }
        public List<object> WholesaleTierList { get; set; }
        public object GroupBuyInfo { get; set; }
        public bool ShopeeVerified { get; set; }
        public object HiddenPriceDisplay { get; set; }
        public string TransparentBackgroundImage { get; set; }
        public object WelcomePackageInfo { get; set; }
        public CoinInfo CoinInfo { get; set; }
        public object IsAdult { get; set; }
        public string Currency { get; set; }
        public long RawDiscount { get; set; }
        public bool IsCategoryFailed { get; set; }
        public long PriceMin { get; set; }
        public bool CanUseBundleDeal { get; set; }
        public long CbOption { get; set; }
        public string Brand { get; set; }
        public long Stock { get; set; }
        public long Status { get; set; }
        public long BundleDealId { get; set; }
        public object IsGroupBuyItem { get; set; }
        public string Description { get; set; }
        public object FlashSale { get; set; }
        public List<object> Models { get; set; }
        public long Price { get; set; }
        public string ShopLocation { get; set; }
        public List<object> TierVariations { get; set; }
        public object Makeups { get; set; }
        public long WelcomePackageType { get; set; }
        public object ShowOfficialShopLabelInNormalPosition { get; set; }
        public long ItemType { get; set; }
    }

    public partial class Attribute
    {
        public bool IsPendingQc { get; set; }
        public long Idx { get; set; }
        public string Value { get; set; }
        public long Id { get; set; }
        public bool IsTimestamp { get; set; }
        public string Name { get; set; }
    }

    public partial class Category
    {
        public string DisplayName { get; set; }
        public long Catid { get; set; }
        public object Image { get; set; }
        public bool NoSub { get; set; }
        public bool IsDefaultSubcat { get; set; }
        public object BlockBuyerPlatform { get; set; }
    }

    public partial class CoinInfo
    {
        public long SpendCashUnit { get; set; }
        public List<object> CoinEarnItems { get; set; }
    }

    public partial class ItemRating
    {
        public long RatingStar { get; set; }
        public List<long> RatingCount { get; set; }
        public long RcountWithImage { get; set; }
        public long RcountWithContext { get; set; }
    }

}