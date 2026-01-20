using System.Collections.Immutable;
using BlazorMVU;

namespace BlazorMVU.Demo.Components;

/// <summary>
/// A complex shopping cart component demonstrating advanced MVU features:
/// - Commands for async operations
/// - Subscriptions for auto-save
/// - Time-travel debugging support
/// - Complex state management
/// </summary>
public partial class MvuShoppingCart
{
    // Domain Types
    public record Product(string Id, string Name, decimal Price);
    public record CartItem(Product Product, int Quantity);

    // Model
    public record Model(
        ImmutableList<Product> Products,
        ImmutableList<CartItem> CartItems,
        string CouponCode,
        string? AppliedCoupon,
        decimal Discount,
        bool IsLoading,
        bool IsApplyingCoupon,
        bool IsCheckingOut,
        string? Error,
        DateTime? LastSaved)
    {
        public decimal Subtotal => CartItems.Sum(item => item.Product.Price * item.Quantity);
        public decimal Total => Math.Max(0, Subtotal - Discount);

        public static Model Initial => new(
            Products: [],
            CartItems: [],
            CouponCode: "",
            AppliedCoupon: null,
            Discount: 0,
            IsLoading: true,
            IsApplyingCoupon: false,
            IsCheckingOut: false,
            Error: null,
            LastSaved: null);
    }

    // Messages
    public abstract record Msg
    {
        // Product loading
        public record LoadProducts : Msg;
        public record ProductsLoaded(MvuResult<ImmutableList<Product>> Result) : Msg;

        // Cart operations
        public record AddToCart(Product Product) : Msg;
        public record RemoveFromCart(string ProductId) : Msg;
        public record IncrementQuantity(string ProductId) : Msg;
        public record DecrementQuantity(string ProductId) : Msg;
        public record ClearCart : Msg;

        // Coupon
        public record UpdateCouponCode(string Code) : Msg;
        public record ApplyCoupon(string Code) : Msg;
        public record CouponApplied(MvuResult<decimal> Result, string Code) : Msg;

        // Checkout
        public record Checkout : Msg;
        public record CheckoutComplete(MvuResult<string> Result) : Msg;

        // Auto-save
        public record AutoSave : Msg;
        public record SaveComplete(DateTime SavedAt) : Msg;
    }

    // Initialize with command to load products
    protected override (Model Model, Cmd<Msg> Cmd) InitWithCmd()
    {
        return (Model.Initial, Cmd.OfMsg<Msg>(new Msg.LoadProducts()));
    }

    protected override Model Init() => Model.Initial;

    // Subscriptions - auto-save every 30 seconds
    protected override Sub<Msg> Subscriptions(Model model)
    {
        // Only auto-save if cart has items and not in the middle of an operation
        if (model.CartItems.Count > 0 && !model.IsLoading && !model.IsCheckingOut)
        {
            return Sub.Every<Msg>(TimeSpan.FromSeconds(30), new Msg.AutoSave(), "autosave");
        }
        return Sub.None<Msg>();
    }

    // Update with commands
    protected override (Model Model, Cmd<Msg> Cmd) UpdateWithCmd(Msg msg, Model model)
    {
        return msg switch
        {
            // Loading products
            Msg.LoadProducts => (
                model with { IsLoading = true, Error = null },
                Cmd.OfTask<Msg>(LoadProductsAsync)),

            Msg.ProductsLoaded loaded => loaded.Result.IsSuccess
                ? (model with { IsLoading = false, Products = loaded.Result.Value ?? [] }, Cmd.None<Msg>())
                : (model with { IsLoading = false, Error = loaded.Result.Error?.Message ?? "Failed to load products" }, Cmd.None<Msg>()),

            // Cart operations
            Msg.AddToCart add => (AddToCart(model, add.Product), Cmd.None<Msg>()),
            Msg.RemoveFromCart remove => (RemoveFromCart(model, remove.ProductId), Cmd.None<Msg>()),
            Msg.IncrementQuantity inc => (UpdateQuantity(model, inc.ProductId, 1), Cmd.None<Msg>()),
            Msg.DecrementQuantity dec => (UpdateQuantity(model, dec.ProductId, -1), Cmd.None<Msg>()),
            Msg.ClearCart => (model with { CartItems = [], AppliedCoupon = null, Discount = 0 }, Cmd.None<Msg>()),

            // Coupon
            Msg.UpdateCouponCode update => (model with { CouponCode = update.Code }, Cmd.None<Msg>()),
            Msg.ApplyCoupon apply => (
                model with { IsApplyingCoupon = true },
                Cmd.OfTask<Msg>(ct => ApplyCouponAsync(apply.Code, model.Subtotal, ct))),
            Msg.CouponApplied coupon => coupon.Result.IsSuccess
                ? (model with { IsApplyingCoupon = false, AppliedCoupon = coupon.Code, Discount = coupon.Result.Value, CouponCode = "" }, Cmd.None<Msg>())
                : (model with { IsApplyingCoupon = false, Error = coupon.Result.Error?.Message }, Cmd.None<Msg>()),

            // Checkout
            Msg.Checkout => (
                model with { IsCheckingOut = true },
                Cmd.OfTask<Msg>(ct => CheckoutAsync(model, ct))),
            Msg.CheckoutComplete complete => complete.Result.IsSuccess
                ? (Model.Initial with { IsLoading = false, Products = model.Products }, Cmd.None<Msg>())
                : (model with { IsCheckingOut = false, Error = complete.Result.Error?.Message }, Cmd.None<Msg>()),

            // Auto-save
            Msg.AutoSave => (model, Cmd.OfTask<Msg>(ct => SaveCartAsync(model, ct))),
            Msg.SaveComplete save => (model with { LastSaved = save.SavedAt }, Cmd.None<Msg>()),

            _ => (model, Cmd.None<Msg>())
        };
    }

    protected override Model Update(Msg msg, Model model) => UpdateWithCmd(msg, model).Model;

    // Helper methods for state updates
    private static Model AddToCart(Model model, Product product)
    {
        var existingItem = model.CartItems.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existingItem is not null)
        {
            var updatedItem = existingItem with { Quantity = existingItem.Quantity + 1 };
            return model with { CartItems = model.CartItems.Replace(existingItem, updatedItem) };
        }
        return model with { CartItems = model.CartItems.Add(new CartItem(product, 1)) };
    }

    private static Model RemoveFromCart(Model model, string productId)
    {
        var item = model.CartItems.FirstOrDefault(i => i.Product.Id == productId);
        if (item is null) return model;
        return model with { CartItems = model.CartItems.Remove(item) };
    }

    private static Model UpdateQuantity(Model model, string productId, int delta)
    {
        var item = model.CartItems.FirstOrDefault(i => i.Product.Id == productId);
        if (item is null) return model;

        var newQuantity = item.Quantity + delta;
        if (newQuantity <= 0)
        {
            return model with { CartItems = model.CartItems.Remove(item) };
        }

        var updatedItem = item with { Quantity = newQuantity };
        return model with { CartItems = model.CartItems.Replace(item, updatedItem) };
    }

    // Async operations (simulated)
    private static async Task<Msg> LoadProductsAsync(CancellationToken ct)
    {
        await Task.Delay(800, ct); // Simulate API call

        var products = ImmutableList.Create(
            new Product("1", "Laptop", 999.99m),
            new Product("2", "Mouse", 29.99m),
            new Product("3", "Keyboard", 79.99m),
            new Product("4", "Monitor", 349.99m),
            new Product("5", "Headphones", 149.99m)
        );

        return new Msg.ProductsLoaded(MvuResult<ImmutableList<Product>>.Success(products));
    }

    private static async Task<Msg> ApplyCouponAsync(string code, decimal subtotal, CancellationToken ct)
    {
        await Task.Delay(500, ct); // Simulate API validation

        // Simulated coupon validation
        var discount = code.ToUpperInvariant() switch
        {
            "SAVE10" => subtotal * 0.10m,
            "SAVE20" => subtotal * 0.20m,
            "FLAT50" => 50m,
            _ => -1m
        };

        if (discount < 0)
        {
            return new Msg.CouponApplied(MvuResult<decimal>.Failure("Invalid coupon code"), code);
        }

        return new Msg.CouponApplied(MvuResult<decimal>.Success(discount), code);
    }

    private static async Task<Msg> CheckoutAsync(Model model, CancellationToken ct)
    {
        await Task.Delay(1500, ct); // Simulate checkout process

        // Simulated checkout - always succeeds for demo
        return new Msg.CheckoutComplete(MvuResult<string>.Success("Order placed successfully!"));
    }

    private static async Task<Msg> SaveCartAsync(Model model, CancellationToken ct)
    {
        await Task.Delay(200, ct); // Simulate save to localStorage/API

        return new Msg.SaveComplete(DateTime.UtcNow);
    }
}
